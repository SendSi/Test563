using System;
using UnityEngine;
using System.Collections;
using UnityEditor;

public class ColorToolEditorWindow : BaseEditorWindow
{
    [MenuItem("Tools/RGB")]
    public static void AddWindow()
    {
        // Rect wr=new Rect(0,0,500,500);
        Open<ColorToolEditorWindow>();
    }

    private string RGBText;
    private string OxText;
    private string text;
    public Color centerColor;
    private Color[] colors;

    private void Start()
    {
        colors = new Color[1];
        colors[0] = centerColor;
    }

    private Color matColor = Color.white;

    protected override void CustomOnGUI()
    {
        RGBText = EditorGUILayout.TextField("输入#16进制颜色值/调色板", RGBText);
        if (GUILayout.Button("ToRGB", GUILayout.Width(200)))
        {
            if (string.IsNullOrEmpty(RGBText)) return;

            string str = RGBText.Substring(1);
            if (str.Substring(4).Length < 2)
            {
                str = "0x" + str.Substring(0, 4) + "0" + str.Substring(4);
            }
            int c = Convert.ToInt32(str, 16);
            //string str1 = string.Format("{0},{1},{2}", (Convert.ToInt32(c & 0xff0000) >> 16), (Convert.ToInt32(c & 0x00ff00) >> 8), Convert.ToInt32(c & 0x0000ff));
            matColor.r = ((Convert.ToInt32(c & 0xff0000) >> 16) / 255.0f);
            matColor.g = ((Convert.ToInt32(c & 0x00ff00) >> 8) / 255.0f);
            matColor.b = ((Convert.ToInt32(c & 0x0000ff)) / 255.0f);
        }
        int r1 = (int)Math.Round(255 * matColor.r);
        int g1 = (int)Math.Round(255 * matColor.g);
        int b1 = (int)Math.Round(255 * matColor.b);

        string r = r1.ToString("X");
        if (r.Length < 2)
            r = "0" + r;
        string g = g1.ToString("X");
        if (g.Length < 2)
            g = "0" + g;
        string b = b1.ToString("X");
        if (b.Length < 2)
            b = "0" + b;

        matColor = EditorGUI.ColorField(new Rect(0, 150, 250, 25), "Color:", matColor);
        text = r1.ToString() + "," + g1.ToString() + "," + b1.ToString();
        text = EditorGUILayout.TextField("输出RGB", text);
        OxText = "#" + r + g + b;
        OxText = EditorGUILayout.TextField("输出16进制", OxText);
    }

    /// <summary>
    /// 实现数据的四舍五入未能
    /// </summary>
    private double Round(double v, int x)
    {
        bool isNegative = false;
        if (v < 0)
        {
            isNegative = true;
            v = -v;
        }
        int iValue = 1;
        for (int i = 1; i <= x; i++)
        {
            iValue = iValue * 10;
        }
        double ints = Math.Round(v * iValue + 0.5, 0);
        v = ints / iValue;
        if (isNegative)
        {
            v = -v;
        }

        return v;
    }
}
