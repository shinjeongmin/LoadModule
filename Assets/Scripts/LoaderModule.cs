using System;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class LoaderModule : MonoBehaviour
{
    public Action<GameObject> OnLoadCompleted;
    public GameObject loadedPrefab;
    private GameObject loadedAsset;

    public void LoadAsset(string assetName)
    {   
        string relativePath = SliceRelativePath(assetName);
        Debug.Log("LoadAsset : " + relativePath);
        loadedPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(relativePath);

        loadedAsset = Instantiate(loadedPrefab, Vector3.zero, Quaternion.LookRotation(GameObject.Find("Main Camera").gameObject.transform.position));
        OnLoadCompleted.Invoke(loadedAsset);
    }
    public async Task<GameObject> LoadAssetAsync(string assetName){
        string relativePath = SliceRelativePath(assetName);
        
        // TODO: create gameobject and obj model from assetName
        return new();
    }

    private string SliceRelativePath(string path){
        int index = path.IndexOf("Assets/Models/");
        return path.Substring(index);
    }
}