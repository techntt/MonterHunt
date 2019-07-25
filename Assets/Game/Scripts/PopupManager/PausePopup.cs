using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;
using ABIPlugins;

public class PausePopup : SingletonPopup<PausePopup> {

	public Image settingBtn;
	public Image questBtn;
	public SettingManager settingTab;
	public QuestPopupData questTab;
	public Color selectColor, unselectColor;
	bool isQuest;

	public void Show () {
		base.Show();
		if (settingTab.gameObject.activeSelf) {
			settingTab.Show();
			isQuest = false;
		}
		if (questTab.gameObject.activeSelf) {
			questTab.PreOpen();
			isQuest = true;
		}
		Time.timeScale = 0;
	}

	public override void Hide () {
		SoundManager.Instance.PlayUIButtonClick();
		base.Hide();
		PlayerSettingData.Instance.Save();
		Time.timeScale = 1;
	}

	public void OnSetting () {
		SoundManager.Instance.PlayUIButtonClick();
		GlobalEventManager.Instance.OnButtonPressed(PopupManager.Instance.scene.ToString(), "pause_setting");
		if (isQuest) {
			isQuest = false;
			questTab.Close();
			questTab.gameObject.SetActive(false);
			questBtn.DOColor(unselectColor, 0.2f).SetUpdate(true);
			questBtn.rectTransform.DOLocalMoveY(180, 0.2f).SetUpdate(true);
			settingTab.Show();
			settingTab.gameObject.SetActive(true);
			settingBtn.DOColor(selectColor, 0.2f).SetUpdate(true);
			settingBtn.rectTransform.DOLocalMoveY(200, 0.2f).SetUpdate(true);
		}
	}

	public void OnQuest () {
		SoundManager.Instance.PlayUIButtonClick();
		GlobalEventManager.Instance.OnButtonPressed(PopupManager.Instance.scene.ToString(), "pause_quest");
		if (!isQuest) {
			isQuest = true;
			settingTab.Hide();
			settingTab.gameObject.SetActive(false);
			settingBtn.DOColor(unselectColor, 0.2f).SetUpdate(true);
			settingBtn.rectTransform.DOLocalMoveY(180, 0.2f).SetUpdate(true);
			questTab.PreOpen();
			questTab.gameObject.SetActive(true);
			questBtn.DOColor(selectColor, 0.2f).SetUpdate(true);
			questBtn.rectTransform.DOLocalMoveY(200, 0.2f).SetUpdate(true);
		}
	}

	public void OnHome () {
		Hide();
		GlobalEventManager.Instance.OnButtonPressed(PopupManager.Instance.scene.ToString(), "pause_home");
		SceneManager.LoadScene("Home");

	}
}