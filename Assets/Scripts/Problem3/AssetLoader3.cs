using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class AssetLoader3 : MonoBehaviour
{
    [field: SerializeField]
    public LoaderModule3 LoaderModule { get; set; }
    public GameObject MainCamera;
    private string projectPath;
    private List<Vector3> modelMap = new List<Vector3>();

    private void Start()
    {
        InitializeModelMap();

        projectPath = Application.dataPath;

        // OpenFilePanel's root directory is "Assets"
        List<string> selectedAssetPaths = GetObjFiles("/Models");

        if(selectedAssetPaths.Count > 0) 
            Load(selectedAssetPaths);
        Debug.Log("All Load Async functions called");
    }

    private List<string> GetObjFiles(string directory)
    {
        if(Directory.Exists(projectPath + directory) == false)
        {
            Debug.LogError("Directory isn't exist");
            return new();
        }
        List<string> files = new List<string>();
        files.AddRange(Directory.GetFiles(projectPath + directory, "*.obj"));
        for (int i = 0; i < files.Count; i++) files[i] = files[i].Replace('\\', '/');

        foreach (string file in files)
        {
            Debug.Log("file path: " + file);
        }

        return files;
    }

    public async void Load(List<string> paths)
    {
        List<Task<GameObject>> loadTasks = new List<Task<GameObject>>();
        int modelCount = 0;
        Debug.Log("Load function");
        for(int i = 0; i < paths.Count; i++)
        {
            loadTasks.Add(LoaderModule.LoadAssetAsync(paths[i]));
        }

        while(loadTasks.Count > 0)
        {
            Task<GameObject> completedTask = await Task.WhenAny(loadTasks);
            loadTasks.Remove(completedTask);

            GameObject loadedAsset = await completedTask;
            Debug.Log("object load complete : " + loadedAsset.name);

            await Task.Yield();
            loadedAsset.transform.rotation = Quaternion.LookRotation(MainCamera.transform.position);
            loadedAsset.transform.SetParent(transform);
            loadedAsset.transform.position = modelMap[modelCount++];
        }
    }

    private void InitializeModelMap()
    {
        int rows = 4;
        int cols = 5;
        float spacing = 5.0f;

        for (int x = 0; x < rows; x++)
        {
            for (int z = 0; z < cols; z++)
            {
                Vector3 position = new Vector3(x * spacing, 0, z * spacing);
                modelMap.Add(position);
            }
        }
    }
}
