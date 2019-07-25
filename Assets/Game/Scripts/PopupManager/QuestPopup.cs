using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ABIPlugins;

public class QuestPopup : SingletonPopup<QuestPopup> {

	public QuestPopupData data;

	public override void Awake () {
		base.Awake();
		data.Init();
	}

	public void Show () {
		data.PreOpen();
		base.Show(true, () => {
			data.Open();	
		});
	}

	public override void Hide () {
		SoundManager.Instance.PlayUIButtonClick();
		data.Close();
		Hide(() => {
			HomeManager.Instance.UpgradeNotice();
			if (PlayerPrefs.GetInt(Const.TUT_UPGRADE, 0) == 0 && HomeManager.Instance.upgradeNotice.enabled) {
				TutorialManager.Instance.CheckUpgrade();
			}
		});
	}
}