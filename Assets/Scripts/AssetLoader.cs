using System;
using UnityEditor;
using UnityEngine;

public class AssetLoader : MonoBehaviour
{
    [field: SerializeField]
    public LoaderModule LoaderModule { get; set; }

    private string projectPath;
    public string assetName = "bunny";

    private void Start()
    {
        projectPath = Application.dataPath;
        Debug.Log(projectPath);

        // OpenFilePanel's root directory is "Assets"
        string selectedAssetName = EditorUtility.OpenFilePanel("Select obj model", projectPath + "/Models" , "obj");

        Load(selectedAssetName);
        // LoadAsync(selectedAssetName);
    }

    public void Load(string assetName) // TODO: need to remove
    {
        LoaderModule.OnLoadCompleted += OnLoadCompleted;
        LoaderModule.LoadAsset(assetName);
    }

    public async void LoadAsync(string assetName)
    {
        GameObject loadedAsset = await LoaderModule.LoadAssetAsync(assetName);
        loadedAsset.transform.SetParent(transform);
    }

    private void OnLoadCompleted(GameObject loadedAsset)
    {
        if(loadedAsset != null){
            loadedAsset.transform.SetParent(transform);
            Debug.Log("Loaded Asset complete.");
        }
        else{
            Debug.Log("loadedAsset object is returned null.");
        }
    }
}
