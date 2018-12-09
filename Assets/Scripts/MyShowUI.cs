using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyShowUI : MonoBehaviour
{

    public UILabel mLabel;

    public void SetLabelTxt(string str)
    {
        if (mLabel == null)
            mLabel = GameObject.Find("UI Root/Camera/Label").GetComponent<UILabel>();
        mLabel.text = str;
    }

    void OnGUI()
    {
        if (GUILayout.Button("SetLabelTxt"))
        {
            SetLabelTxt("测试时,还要去写OnGUI 多麻烦呀,要编译,用完还要删掉");
        }
    }

}
