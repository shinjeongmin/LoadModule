using System.Threading.Tasks;
using UnityEngine;
using System.IO;

public class LoaderModule2 : MonoBehaviour
{
    private GameObject loadedAsset;

    public async Task<GameObject> LoadAssetAsync(string assetName){
        Debug.Log("load asset async");
        string relativePath = SliceRelativePath(assetName);
        loadedAsset = await ObjectLoader(relativePath);
        Debug.Log("object load ¿Ï·á");

        return loadedAsset;
    }

    private string SliceRelativePath(string path){
        Debug.Log("slice relative path");
        int index = path.IndexOf("Assets/Models/");
        return path.Substring(index);
    }

    private async Task<GameObject> ObjectLoader(string path)
    {
        string fileContent = await File.ReadAllTextAsync(path);
        Debug.Log("object text : " + fileContent);
        GameObject loadedObject = null;

        return await Task.FromResult(loadedObject);
    }
}