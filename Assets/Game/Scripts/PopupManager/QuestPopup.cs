using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestPopup : SingletonMonoBehaviour<QuestPopup> {

	public QuestPopupData data;

	private void Awake () {
		data.Init();
	}

	public void Show () {
		data.PreOpen();
        data.Open();
    }

	public void Hide () {
		SoundManager.Instance.PlayUIButtonClick();
		data.Close();
        HomeManager.Instance.UpgradeNotice();
        if (PlayerPrefs.GetInt(Const.TUT_UPGRADE, 0) == 0 && HomeManager.Instance.upgradeNotice.enabled)
        {
            TutorialManager.Instance.CheckUpgrade();
        }
    }
}