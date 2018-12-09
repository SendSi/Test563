using UnityEngine;
using System.Collections;
using UnityEditor;

public class ScreenResizeEditorWindow : BaseEditorWindow
{
    private ScreenOrientation _screenOrientation;
    private ScreenResizeManager.PhoneState _phoneState;


    private ScreenOrientation _cacheOrientation;
    private ScreenResizeManager.PhoneState _cachePhone;

    [MenuItem("Tools/ScreenResize")]
    private static void Open()
    {
        Open<ScreenResizeEditorWindow>();
    }

    private void Awake()
    {
        _screenOrientation = ScreenResizeManager.GetScreenOrientation();
        _phoneState = ScreenResizeManager.GetPhoneState();
    }
    protected override void CustomOnGUI()
    {
        _cachePhone = _phoneState;
        _phoneState = EnumPopup<ScreenResizeManager.PhoneState>("型号", _phoneState);
        if (_cachePhone != _phoneState)
        {
            ScreenResizeManager.SetPhoneVersion(_phoneState);
        }
        Space();
        _cacheOrientation = _screenOrientation;
        _screenOrientation = EnumPopup<ScreenOrientation>("朝向", _screenOrientation);
        if (_cacheOrientation != _screenOrientation)
        {
            ScreenResizeManager.SetPhoneOrientation(_screenOrientation);
        }
    }
}

