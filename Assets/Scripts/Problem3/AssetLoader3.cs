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
    public string assetName = "bunny";

    private void Start()
    {
        projectPath = Application.dataPath;

        // OpenFilePanel's root directory is "Assets"
        //string selectedAssetPath = EditorUtility.OpenFilePanel("Select obj model", projectPath + "/Models" , "obj");
        List<string> selectedAssetPaths = GetObjFiles("/Models");

        if(selectedAssetPaths.Count > 0) 
            Load(selectedAssetPaths);
        Debug.Log("End Start");
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
            Debug.Log("ÆÄÀÏ: " + file);
        }

        return files;
    }

    public async void Load(List<string> paths)
    {
        List<Task<GameObject>> loadTasks = new List<Task<GameObject>>();
        Debug.Log("Load function");
        for(int i = 0; i < paths.Count; i++)
        {
            // method 1: main thread is waiting
            //loadTasks.Add(LoaderModule.LoadAssetAsync(paths[i]));

            // method 2: 
            await LoaderModule.LoadAssetAsync(paths[i]).ContinueWith(task =>
            {
                if (task.Status == TaskStatus.RanToCompletion)
                {
                    GameObject loadedAsset = task.Result;
                }
            });
        }

        while(loadTasks.Count > 0)
        {
            Task<GameObject> completedTask = await Task.WhenAny(loadTasks);
            loadTasks.Remove(completedTask);

            GameObject loadedAsset = await completedTask;

            //loadedAsset.transform.rotation = Quaternion.LookRotation(MainCamera.transform.position);
            //loadedAsset.transform.SetParent(transform);
        }
    }
}
