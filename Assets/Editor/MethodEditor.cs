using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class MethodEditor : EditorWindow
{
    [MenuItem("Tools/反射")]
    public static void ShowAsset()
    {
        EditorWindow.GetWindow<MethodEditor>(false);
    }

    private string mClassName;
    private string mMethodName;
    private string mParmas;

    void TestOne()
    {
        EditorGUILayout.EndVertical();
        EditorGUILayout.BeginVertical("Box");
        {
            mClassName = EditorGUILayout.TextField("类名:", mClassName);
            mMethodName = EditorGUILayout.TextField("方法名:", mMethodName);
            mParmas = EditorGUILayout.TextField("参数", mParmas);
            EditorGUILayout.LabelField("参数-->:(值:类型,值:类型...)(wzx:string,4:int)");
            if (GUILayout.Button("执行", GUILayout.Height(30)))
            {
                Debug.Log("此程序集=" + Assembly.GetExecutingAssembly().FullName);
                if (string.IsNullOrEmpty(mClassName) || string.IsNullOrEmpty(mMethodName)) return;
                try
                {
                    var tAsb = Assembly.Load("Assembly-CSharp");
                    var tType = tAsb.GetType(mClassName);
                    var tMethod = tType.GetMethod(mMethodName);
                    var tObj = Activator.CreateInstance(tType);
                    var par = tMethod.GetParameters();
                    foreach (var item in par)
                    {
                        Debug.Log(item);
                    }
                    //if (string.IsNullOrEmpty(mParmas))
                    //    tMethod.Invoke(tObj, null);
                    //else
                    //{
                    //    var tGroups = mParmas.Split(',');
                    //    object[] tPars = new object[tGroups.Length];
                    //    for (int i = 0; i < tGroups.Length; i++)
                    //    {
                    //        var tValueType = tGroups[i].Split(':')[1];
                    //        if (tValueType.Contains("string"))
                    //        {
                    //            tPars[i] = tGroups[i].Split(':')[0];
                    //        }
                    //        else if (tValueType.Contains("int"))
                    //        {
                    //            tPars[i] = int.Parse(tGroups[i].Split(':')[0]);
                    //        }
                    //    }
                    //    tMethod.Invoke(tObj, tPars);
                    //    tMethod.Invoke(tObj, new object[] {int.Parse(mParmas),"string类型"});
                    //}
                }
                catch (Exception e)
                {
                    Debug.LogError("异常了--查看类名.方法.参数");
                }
            }
        }
    }
    void OnGUI()
    {
        TestOne();
        //TestTwo();
    }



    private List<string> mTempName = new List<string>();
    private List<string> mTempValue = new List<string>();
    private string mNotifyName;
    private string mDto;
    void TestTwo()
    {
        EditorGUILayout.EndVertical();
        EditorGUILayout.BeginVertical("Box");
        {
            mNotifyName = EditorGUILayout.TextField("消息名:", mNotifyName);
            mDto = EditorGUILayout.TextField("dto:", mDto);
            if (GUILayout.Button("执行", GUILayout.Height(30)))
            {
                try
                {
                    mTempName.Clear(); mTempValue.Clear();
                    var tAsb = Assembly.Load("Assembly-CSharp");
                    var tType = tAsb.GetType(mDto);
                    var tFields = tType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public);
                    for (int i = 0; i < tFields.Length; i++)
                    {
                        var tField = tFields[i];
                        mTempName.Add(tField.Name);
                        mTempValue.Add("");
                    }
                    Debug.Log("");
                }
                catch (Exception e)
                {
                    for (int i = 0; i < mTempName.Count; i++)
                    {
                        mTempValue[i] = EditorGUILayout.TextField(mTempName[i], mTempValue[i]);
                    }
                    Debug.LogError("异常了");
                }
            }

            if (mTempValue.Count > 0)
            {
                for (int i = 0; i < mTempName.Count; i++)
                {
                    mTempValue[i] = EditorGUILayout.TextField(mTempName[i], mTempValue[i]);
                }
            }
        }
    }

}
