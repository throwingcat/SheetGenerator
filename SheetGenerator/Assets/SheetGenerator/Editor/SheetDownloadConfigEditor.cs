using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using UnityEditor;
using UnityEngine;

namespace FrameWork
{
    [CustomEditor(typeof(SheetDownloadConfig))]
    public class SheetDownloadConfigEditor : Editor
    {
        SheetDownloadConfig asset
        {
            get { return (SheetDownloadConfig) target; }
        }

        private bool _isDownload = false;
        private float _downloadProgress = 0;

        private bool _isGenerate = false;
        private float _generateProgress = 0;

        public override void OnInspectorGUI()
        {
            DrawTitle("Sheet 다운로드");
            if (GUILayout.Button("다운로드") == true)
            {
                _isDownload = true;
                _downloadProgress = 0f;
                asset.Download(
                    (progress) => { _downloadProgress = progress; },
                    () => { _isDownload = false; });
            }

            if (_isDownload == true)
                DrawProgressBar(_downloadProgress);


            DrawTitle("클래스 생성");
            if (asset.ClassCreateFolder == null)
                asset.ClassCreateFolder = AssetDatabase.LoadAssetAtPath<Object>("Assets/FrameWork/Definition");
            EditorGUILayout.ObjectField("Class Create Folder", asset.ClassCreateFolder, typeof(Object), true);
            
            if (GUILayout.Button("클래스 생성") == true)
            {
                _isGenerate = true;
                _generateProgress = 0f;
                asset.Generate(
                    (progress) => { _generateProgress = progress; },
                    () =>
                    {
                        _isGenerate = false;
                        AssetDatabase.Refresh();
                    });
            }

            if (_isGenerate == true)
                DrawProgressBar(_generateProgress);

            DrawTitle("CSV 목록");
            GUILayout.Label("CSV List");
            GUILayout.Label(string.Format("{0} items", asset.Config.Files.Count));

            GUILayout.Space(-4);

            for (int i = 0; i < asset.Config.Files.Count; i++)
            {
                using (var h = new GUILayout.HorizontalScope("box"))
                {
                    EditorGUILayout.VerticalScope vScope = null;
                    using (vScope = new EditorGUILayout.VerticalScope())
                    {
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("URL");
                        string url = asset.Config.BuildURL(asset.Config.Files[i]);
                        GUILayout.Label(url, GUILayout.MaxWidth(Screen.width * 0.6f));
                        GUILayout.EndHorizontal();

                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Name");
                        GUILayout.Label(asset.Config.Files[i].Name, GUILayout.MaxWidth(Screen.width * 0.6f));
                        GUILayout.EndHorizontal();
                    }
                }
            }

            Undo.RecordObject(asset, "Update CSVDownload Data");
        }


        private void DrawTitle(string title)
        {
            GUILayout.Label(title, EditorStyles.boldLabel);
            DrawLine();
        }

        GUIStyle horizontalLine = null;

        private void DrawLine()
        {
            if (horizontalLine == null)
            {
                horizontalLine = new GUIStyle();
                horizontalLine.normal.background = EditorGUIUtility.whiteTexture;
                horizontalLine.margin = new RectOffset(0, 0, 4, 4);
                horizontalLine.fixedHeight = 1;
            }

            var c = GUI.color;
            GUI.color = Color.black;
            GUILayout.Box(GUIContent.none, horizontalLine);
            GUI.color = c;
        }

        private void DrawProgressBar(float value)
        {
            GUILayout.Box("", GUILayout.ExpandWidth(true), GUILayout.Height(20));
            GUILayout.Space(-24);
            GUI.color = Color.green;
            GUILayout.Box("", GUILayout.Width(Screen.width * value), GUILayout.Height(20));
            GUI.color = Color.white;
        }
    }
}