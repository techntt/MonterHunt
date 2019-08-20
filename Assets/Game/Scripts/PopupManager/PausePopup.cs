using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class PausePopup : BasePopup {    
	public SettingManager settingTab;

	public override void Show () {
        settingTab.Show();
        Time.timeScale = 0;
        base.Show();
	}

	public override void Hide () {
		SoundManager.Instance.PlayUIButtonClick();		
		PlayerSettingData.Instance.Save();
		Time.timeScale = 1;
        base.Hide();
	}
    
	public void OnHome () {
		Hide();
		GlobalEventManager.Instance.OnButtonPressed(PopupManager.Instance.scene.ToString(), "pause_home");
		SceneManager.LoadScene("Home");

	}
}