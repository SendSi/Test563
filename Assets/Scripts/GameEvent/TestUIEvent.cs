using UnityEngine;
using System.Collections;

public class TestUIEvent : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        GameEventCenter.AddListener(EventNames.Back_Setup, OnSetUp);
        GameEventCenter.AddListener(EventNames.TestInt, OnTestInt);
        GameEventCenter.AddListener(EventNames.TestIntString, OnTestIntString);
    }

    private void OnTestIntString(int t,string s)
    {
        Debug.Log(t+s);
    }

    void OnDestory()
    {
        GameEventCenter.RemoveListener(EventNames.Back_Setup, OnSetUp);
        GameEventCenter.RemoveListener(EventNames.TestInt, OnTestInt);
        GameEventCenter.RemoveListener(EventNames.TestIntString, OnTestIntString);
    }

    private void OnTestInt(int t)
    {
        Debug.Log(t);
    }

    private void OnSetUp()
    {
        Debug.Log("ttt");
    }

    void OnGUI()
    {
        if (GUILayout.Button("--------"))
        {
            GameEventCenter.SendEvent(EventNames.Back_Setup);
        }
        if (GUILayout.Button("----int----"))
        {
            GameEventCenter.SendEvent(EventNames.TestInt,100);
        }
        if (GUILayout.Button("----int----"))
        {
            GameEventCenter.SendEvent(EventNames.TestIntString, 100,"oop");
        }
    }
}
