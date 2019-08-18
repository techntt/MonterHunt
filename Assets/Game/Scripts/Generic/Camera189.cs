using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera189 : SingletonMonoBehaviour<Camera189> {

	public Camera cameraGameplay;

	float defaultOrthoSize = 5;
	float referHeight = 960;
	float referWidth = 640;
	public static Rect gameView;

	void Reset () {
		cameraGameplay = (Camera)GetComponent(typeof(Camera));
	}

	void Awake () {
        Init();
    }
    public void Init()
    {
        float unitWidth = referWidth * defaultOrthoSize / referHeight;
        float ratio = (float)Screen.height / (float)Screen.width;
        cameraGameplay.orthographicSize = ratio * unitWidth;
        gameView.xMin = -unitWidth;
        gameView.xMax = unitWidth;
        gameView.yMin = -cameraGameplay.orthographicSize;
        gameView.yMax = cameraGameplay.orthographicSize;
        Debug.Log("Ratio: " + ratio);
        Debug.Log("Y: [" + gameView.yMin + ", " + gameView.yMax + "]");
        Debug.Log("X: [" + gameView.xMin + ", " + gameView.xMax + "]");
    }
}
