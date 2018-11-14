using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Text;

public class ModifyScriptEditor : EditorWindow
{
    [MenuItem("Tools/使用宏Log")]
    public static void ShowGUIMacro()
    {
        EditorWindow.GetWindow<ModifyScriptGUI>(false);
    }
}


public class ModifyScriptGUI : EditorWindow
{
    private string mModuleStr = "";

    void OnGUI()
    {
        EditorGUILayout.EndVertical();
        EditorGUILayout.BeginVertical("Box");
        {
            mModuleStr = EditorGUILayout.TextField("模块路径:", mModuleStr);
        }
        ShowExecute();
    }

    private void GetPathForDebugs(string pPath)
    {
        mDicTxts.Clear();mCates.Clear();
        string mProject = Application.dataPath.Replace("/Assets", "");
        if (string.IsNullOrEmpty(pPath)) pPath = "Assets/Scripts";
        var guids = AssetDatabase.FindAssets("t:Script", new string[] { pPath });
        foreach (var item in guids)
        {
            var tPath = mProject + "/" + AssetDatabase.GUIDToAssetPath(item);
            var tTxt = AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GUIDToAssetPath(item)) as TextAsset;
            if (tTxt.text.Contains("Debug"))
            {
                mDicTxts[tPath] = tTxt.text;
                mCates.Add(tPath);
            }
        }
    }
    private Vector2 mScrollViewCommon;
    private Dictionary<string, string> mDicTxts=new Dictionary<string, string>();
    private List<string> mCates = new List<string>();
    void ShowNotifyLabel(string label)
    {
        this.ShowNotification(new GUIContent(label));
    }
    private void ShowExecute()
    {
        EditorGUILayout.EndVertical();
        EditorGUILayout.BeginVertical("Box");
        {
            GUILayout.Label(@"脚本代码的使用,默认路径(Assets/Script),执行自己跟的模块Log输出,拼接字符串也会产生GC.开发时用宏打印log.打包时去掉宏");
            if (GUILayout.Button("执行", GUILayout.Height(27)))
            {
                GetPathForDebugs(mModuleStr);
            }

            if (mCates != null && mCates.Count > 0)
            {
                EditorGUILayout.EndVertical();
                EditorGUILayout.BeginVertical("HelpBox", GUILayout.Height(250));
                {
                    mScrollViewCommon = EditorGUILayout.BeginScrollView(mScrollViewCommon);
                    for (int i = 0; i < mCates.Count; i++)
                    {
                        var tPathKey = mCates[i];
                        if (mDicTxts.ContainsKey(tPathKey) == false) continue;
                        var tValueContent = mDicTxts[tPathKey];
                        var tName = tPathKey.Split('/')[tPathKey.Split('/').Length - 1];
                        if (GUILayout.Button("-点击给引脚本加上宏-," + tName, GUILayout.Height(20)))
                        {
                            var tLines = tValueContent.Split(new string[] { "\r\n" }, StringSplitOptions.None);
                            var sb = new StringBuilder();
                            for (int j = 0;j < tLines.Length; j++)
                            {
                                var tLine = tLines[j];
                                if (tLine.Contains("Debug"))
                                {
                                    sb.AppendLine("#if EDITOR_LOG");
                                    sb.AppendLine(tLine);
                                    sb.AppendLine("#endif");
                                }
                                else
                                {
                                    sb.AppendLine(tLine);
                                }
                            }
                            StreamWriter tReader = new StreamWriter(tPathKey, false);
                            tReader.Write(sb.ToString());
                            tReader.Close();
                            AssetDatabase.SaveAssets();
                            mCates.Remove(tPathKey);
                            ShowNotifyLabel("执行了此脚本");
                        }
                    }
                    EditorGUILayout.EndScrollView();
                }
                if (GUILayout.Button("若太多,一口气此模块全部加宏", GUILayout.Height(27)))
                {
                    for (int i = 0; i < mCates.Count; i++)
                    { 
                        var tPathKey = mCates[i];
                        if(mDicTxts.ContainsKey(tPathKey)==false)continue;
                        var tValueContent = mDicTxts[tPathKey];
                        var tLines = tValueContent.Split(new string[] { "\r\n" }, StringSplitOptions.None);
                        var sb = new StringBuilder();
                        for (int j = 0; j < tLines.Length; j++)
                        {
                            var tLine = tLines[j];
                            if (tLine.Contains("Debug"))
                            {
                                sb.AppendLine("#if EDITOR_LOG");
                                sb.AppendLine(tLine);
                                sb.AppendLine("#endif");
                            }
                            else
                            {
                                sb.AppendLine(tLine);
                            }
                            StreamWriter tReader = new StreamWriter(tPathKey, false);
                            tReader.Write(sb.ToString());
                            tReader.Close();
                            AssetDatabase.SaveAssets();
                            mDicTxts[tPathKey] = "";
                        }
                    }
                    ShowNotifyLabel("执行了此模块");
                }
            }
        }
    }
}

