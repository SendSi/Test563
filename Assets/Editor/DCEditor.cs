using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class DCEditor : EditorWindow
{
    [MenuItem("Tools/废弃图集的引用")]
    public static void ShowWindowDC()
    {
        EditorWindow.GetWindow<CommonUIWindow>(false);
    }
}

public class CommonUIWindow : EditorWindow
{
    private string mModuleStr;

    void OnGUI()
    {
        EditorGUILayout.EndVertical();
        EditorGUILayout.BeginVertical("Box");
        {
            mModuleStr = EditorGUILayout.TextField("模块路径:", mModuleStr);
        }
        ShowCommon();
        ShowCheckLabel();
    }

    private Dictionary<GameObject, List<UILabel>> mDicPrefabLabels = new Dictionary<GameObject, List<UILabel>>();
    private Vector2 mScrollViewLabel;
    private GameObject mHieraPrefab;
    private void ShowCheckLabel()
    {
        EditorGUILayout.EndVertical();
        EditorGUILayout.BeginVertical("Box");
        {
            GUILayout.Label("UILabel的引用,请保持10~15的深度(Depth),默认路径(Assets/UI/Prefabs)");
            if (GUILayout.Button("查看引用者", GUILayout.Height(27)))
            {
                mDicPrefabLabels.Clear();
                mHieraPrefab = null;
                var tAllPrefabs = GetPrefabs(mModuleStr);
                for (int i = 0; i < tAllPrefabs.Count; i++)
                {
                    var tPrefab = tAllPrefabs[i];
                    var tLabels = tPrefab.GetComponentsInChildren<UILabel>(true);
                    var tHasLabels = new List<UILabel>();
                    for (int j = 0; j < tLabels.Length; j++)
                    {
                        var tLabel = tLabels[j];
                        if (tLabel.bitmapFont != null)
                        {
                            if (tLabel.depth > 15 || tLabel.depth < 10)
                            {
                                if (tHasLabels.Contains(tLabel) == false)
                                    tHasLabels.Add(tLabel);
                            }
                        }
                    }
                    if (tHasLabels.Count > 0)
                        mDicPrefabLabels[tPrefab] = tHasLabels;
                }
            }

            if (mDicPrefabLabels.Count > 0)
            {
                EditorGUILayout.EndVertical();
                EditorGUILayout.BeginVertical("HelpBox", GUILayout.Height(250));
                {
                    mScrollViewLabel = EditorGUILayout.BeginScrollView(mScrollViewLabel);
                    var tRootHieView = GameObject.Find("UI Root/Camera");
                    foreach (var item in mDicPrefabLabels)
                    {
                        var tPrefab = item.Key;
                        var tLabels = item.Value;
                        for (int i = 0; i < tLabels.Count; i++)
                        {
                            var tLabel = tLabels[i];
                            if (GUILayout.Button(tPrefab.name + " , " + tLabel.gameObject.name, GUILayout.Height(20)))
                            {
                                var tHierarchyGo = GameObject.Find(tPrefab.name);
                                if (tHierarchyGo == null)
                                    tHierarchyGo = PrefabUtility.InstantiatePrefab(tPrefab) as GameObject;
                                tHierarchyGo.transform.parent = tRootHieView.transform;
                                tHierarchyGo.transform.localScale = new Vector3(1, 1, 1);

                                EditorGUIUtility.PingObject(tPrefab);
                                var tHierarchyLabels = tHierarchyGo.GetComponentsInChildren<UILabel>(true);
                                for (int j = 0; j < tHierarchyLabels.Length; j++)
                                {
                                    var tHierarch = tHierarchyLabels[j];
                                    if (tLabel.gameObject.name == tHierarch.gameObject.name &&
                                        tLabel.depth == tHierarch.depth && tLabel.text == tHierarch.text &&
                                        tLabel.transform.localPosition == tHierarch.transform.localPosition)
                                    {
                                        Selection.activeGameObject = tHierarchyLabels[j].gameObject;
                                        mHieraPrefab = tPrefab;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    EditorGUILayout.EndScrollView();
                }

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal("Box");
                {
                    if (GUILayout.Button(mHieraPrefab==null?"空":"(此Prefab="+mHieraPrefab.name+"不为(10~15)的全设为15)", GUILayout.Height(27)))
                    {
                        if(mHieraPrefab==null)return;
                        var tRootHiePrefab = GameObject.Find("UI Root/Camera/"+mHieraPrefab.name);
                        if(tRootHiePrefab==null)return;

                        var tLabels = mHieraPrefab.GetComponentsInChildren<UILabel>();
                        foreach (var item in tLabels)
                        {
                            if (item.bitmapFont != null && (item.depth < 10 || item.depth > 15))
                            {
                                item.depth = 15;
                            }
                        }
                        SaveNewPrefabs(new List<GameObject>(){mHieraPrefab});
                        mDicPrefabLabels.Remove(mHieraPrefab);
                    }
                    if (GUILayout.Button("此模块的不为(10~15)的全设为15.", GUILayout.Height(27)))
                    {
                        foreach (var item in mDicPrefabLabels)
                        {
                            var tPefab = item.Key;
                            var tLabels = item.Value;
                            for(int i=0;i<tLabels.Count;i++)
                            {
                                var tLabel = tLabels[i];
                                if (tLabel.bitmapFont != null && (tLabel.depth < 10 || tLabel.depth > 15))
                                {
                                    tLabel.depth = 15;
                                }
                            }
                        }
                        ShowNotifyLabel("此模块所有Prefab全部设置(不在10~15)为15啦,共" + mDicPrefabLabels.Count + "个Prefab");
                        SaveNewPrefabs(mDicPrefabLabels.Keys.ToList());
                        mDicPrefabLabels.Clear();
                    }
                }
            }
        }
    }


    #region CommonUI
    private Dictionary<GameObject, List<UISprite>> mDicPrefabCommons = new Dictionary<GameObject, List<UISprite>>();
    private Vector2 mScrollViewCommon;
    private string mNewAtlas, mNewSprite;
    private void ShowCommon()
    {
        EditorGUILayout.EndVertical();
        EditorGUILayout.BeginVertical("Box");
        {
            GUILayout.Label("废弃图集CommonUIAtlas的引用,默认路径(Assets/UI/Prefabs)");
            if (GUILayout.Button("查看引用者", GUILayout.Height(27)))
            {
                mDicPrefabCommons.Clear();
                var tAllPrefabs = GetPrefabs(mModuleStr);
                for (int i = 0; i < tAllPrefabs.Count; i++)
                {
                    var tPrefab = tAllPrefabs[i];
                    var tSprites = tPrefab.GetComponentsInChildren<UISprite>(true);

                    List<UISprite> tHasSprites = new List<UISprite>();
                    for (int j = 0; j < tSprites.Length; j++)
                    {
                        var tSprite = tSprites[j];
                        if (tSprite.atlas != null && tSprite.atlas.name.Contains("CommonUIAtlas"))
                        {
                            if (tHasSprites.Contains(tSprite) == false)
                                tHasSprites.Add(tSprite);
                        }
                    }

                    if (tHasSprites.Count > 0)
                        mDicPrefabCommons[tPrefab] = tHasSprites;
                }
            }
            if (mDicPrefabCommons.Count > 0)
            {
                EditorGUILayout.EndVertical();
                EditorGUILayout.BeginVertical("HelpBox", GUILayout.Height(250));
                {
                    mScrollViewCommon = EditorGUILayout.BeginScrollView(mScrollViewCommon);
                    var tRootHieView = GameObject.Find("UI Root/Camera");
                    foreach (var item in mDicPrefabCommons)
                    {
                        var tPrefab = item.Key;
                        var tSprites = item.Value;
                        for (int i = 0; i < tSprites.Count; i++)
                        {
                            var tSprite = tSprites[i];
                            if (GUILayout.Button(tPrefab.name + " , " + tSprite.gameObject.name, GUILayout.Height(20)))
                            {
                                var tHierarchyGo = GameObject.Find(tPrefab.name);
                                if (tHierarchyGo == null)
                                    tHierarchyGo = PrefabUtility.InstantiatePrefab(tPrefab) as GameObject;
                                tHierarchyGo.transform.parent = tRootHieView.transform;
                                tHierarchyGo.transform.localScale = new Vector3(1, 1, 1);

                                EditorGUIUtility.PingObject(tPrefab);
                                var tHierarchySprites = tHierarchyGo.GetComponentsInChildren<UISprite>(true);
                                for (int j = 0; j < tHierarchySprites.Length; j++)
                                {
                                    var tHierarch = tHierarchySprites[j];
                                    if (tSprite.atlas == tHierarch.atlas &&
                                        tSprite.spriteName == tHierarch.spriteName &&
                                        tSprite.transform.localPosition == tHierarch.transform.localPosition)
                                    {
                                        Selection.activeGameObject = tHierarchySprites[j].gameObject;
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    EditorGUILayout.EndScrollView();
                }
                mNewAtlas = EditorGUILayout.TextField("新的图集:", mNewAtlas);
                mNewSprite = EditorGUILayout.TextField("新的碎图:", mNewSprite);
                if (GUILayout.Button("若太多,一口气改成某一张碎图 慎用", GUILayout.Height(27)))
                {
                    if (string.IsNullOrEmpty(mNewAtlas) || string.IsNullOrEmpty(mNewSprite))
                    {
                        Debug.LogError("图集或碎图为空");
                        return;
                    }

                    var tNewAtlasGo = FindOneGo("Prefab", mNewAtlas);
                    var tAtlas = tNewAtlasGo.GetComponent<UIAtlas>();
                    var tPrefabs = new List<GameObject>();
                    foreach (var item in mDicPrefabCommons)
                    {
                        if (tPrefabs.Contains(item.Key) == false)
                            tPrefabs.Add(item.Key);
                        var tSprites = item.Value;
                        for (int i = 0; i < tSprites.Count; i++)
                        {
                            tSprites[i].atlas = tAtlas;
                            tSprites[i].spriteName = mNewSprite;
                        }
                    }
                    ShowNotifyLabel("此模块所有Prefab全部设置一张碎图(原使用了废弃的图集),共" + mDicPrefabCommons.Count + "个Prefab");
                    SaveNewPrefabs(tPrefabs);
                    mDicPrefabCommons.Clear();
                }
            }
        }
    }
    #endregion



    void ShowNotifyLabel(string label)
    {
        this.ShowNotification(new GUIContent(label));
    }

    private List<GameObject> GetPrefabs(string pPath)
    {
        if (string.IsNullOrEmpty(pPath)) pPath = "Assets/UI/Prefabs";
        var guids = AssetDatabase.FindAssets("t:Prefab", new string[] { pPath });
        List<GameObject> tAllPrefab = new List<GameObject>();
        foreach (var item in guids)
        {
            var go = AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GUIDToAssetPath(item)) as GameObject;
            tAllPrefab.Add(go);
        }
        return tAllPrefab;
    }

    /// <summary> 也就是Inspector面板 的 Apply按钮 </summary>
    private void SaveNewPrefabs(List<GameObject> pWillChanges)
    {
        if (pWillChanges == null || pWillChanges.Count == 0) return;
        for (int i = 0; i < pWillChanges.Count; i++)
        {
            var tPrefab = pWillChanges[i];
            var tOldHierarchyPrefab = PrefabUtility.InstantiatePrefab(tPrefab) as GameObject;
            var tPath = AssetDatabase.GetAssetPath(tPrefab);
            var tNewEmpty = PrefabUtility.CreateEmptyPrefab(tPath);
            PrefabUtility.ReplacePrefab(tOldHierarchyPrefab, tNewEmpty, ReplacePrefabOptions.ConnectToPrefab);
            GameObject.DestroyImmediate(tOldHierarchyPrefab);
        }
    }

    /// <summary>得到prefab           pFilter:Pefab或Script                pGoStr:prefabName </summary>
    private GameObject FindOneGo(string pFilter, string pGoStr)
    {
        var tGuids = AssetDatabase.FindAssets("t:" + pFilter + " " + pGoStr, new string[] { "Assets/UI" });
        GameObject tGo = null;
        foreach (var guid in tGuids)
        {
            tGo = AssetDatabase.LoadMainAssetAtPath(AssetDatabase.GUIDToAssetPath(guid)) as GameObject;
        }
        return tGo;
    }
}
