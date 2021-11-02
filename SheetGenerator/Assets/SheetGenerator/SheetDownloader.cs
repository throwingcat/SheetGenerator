#if UNITY_EDITOR
using System;
using System.Collections;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;
using Object = UnityEngine.Object;

public class SheetDownloader : MonoBehaviour
{
    public Object DownloadFolder;

    public void Download(SheetDownloadData sheetDownloadData)
    {
        var downloadPath = Application.persistentDataPath + "/";

        var path = AssetDatabase.GetAssetPath(DownloadFolder);
        StartCoroutine(DownloadProcess(sheetDownloadData.URL,
            text =>
            {
                if (string.IsNullOrEmpty(text) == false) File.WriteAllText(path + "/" + sheetDownloadData.Name, text);
            }));
    }

    public IEnumerator DownloadProcess(string url, Action<string> onDownloadFinish)
    {
        var req = UnityWebRequest.Get(url);

        yield return req.SendWebRequest();

        if (req.isNetworkError && req.responseCode > 400)
        {
            onDownloadFinish("");
            yield break;
        }

        onDownloadFinish(req.downloadHandler.text);
    }

    public class SheetDownloadData
    {
        public string Name;
        public string URL;
    }
}
#endif