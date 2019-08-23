using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSettingData : Singleton<PlayerSettingData> {

	public bool isMusic;
	public bool isSound;
    public bool isVibrate;

	public CONTROL_STYLE controlStyle;
	public GRAPHIC_QUALITY graphic;

	public PlayerSettingData () {
		isMusic = (PlayerPrefs.GetInt("music", 1) == 1);
		isSound = (PlayerPrefs.GetInt("sound", 1) == 1);
		controlStyle = (CONTROL_STYLE)PlayerPrefs.GetInt("control", 1);
		graphic = (GRAPHIC_QUALITY)PlayerPrefs.GetInt("graphic", 1);
	}

	public void Save () {
		PlayerPrefs.SetInt("music", isMusic ? 1 : 0);
		PlayerPrefs.SetInt("sound", isSound ? 1 : 0);
		PlayerPrefs.SetInt("control", (int)controlStyle);
		PlayerPrefs.SetInt("graphic", (int)graphic);
	}
}

public enum GRAPHIC_QUALITY {
	LOW,
	HIGH
}