using System.Threading.Tasks;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class LoaderModule2 : MonoBehaviour
{
    private GameObject loadedAsset;
    private int mainThreadYieldCnt = 0;
    private int mainThreadYieldCntMax = 10000;
    public async Task<GameObject> LoadAssetAsync(string path){
        loadedAsset = await ObjectLoader(path);
        Debug.Log("object load complete");
        loadedAsset.name = path.Split('/')[path.Split('/').Length - 1];

        return loadedAsset;
    }

    private async Task<GameObject> ObjectLoader(string path)
    {
        string fileContent;
        try
        {
            // read obj file
            fileContent = await File.ReadAllTextAsync(path);
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
        List<int> faces = new List<int>();

        // parse obj file
        string[] lines = fileContent.Split('\n');
        foreach (string line in lines)
        {
            string trimmedLine = line.Trim();

            // vertex
            if (trimmedLine.StartsWith("v "))
            {
                string[] parts = trimmedLine.Split(new[] { ' ' , '\t' }, System.StringSplitOptions.RemoveEmptyEntries);
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
                string[] unprocessedParts = trimmedLine.Split(' ');
                string[] parts;

                // check face is quad
                if (unprocessedParts.Length == 5)
                {
                    parts = new string[7];
                    // index 0 : 'f'
                    parts[0] = unprocessedParts[0];

                    parts[1] = unprocessedParts[1];
                    parts[2] = unprocessedParts[2];
                    parts[3] = unprocessedParts[3];

                    parts[4] = unprocessedParts[1];
                    parts[5] = unprocessedParts[3];
                    parts[6] = unprocessedParts[4];
                }
                else // face is triangle
                {
                    parts = unprocessedParts;
                }

                for (int i = 1; i < parts.Length; i++)
                {
                    string[] vertexInfo = parts[i].Split('/');

                    // obj face is 1-based indexing convert to 0-based indexing
                    int vertexIndex = int.Parse(vertexInfo[0]) - 1;

                    if (vertexIndex < 0 || vertexIndex >= vertices.Count)
                    {
                        Debug.LogError($"Invalid vertex index: {vertexIndex}");
                    }

                    faces.Add(vertexIndex);
                }
            }

            if (mainThreadYieldCnt++ > mainThreadYieldCntMax)
            {
                await Task.Yield();
                mainThreadYieldCnt = 0;
            }
        }

        GameObject loadedObject = new GameObject("loadedObject");

        // mesh component
        Mesh mesh = new Mesh();
        if (vertices.Count > 65000) mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mesh.vertices = vertices.ToArray();
        mesh.triangles = faces.ToArray();

        // uv coordinate set
        if (uv.Count > 0 && uv.Count == vertices.Count)
        {
            mesh.uv = uv.ToArray();
        }

        // mesh normal handle
        if (normalExist && normals.Count == vertices.Count)
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