using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace FrameWork
{
    public class LocalDataManager
    {
        private static LocalDataManager _instance;

        private readonly Dictionary<string, Dictionary<string, DefinitionBase>> Table =
            new Dictionary<string, Dictionary<string, DefinitionBase>>();

        private SheetDownloadConfig _configFile;

        public static LocalDataManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new LocalDataManager();
                return _instance;
            }
        }

        private SheetDownloadConfig.SheetDownloadConfigData Config
        {
            get
            {
                // #if UNITY_EDITOR
                // if (_configFile == null)
                //     _configFile =  SheetDownloadConfig.Instance;
                // #else
                if (_configFile == null)
                    _configFile =  SampleScene.Instance.Config;
                //#endif
                return _configFile.Config;
            }
        }

        public void Load()
        {
            foreach (var sheet in Config.Files)
            {
                //테이블 추가
                if (Table.ContainsKey(sheet.Name) == false)
                    Table.Add(sheet.Name, new Dictionary<string, DefinitionBase>());

                //데이터 추가
                var type = Type.GetType("SheetData." + sheet.Name);

                //Sheet 로드
                var file = string.Format("{0}.csv", sheet.Name);
                var path = Path.Combine(SheetDownloadConfig.DOWNLOAD_PATH, file);

                if (SheetDownloadConfig.Sheet.ContainsKey(path) == false)
                {
                    var text = File.ReadAllText(path);
                    SheetDownloadConfig.Sheet.Add(path, text);
                }

                var list = CSVReader.Read(new TextAsset(SheetDownloadConfig.Sheet[path]));
                foreach (var row in list)
                {
                    var key = "";
                    var instance = Activator.CreateInstance(type);

                    foreach (var element in row)
                    {
                        var culumn = element.Key;
                        var value = element.Value;
                        var pi = type.GetProperty(culumn);
                        if (pi != null)
                        {
                            pi.SetValue(instance, Convert.ChangeType(value, pi.PropertyType));

                            if (culumn.Equals("key") || culumn.Equals("Key"))
                                key = value.ToString();

                            continue;
                        }

                        var fi = type.GetField(culumn);
                        if (fi != null)
                        {
                            fi.SetValue(instance, Convert.ChangeType(value, fi.FieldType));

                            if (culumn.Equals("key") || culumn.Equals("Key"))
                                key = value.ToString();
                        }
                    }

                    if (string.IsNullOrEmpty(key) == false) Table[sheet.Name].Add(key, instance as DefinitionBase);
                }
            }

            foreach (var t in Table)
            {
                var type = Type.GetType("SheetData." + t.Key);
                foreach (var v in t.Value)
                {
                    var mi = type.GetMethod("Initialize");
                    mi.Invoke(v.Value, null);
                }
            }
        }

        public Dictionary<string, DefinitionBase> GetDefinitions<T>() where T : DefinitionBase
        {
            var key = typeof(T).Name;
            if (Table.ContainsKey(key))
                return Table[key];

            return new Dictionary<string, DefinitionBase>();
        }

        public T GetDefinition<T>(string key) where T : DefinitionBase
        {
            var tableKey = typeof(T).Name;
            if (Table.ContainsKey(tableKey))
                if (Table[tableKey].ContainsKey(key))
                    return Table[tableKey][key] as T;

            return default;
        }
    }
}