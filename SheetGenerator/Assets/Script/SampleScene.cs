using System.Collections;
using System.Collections.Generic;
using FrameWork;
using SheetData;
using UnityEngine;

public class SampleScene : MonoSingleton<SampleScene>
{
    public SheetDownloadConfig Config;
    
    // Start is called before the first frame update
    void Start()
    {
        Config.Download((progress) =>
        {
            Debug.Log(string.Format("Progress Update {0}",progress));
        }, () =>
        {
            Debug.Log("Download Complete");
            
            LocalDataManager.Instance.Load();
            var sheet = LocalDataManager.Instance.GetDefinitions<SampleSheet>();
            foreach (var row in sheet)
            {
                var data = row.Value as SampleSheet;
                Debug.Log(string.Format("Key:{0} / Value:{1} / Comment:{2}", data.key, data.value, data.comment));
            }
        });
        
    }
}
