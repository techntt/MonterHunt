using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using ABIPlugins;

public class SettingManager : MonoBehaviour {

	public Image musicBtn;
	public Image soundBtn;
	public Image graphicBtn;
	public Text musicText;
	public Text soundText;
	public Text graphicText;
	public Image drag;
	public Image touch;
	public Color musicOn, musicOff, controlOn, controlOff;


	public void Show () {
		if (PlayerSettingData.Instance.isMusic) {
			musicBtn.rectTransform.localPosition = new Vector3(36, 0, 0);
			musicBtn.color = musicOn;
			musicText.text = "On";
		} else {
			musicBtn.rectTransform.localPosition = new Vector3(-36, 0, 0);
			musicBtn.color = musicOff;
			musicText.text = "Off";
		}
		if (PlayerSettingData.Instance.isSound) {
			soundBtn.rectTransform.localPosition = new Vector3(36, 0, 0);
			soundBtn.color = musicOn;
			soundText.text = "On";
		} else {
			soundBtn.rectTransform.localPosition = new Vector3(-36, 0, 0);
			soundBtn.color = musicOff;
			soundText.text = "Off";
		}
		if (graphicBtn) {
			if (PlayerSettingData.Instance.graphic == GRAPHIC_QUALITY.LOW) {
				graphicBtn.rectTransform.localPosition = new Vector3(-36, 0, 0);
				graphicBtn.color = musicOff;
				graphicText.text = "Low";
			} else {
				graphicBtn.rectTransform.localPosition = new Vector3(36, 0, 0);
				graphicBtn.color = musicOn;
				graphicText.text = "High";
			}
		}
		if (PlayerSettingData.Instance.controlStyle == CONTROL_STYLE.FIXED) {
			drag.color = controlOn;
			touch.color = controlOff;
		} else {
			drag.color = controlOff;
			touch.color = controlOn;
		}
	}

	public void Hide () {
		PlayerSettingData.Instance.Save();
	}

	public void Music () {
		PlayerSettingData.Instance.isMusic = !PlayerSettingData.Instance.isMusic;
		GlobalEventManager.Instance.OnButtonPressed(PopupManager.Instance.scene.ToString(), "setting_music");
		if (PlayerSettingData.Instance.isMusic) {
			musicBtn.transform.DOLocalMoveX(36, 0.2f).SetUpdate(true);
			musicBtn.DOColor(musicOn, 0.2f).SetUpdate(true);
			musicText.text = "On";
			SoundManager.Instance.MusicOn();
		} else {
			musicBtn.transform.DOLocalMoveX(-36, 0.2f).SetUpdate(true);
			musicBtn.DOColor(musicOff, 0.2f).SetUpdate(true);
			musicText.text = "Off";
			SoundManager.Instance.MusicOff();
		}
	}

	public void Sound () {
		PlayerSettingData.Instance.isSound = !PlayerSettingData.Instance.isSound;
		if (PlayerSettingData.Instance.isSound) {
			soundBtn.transform.DOLocalMoveX(36, 0.2f).SetUpdate(true);
			soundBtn.DOColor(musicOn, 0.2f).SetUpdate(true);
			soundText.text = "On";
			SoundManager.Instance.SoundOn();
		} else {
			soundBtn.transform.DOLocalMoveX(-36, 0.2f).SetUpdate(true);
			soundBtn.DOColor(musicOff, 0.2f).SetUpdate(true);
			soundText.text = "Off";
			SoundManager.Instance.SoundOff();
		}
		SoundManager.Instance.PlayUIButtonClick();
		GlobalEventManager.Instance.OnButtonPressed(PopupManager.Instance.scene.ToString(), "setting_sound");
	}

	public void Graphic () {
		if (PlayerSettingData.Instance.graphic == GRAPHIC_QUALITY.HIGH) {
			PlayerSettingData.Instance.graphic = GRAPHIC_QUALITY.LOW;
			graphicBtn.transform.DOLocalMoveX(-36, 0.2f).SetUpdate(true);
			graphicBtn.DOColor(musicOff, 0.2f).SetUpdate(true);
			graphicText.text = "Low";
		} else {
			PlayerSettingData.Instance.graphic = GRAPHIC_QUALITY.HIGH;
			graphicBtn.transform.DOLocalMoveX(36, 0.2f).SetUpdate(true);
			graphicBtn.DOColor(musicOn, 0.2f).SetUpdate(true);
			graphicText.text = "High";
		}
		SoundManager.Instance.PlayUIButtonClick();
		GlobalEventManager.Instance.OnButtonPressed(PopupManager.Instance.scene.ToString(), "setting_graphic");
	}

	public void SelectDrag () {
		PlayerSettingData.Instance.controlStyle = CONTROL_STYLE.FIXED;
		drag.color = controlOn;
		touch.color = controlOff;
		SoundManager.Instance.PlayUIButtonClick();
		GlobalEventManager.Instance.OnButtonPressed(PopupManager.Instance.scene.ToString(), "setting_drag");
	}

	public void SelectTouch () {
		PlayerSettingData.Instance.controlStyle = CONTROL_STYLE.FOLLOW;
		drag.color = controlOff;
		touch.color = controlOn;
		SoundManager.Instance.PlayUIButtonClick();
		GlobalEventManager.Instance.OnButtonPressed(PopupManager.Instance.scene.ToString(), "setting_touch");
	}
}