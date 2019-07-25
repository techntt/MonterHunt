using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ABIPlugins;

public class SettingPopup : SingletonPopup<SettingPopup> {

	public SettingManager manager;

	public void Show () {
		base.Show();
		manager.Show();
	}

	public override void Hide () {
		SoundManager.Instance.PlayUIButtonClick();
		base.Hide();
		manager.Hide();
	}
}