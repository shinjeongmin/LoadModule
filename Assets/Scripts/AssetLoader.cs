using System;
using UnityEditor;
using UnityEngine;

public class AssetLoader : MonoBehaviour
{
    [field: SerializeField]
    public LoaderModule LoaderModule { get; set; }

    /// modify projectPath string to own project path. 
    [Header("프로젝트 열기 경로")]
    public string projectPath = "/Users/shinjeongmin/Documents/UnityProjects/LoadModule";
    public string assetName = "bunny";

    private void Start()
    {
        // OpenFilePanel's root directory is "Assets"
        string selectedAssetName = EditorUtility.OpenFilePanel("Select obj model", projectPath + "/Assets/Models" , "obj");
        // string relativePath = selectedAssetName.Replace(projectPath + '/', "");
        Debug.Log(selectedAssetName);
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
            Debug.Log("Loaded Asset complete.");
        }
        else{
            Debug.Log("loadedAsset object is returned null.");
        }
    }
}
