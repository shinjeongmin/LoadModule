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
        Debug.Log(projectPath);

        // OpenFilePanel's root directory is "Assets"
        string selectedAssetName = EditorUtility.OpenFilePanel("Select obj model", projectPath + "/Models" , "obj");

        if (!string.IsNullOrEmpty(selectedAssetName))
        {
            Load(selectedAssetName);
        }
        Debug.Log("먼저 실행");
    }

    public async void Load(string assetName)
    {
        Debug.Log("Load 함수");
        GameObject loadedAsset = await LoaderModule.LoadAssetAsync(assetName);
        Debug.Log("asset 반환");

        GameObject newAssetObject = Instantiate(loadedAsset, Vector3.zero, Quaternion.LookRotation(MainCamera.transform.position));
        newAssetObject.transform.SetParent(transform);
    }
}
