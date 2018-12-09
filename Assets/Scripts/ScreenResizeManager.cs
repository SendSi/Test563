using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class ScreenResizeManager
{
    public enum PhoneState
    {
        NONE = 0,
        DEFAULT = 1,
        IPHONEX = 2
    }

    public class EdgeInset
    {
        public int Left;
        public int Bottom;
        public int Right;
        public int Top;

        public int Width;
        public int Height;

        public EdgeInset(int l, int b, int r, int t)
        {
            Left = l;
            Right = r;
            Bottom = b;
            Top = t;
        }
        public EdgeInset(int l, int b, int r, int t, int w, int h)
        {
            Left = l;
            Right = r;
            Bottom = b;
            Top = t;
            Height = h;
            Width = w;
        }
    }

    public class Rect
    {
        public float X;
        public float Y;
        public float Width;
        public float Height;

        public Rect()
        {
        }

        public Rect(float x, float y, float width, float height)
        {
            X = x;
            Y = y;
            Width = width;
           Height = height;
        }
    }


    private static ScreenResizeManager instance;

    public static ScreenResizeManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new ScreenResizeManager();
                instance.Init();
            }
            return instance;
        }
    }

    PhoneState _curPhoneState = PhoneState.NONE;
    ScreenOrientation _curOrientation = ScreenOrientation.LandscapeLeft;
    private Dictionary<PhoneState, EdgeInset> _phoneEdgeInsetDic;
    private Rect _leftRect;
    private Rect _rightRect;
    public event Action OnOrientationChanged;

    private void Init()
    {
        _phoneEdgeInsetDic = new Dictionary<PhoneState, EdgeInset>()
        {
            {PhoneState.IPHONEX, new EdgeInset(132,0,0,0,2436,1125)}
        };
        _curPhoneState = GetCurPhone();
        //_curOrientation = GetScreenOrientation();
        UpdateOrientation();

        _leftRect = CalculateNguiRect();
        _rightRect = _leftRect != null
            ? new Rect(-_leftRect.X, -_leftRect.Y, _leftRect.Width, _leftRect.Height)
            : null;
    }

    private void UpdateOrientation()
    {
        var lastOri = _curOrientation;
        _curOrientation = GetScreenOritationRuntime();
        if (lastOri != _curOrientation && lastOri != ScreenOrientation.Unknown)
        {
            OrientationChanging();
        }
    }

    private void OrientationChanging()
    {
        if (OnOrientationChanged != null)
        {
            OnOrientationChanged();
        }
    }

    public Rect GetRect()
    {
        if (_leftRect == null || _rightRect == null)
        {
            return null;
        }
        if (_curOrientation == ScreenOrientation.LandscapeRight)
        {
            return _rightRect;
        }
        else
        {
            return _leftRect;
        }
    }

    public bool IsNeedResize()
    {
        return GetRect() != null;
    }

    private string GetDeviceModel()
    {
        if (Application.isEditor && IsEditorOrientation())
        {
            switch (GetPhoneState())
            {
                case PhoneState.IPHONEX:
                    return "iPhone10,3";
                    break;
            }
        }
        return SystemInfo.deviceModel;
    }



    private PhoneState GetCurPhone()
    {
        var dev = GetDeviceModel();
        if (dev.Contains("iPhone10,3") || dev.Contains("iPhone10,6"))
        {
            return PhoneState.IPHONEX;
        }
        return PhoneState.DEFAULT;
    }

    private EdgeInset GetPhoneEdgeInset()
    {
        EdgeInset _inset = null;
        _phoneEdgeInsetDic.TryGetValue(_curPhoneState, out _inset);
        return _inset;
    }

    private Rect CalculateNguiRect()
    {
        var edgeInset = GetPhoneEdgeInset();
        if (edgeInset == null) return null;

        var root = GameObject.Find("UI Root").GetComponent<UIRoot>();
        var pixelSizeAdjustment = root.pixelSizeAdjustment;//tothink
        var screen = NGUITools.screenSize;
        var widthScale = 1f;
        var heightScale = 1f;
#if UNITY_EDITOR
        widthScale = screen.x / edgeInset.Width;
        heightScale = screen.y / edgeInset.Height;
#endif
        var width = screen.x- (edgeInset.Left - edgeInset.Right) * widthScale;
        var height = screen.y - (edgeInset.Top - edgeInset.Bottom) * heightScale;
        var rect = new Rect
        {
            Width = width * pixelSizeAdjustment,
            Height = height * pixelSizeAdjustment,
            X = (edgeInset.Left - edgeInset.Right) * pixelSizeAdjustment / 2 * widthScale,
            Y= (edgeInset.Bottom - edgeInset.Top) * pixelSizeAdjustment / 2 * heightScale,
        };
        return rect;
    }

    private ScreenOrientation GetScreenOritationRuntime()
    {
        if (Application.isEditor)
        {
            return GetScreenOrientation();
        }
        var orientation = Screen.orientation;
        if (orientation != ScreenOrientation.Landscape && orientation != ScreenOrientation.LandscapeLeft && orientation != ScreenOrientation.LandscapeRight)
        {
            switch (Input.deviceOrientation)
            {
                case DeviceOrientation.LandscapeLeft:
                    orientation = ScreenOrientation.LandscapeLeft;
                    break;
                case DeviceOrientation.LandscapeRight:
                    orientation = ScreenOrientation.LandscapeRight;
                    break;
            }
        }
        return orientation;
    }

    public void ResizePanel(GameObject go, bool clip = false)
    {
        var rect = GetRect();
        if (rect != null)
        {
            var panel = go.GetComponentInChildren<UIPanel>();
            if (panel != null)
            {
                panel.clipping = clip ? UIDrawCall.Clipping.SoftClip : UIDrawCall.Clipping.ConstrainButDontClip;
                panel.SetRect(rect.X, rect.Y, rect.Width, rect.Height);
                var rects = go.GetComponentsInChildren<UIRect>(true);
                var count = rects.Length;
                for (int i = 0; i < count; i++)
                {
                    rects[i].UpdateAnchors();
                }
            }
        }
    }






    //---------------------------测试使用--------------------------
    private const string ScreenOrientationKey = "ScreenOrientationKeyDev";
    private const string PhoneStateKey = "PhoneStateKeyDev";
    private static bool IsEditorOrientation()
    {
        var orientation = GetScreenOrientation();
        return orientation == ScreenOrientation.Landscape || orientation == ScreenOrientation.LandscapeLeft || orientation == ScreenOrientation.LandscapeRight;
    }


    public static ScreenOrientation GetScreenOrientation()
    {
        if (PlayerPrefs.HasKey(ScreenOrientationKey))
        {
            return (ScreenOrientation)PlayerPrefs.GetInt(ScreenOrientationKey);
        }
        return ScreenOrientation.Unknown;
    }

    public static PhoneState GetPhoneState()
    {
        if (PlayerPrefs.HasKey(PhoneStateKey))
        {
            return (PhoneState)PlayerPrefs.GetInt(PhoneStateKey);
        }
        return PhoneState.DEFAULT;
    }

    /// <summary> Iphone 型号 </summary> 
    public static void SetPhoneVersion(PhoneState version)
    {
        PlayerPrefs.SetInt(PhoneStateKey,(int)version);
    }
    /// <summary> Iphone 方向 </summary> 
    public static void SetPhoneOrientation(ScreenOrientation ori)
    {
        PlayerPrefs.SetInt(ScreenOrientationKey,(int)ori);
    }
}
