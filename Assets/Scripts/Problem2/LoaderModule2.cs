using System.Threading.Tasks;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class LoaderModule2 : MonoBehaviour
{
    private GameObject loadedAsset;

    public async Task<GameObject> LoadAssetAsync(string assetName){
        Debug.Log("load asset async");
        string relativePath = SliceRelativePath(assetName);
        loadedAsset = await ObjectLoader(relativePath);
        Debug.Log("object load complete");

        return loadedAsset;
    }

    private string SliceRelativePath(string path){
        Debug.Log("slice relative path");
        int index = path.IndexOf("Assets/Models/");
        return path.Substring(index);
    }


    private async Task<GameObject> ObjectLoader(string path)
    {
        // read obj file
        string fileContent = await File.ReadAllTextAsync(path);

        // vertex, uv, normal, face list
        List<Vector3> vertices = new List<Vector3>();
        List<Vector2> uv = new List<Vector2>();
        List<Vector3> normals = new List<Vector3>();
        bool normalDataExist = false;
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
                normalDataExist = true;
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

        Mesh mesh = new Mesh();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = faces.ToArray();

        // uv coordinate set
        if (uv.Count > 0 && uv.Count == vertices.Count)
        {
            mesh.uv = uv.ToArray();
        }

        // mesh normal handle
        if (normalDataExist)
        { // normal data exists
            mesh.normals = normals.ToArray();
        }
        else
        { // normal data no exists
            mesh.RecalculateNormals();
        }

        MeshFilter meshFilter = loadedObject.AddComponent<MeshFilter>();
        meshFilter.mesh = mesh;
        MeshRenderer meshRenderer = loadedObject.AddComponent<MeshRenderer>();
        meshRenderer.material = new Material(Shader.Find("Standard")); // use standar shader

        return await Task.FromResult(loadedObject);

    }
}