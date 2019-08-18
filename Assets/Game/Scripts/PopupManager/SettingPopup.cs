using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingPopup : BasePopup {

	public SettingManager manager;

	public void Show () {
		manager.Show();
	}

	public void Hide () {
		SoundManager.Instance.PlayUIButtonClick();
		manager.Hide();
	}
}