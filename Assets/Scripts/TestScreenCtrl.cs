using UnityEngine;
using System.Collections;

public class TestScreenCtrl : MonoBehaviour {

	
	void Start () {
        ResizeWindow();
        ScreenResizeManager.Instance.OnOrientationChanged += ResizeWindow;
	}

    private void ResizeWindow()
    {
        ScreenResizeManager.Instance.ResizePanel(this.gameObject);
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
