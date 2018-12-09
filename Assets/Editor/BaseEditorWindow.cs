using System;
using UnityEngine;
using UnityEditor;
using System.Collections;

public abstract class BaseEditorWindow : EditorWindow
{
    public const int NormalFontSize = 20;
    private static GUIStyle _leftButtonStyle;
    public static GUIStyle LeftButtonStyle
    {
        get
        {
            if (_leftButtonStyle == null)
            {
                _leftButtonStyle = new GUIStyle(GUI.skin.button);
                _leftButtonStyle.alignment = TextAnchor.MiddleLeft;
            }
            return _leftButtonStyle;
        }
    }

    protected Vector2 _scrollPos;
    protected virtual void OnGUI()
    {
        StartOnGUI();
        Space();
        CustomOnGUI();
        Space();
        EndOnGUI();
    }
    protected virtual void StartOnGUI()
    {
        _scrollPos = BeginScrollView(_scrollPos);
    }

    protected abstract void CustomOnGUI();
    protected virtual void EndOnGUI()
    {
        EditorGUILayout.EndScrollView();
    }
    protected virtual void Space()
    {
        EditorGUILayout.Space();
    }

    protected static  T Open<T>() where T : BaseEditorWindow
    {
        return GetWindow<T>(typeof(T).Name.Replace("EditorWindow",""));
    }

    protected  virtual Vector2 BeginScrollView(Vector2 pos,params  GUILayoutOption[] options)
    {
        return EditorGUILayout.BeginScrollView(pos, options);
    }

    protected virtual T EnumPopup<T>(string title, Enum selectEnum, GUIStyle style = null)
    {
        style = style ?? EditorStyles.popup;
        //Convert.ChangeType(object,typeof())
        return (T)Convert.ChangeType(EditorGUILayout.EnumPopup(title, selectEnum, style),typeof(T));
    }

    protected virtual void Labe()
    {
        //EditorGUILayout.LabelField();
    }
}
