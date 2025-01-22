using System;
using UnityEditor;
using UnityEngine;

public class AssetLoader2 : MonoBehaviour
{
    [field: SerializeField]
    public LoaderModule2 LoaderModule { get; set; }
    public GameObject MainCamera;
    private string projectPath;
    public string assetName = "bunny";

    private void Start()
    {
        projectPath = Application.dataPath;

        // OpenFilePanel's root directory is "Assets"
        string selectedAssetPath = EditorUtility.OpenFilePanel("Select obj model", projectPath + "/Models" , "obj");

        if (!string.IsNullOrEmpty(selectedAssetPath))
        {
            Load(selectedAssetPath);
        }
        Debug.Log("End Start");
    }

    public async void Load(string path)
    {
        Debug.Log("Load function");
        GameObject loadedAsset = await LoaderModule.LoadAssetAsync(path);
        Debug.Log("Asset return");

        loadedAsset.name = assetName;
        loadedAsset.transform.rotation = Quaternion.LookRotation(MainCamera.transform.position);
        loadedAsset.transform.SetParent(transform);
    }
}
