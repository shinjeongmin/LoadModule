using System.Threading.Tasks;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class LoaderModule3 : MonoBehaviour
{
    private GameObject loadedAsset;

    public async Task<GameObject> LoadAssetAsync(string path){
        string relativePath = SliceRelativePath(path);

        loadedAsset = await ObjectLoader(relativePath);

        loadedAsset.name = relativePath.Split('/')[relativePath.Split('/').Length - 1];
        //await Task.Yield();

        return loadedAsset;
    }

    private string SliceRelativePath(string path){
        int index = path.IndexOf("Assets/Models/");
        return path.Substring(index);
    }

    private async Task<GameObject> ObjectLoader(string path)
    {
        string fileContent;
        try
        {
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true))
            using (StreamReader reader = new StreamReader(fs))
            {
                fileContent = await reader.ReadToEndAsync();
            }
        }
        catch(FileLoadException e)
        {
            Debug.LogError("File Load Exception : " + e);
            return null;
        }

        // vertex, uv, normal, face list
        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uv = new List<Vector2>();
        List<Vector3> normals = new List<Vector3>();
        bool normalExist = false;
        bool uvExist = false;
        List<int> faces = new List<int>();

        // parse obj file
        string[] lines = fileContent.Split('\n');
        foreach (string line in lines)
        {
            string trimmedLine = line.Trim();

            // vertex
            if (trimmedLine.StartsWith("v "))
            {
                string[] parts = trimmedLine.Split(' ');
                if (parts.Length >= 4)
                {
                    float x = float.Parse(parts[1]);
                    float y = float.Parse(parts[2]);
                    float z = float.Parse(parts[3]);
                    vertices.Add(new Vector3(x, y, z));
                }
            }
            // vertex texture
            else if (trimmedLine.StartsWith("vt "))
            {
                uvExist = true;
                string[] parts = trimmedLine.Split(' ');
                if (parts.Length >= 3)
                {
                    float u = float.Parse(parts[1]);
                    float v = float.Parse(parts[2]);
                    uv.Add(new Vector2(u, v));
                }
            }
            // normal
            else if (trimmedLine.StartsWith("vn "))
            {
                normalExist = true;
                string[] parts = trimmedLine.Split(' ');
                if (parts.Length >= 4)
                {
                    float x = float.Parse(parts[1]);
                    float y = float.Parse(parts[2]);
                    float z = float.Parse(parts[3]);
                    normals.Add(new Vector3(x, y, z));
                }
            }
            // face
            else if (trimmedLine.StartsWith("f "))
            {
                string[] parts = trimmedLine.Split(' ');
                for (int i = 1; i < parts.Length; i++)
                {
                    string[] vertexInfo = parts[i].Split('/');

                    // obj face is 1-based indexing convert to 0-based indexing
                    int vertexIndex = int.Parse(vertexInfo[0]) - 1;
                    faces.Add(vertexIndex);
                }
            }
        }

        GameObject loadedObject = new GameObject("loadedObject");

        // mesh component
        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = faces.ToArray();

        // uv coordinate set
        if (uv.Count > 0 && uv.Count == vertices.Count)
        {
            mesh.uv = uv.ToArray();
        }

        // mesh normal handle
        if (normalExist)
        { // apply normal data
            mesh.normals = normals.ToArray();
        }
        else
        { // calculate normal 
            mesh.RecalculateNormals();
        }

        MeshFilter meshFilter = loadedObject.AddComponent<MeshFilter>();
        meshFilter.mesh = mesh;
        MeshRenderer meshRenderer = loadedObject.AddComponent<MeshRenderer>();
        meshRenderer.material = new Material(Shader.Find("Standard")); // use standar shader

        return await Task.FromResult(loadedObject);

    }
}