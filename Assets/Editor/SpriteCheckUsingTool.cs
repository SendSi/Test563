
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UISpriteType = UIBasicSprite.Type;

public class SpriteCheckUsingTool : EditorWindow
{
    [MenuItem("Tools/图集与碎图工具")]
    public static void OpenCheckUsing()
    {
        EditorWindow.GetWindow<SpriteCheckUsing>(true, "图集与碎图工具(只检测prefab,检测不了代码中的引用哦)").Show();
    }
}

public class SpriteCheckUsing : EditorWindow
{
    private string mAtlas, mSprite;
    private UISpriteType mSpriteType;
    private string mCheckAtlas;
    private bool mIsSelectDelete;
    private string mNewAltas, mNewSprite, mOldAtlas, mOldSprite;

    private void OnGUI()
    {
        EditorGUILayout.BeginVertical("Box");
        {
            GUILayout.Label("此批处理器作用:美术出图时没考虑使用9宫格省资源,但很多prefab已使用着不是\n9宫的碎图,这时美术重新出了新的用9宫碎图,一个个替换容易出漏,故写此工具");
            mAtlas = EditorGUILayout.TextField("图集名:", mAtlas);
            mSprite = EditorGUILayout.TextField("碎图名:", mSprite);
            mSpriteType = (UISpriteType)EditorGUILayout.EnumPopup("UISprite的Type属性", mSpriteType);
            if (GUILayout.Button("检测碎图有无被prefab使用过(看输出log)", GUILayout.Height(30)))
            {
                Debug.Log(mAtlas + mSprite + mSpriteType);
                var tPrefabs = GetFindAllUsing(mAtlas, mSprite, false, mSpriteType, null, "", true);
                if (tPrefabs.Count <= 0)
                {
                    Debug.Log("并没有使用过该碎图,图集名=" + mAtlas + ",碎图名=" + mSprite); return;
                }
                Debug.Log("总共有" + tPrefabs.Count + "个prefab使用着");
            }
            if (GUILayout.Button("使所有prefab的碎图全使用" + mSpriteType.ToString(), GUILayout.Height(30)))
            {
                var tPrefabs = GetFindAllUsing(mAtlas, mSprite, false, mSpriteType, null, "", false);
                if (tPrefabs.Count == 0)
                    Debug.Log("并没有使用过该碎图");
                ApplySavePrefab(tPrefabs);
            }
        }

        EditorGUILayout.EndVertical(); EditorGUILayout.Space(); EditorGUILayout.Space(); EditorGUILayout.Space();

        EditorGUILayout.BeginVertical("Box");
        {
            GUILayout.Label("检测图集里面有无被prefab引用.\neg:Common图集太大了,有些碎图根本就没被使用过,然而还打在图集里,浪费空间");
            mCheckAtlas = EditorGUILayout.TextField("图集名:", mCheckAtlas);
            mIsSelectDelete = EditorGUILayout.Toggle("是否无引用碎图删除", mIsSelectDelete);
            if (GUILayout.Button("检测" + (mIsSelectDelete ? "时并删除无引用的碎图" : "无引用的碎图"), GUILayout.Height(30)))
            {
                var tTextures = new List<Texture>();
                var tPath = AssetDatabase.FindAssets("t:Texture", new string[] { "Assets/UI/Atlas/" + mCheckAtlas + "/imgs/" });
                for (int i = 0; i < tPath.Length; i++)
                {
                    tTextures.Add(AssetDatabase.LoadMainAssetAtPath(AssetDatabase.AssetPathToGUID(tPath[i])) as Texture);
                }
                int tCount = 0;
                for (int i = 0; i < tTextures.Count; i++)
                {
                    var tItem = tTextures[i];
                    if (GetFindAllUsing(mCheckAtlas, tItem.name, false, mSpriteType).Count <= 0)
                    {
                        tCount++;
                        Debug.Log("并没使用过该碎图,所在的图集名=" + mCheckAtlas + ",碎图名=" + tItem.name + ",count=" + tCount);
                        if (mIsSelectDelete)
                            File.Delete(Application.dataPath + "/UI/Atlas/" + mCheckAtlas + "/imgs/" + tItem.name + ".png");
                    }
                }
                Debug.Log("原图集有" + tTextures.Count + "张碎图,无引用的有" + tCount + "张");
                if (tCount > 0 && mIsSelectDelete)
                    Debug.LogError("删除了" + tCount + "张碎图,现还有" + (tTextures.Count - tCount) + "张碎图,请打开TP工具重新生成下吧,并应用下吧");
            }
        }

        EditorGUILayout.EndVertical(); EditorGUILayout.Space(); EditorGUILayout.Space(); EditorGUILayout.Space();

        EditorGUILayout.BeginVertical("Box");
        {
            GUILayout.Label("eg:工程中有三个Common图集 都有同一个资源,为省空间面打算\n不使用另两个图集,则去检测图集并把prefab的引用都指定同一个图集");
            mOldAtlas = EditorGUILayout.TextField("原图集名:", mOldAtlas);
            mOldSprite = EditorGUILayout.TextField("原碎图名:", mOldSprite);
            mNewAltas = EditorGUILayout.TextField("将使用的图集名:", mNewAltas);
            mNewSprite = EditorGUILayout.TextField("将使用的碎图名:", mNewSprite);
            if (GUILayout.Button("替换prefab到新图集", GUILayout.Height(30)))
            {
                var tFindOld = AssetDatabase.FindAssets("t:Prefab", new string[] { "Assets/UI/Atlas/" + mOldAtlas });
                var tFindNew = AssetDatabase.FindAssets("t:Prefab", new string[] { "Assets/UI/Atlas/" + mNewAltas });
                UIAtlas tNewAtlas = null, tOldAtlas = null;
                if (tFindNew != null && tFindNew.Length == 1)
                {
                    var newGo = AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GUIDToAssetPath(tFindNew[0])) as GameObject;
                    tNewAtlas = newGo.GetComponent<UIAtlas>();
                }
                if (tFindOld != null && tFindOld.Length == 1)
                {
                    var oldGo = AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GUIDToAssetPath(tFindOld[0])) as GameObject;
                    tOldAtlas = oldGo.GetComponent<UIAtlas>();
                }
                if (tNewAtlas == null || tOldAtlas == null || tOldAtlas == tNewAtlas)
                {
                    Debug.LogError("图集没加载出来,检测是否正确输入吧");
                    return;
                }
                var tPrefabs = GetFindAllUsing(mOldAtlas, mOldSprite, false, mSpriteType, tNewAtlas, mNewSprite);
                if (tPrefabs.Count <= 0)
                {
                    Debug.Log("旧图集并没有使用过该碎图,旧图集名=" + mOldAtlas + ",碎图名=" + mOldSprite); return;
                }
                ApplySavePrefab(tPrefabs);
            }
        }
        EditorGUILayout.EndVertical();
    }




    private void ApplySavePrefab(List<GameObject> pPrefabs)
    {
        for (int i = 0; i < pPrefabs.Count; i++)
        {
            var tPrefab = pPrefabs[i];
            var tInsaniateGo = PrefabUtility.InstantiatePrefab(tPrefab) as GameObject;//先在Inspector生成  再apply(apply生成一个新的,用inspector替换下)
            var tPath = AssetDatabase.GetAssetPath(tPrefab);
            var tNewPrefab = PrefabUtility.CreateEmptyPrefab(tPath);
            PrefabUtility.ReplacePrefab(tInsaniateGo, tNewPrefab, ReplacePrefabOptions.ConnectToPrefab);
            GameObject.DestroyImmediate(tInsaniateGo);
        }
        AssetDatabase.SaveAssets();
    }



    private List<GameObject> GetFindAllUsing(string pAltas, string pSprite, bool pChageType, UISpriteType pTargetType, UIAtlas pNewAltas = null, string pNewSprite = "", bool plog = false)
    {
        var tAllPrefab = new List<GameObject>();
        var tSearchPath = AssetDatabase.FindAssets("t:Prefab", new string[] { "Assets/UI/Prefab" });
        for (int i = 0; i < tSearchPath.Length; i++)
        {
            tAllPrefab.Add(AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GUIDToAssetPath(tSearchPath[i])) as GameObject);
        }
        var tWillChangePrefabs = new List<GameObject>();
        for (int i = 0; i < tAllPrefab.Count; i++)
        {
            var tPrefab = tAllPrefab[i];
            var tSpriteChilds = tPrefab.GetComponentsInChildren<UISprite>(true);
            for (int j = 0; j < tSpriteChilds.Length; j++)
            {
                var tSpriteChild = tSpriteChilds[i];
                if (tSpriteChild.atlas != null && tSpriteChild.atlas.name.Equals(pAltas) && tSpriteChild.spriteName.Contains(pSprite))
                {
                    if (plog)//输出
                        Debug.Log("输出,prefab=" + tPrefab + ",所在的挂点=" + tSpriteChild.name);
                    if (tWillChangePrefabs.Contains(tPrefab) == false)//检测得到的prefab总数
                        tWillChangePrefabs.Add(tPrefab);
                    if (pChageType && tSpriteChild.type != pTargetType)//改变spriteType
                        tSpriteChild.type = pTargetType;

                    if (pNewAltas != null && pNewAltas.name != pAltas)
                    {
                        tSpriteChild.atlas = pNewAltas;
                        if (string.IsNullOrEmpty(pNewSprite) == false)
                            tSpriteChild.spriteName = pNewSprite;
                    }
                }
            }
        }
        return tWillChangePrefabs;
    }
}

