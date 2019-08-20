using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class SettingManager : MonoBehaviour {

	public Image musicBtn;
	public Image soundBtn;
	public Text musicText;
	public Text soundText;
	public Color musicOn, musicOff;


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
    	
}