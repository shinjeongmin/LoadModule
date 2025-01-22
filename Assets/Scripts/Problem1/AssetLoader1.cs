using System;
using UnityEditor;
using UnityEngine;

public class AssetLoader1 : MonoBehaviour
{
    [field: SerializeField]
    public LoaderModule1 LoaderModule { get; set; }

    private string projectPath;
    public string assetName = "bunny";

    private void Start()
    {
        projectPath = Application.dataPath;
        Debug.Log(projectPath);

        // OpenFilePanel's root directory is "Assets"
        string selectedAssetName = EditorUtility.OpenFilePanel("Select obj model", projectPath + "/Models" , "obj");

        Load(selectedAssetName);
    }

    public void Load(string assetName)
    {
        LoaderModule.OnLoadCompleted += OnLoadCompleted;
        LoaderModule.LoadAsset(assetName);
    }

    private void OnLoadCompleted(GameObject loadedAsset)
    {
        if(loadedAsset != null){
            loadedAsset.transform.SetParent(transform);
            loadedAsset.transform.name = assetName;
            Debug.Log("Loaded Asset complete.");
        }
        else{
            Debug.LogError("loadedAsset object is returned null.");
        }
    }
}
