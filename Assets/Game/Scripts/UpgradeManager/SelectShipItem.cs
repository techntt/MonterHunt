using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UpgradeUI {
	public class SelectShipItem : MonoBehaviour {

		public Image ship;
		public Text shipHealth;
		public Image Lock;
		public int myShipId;
		public Image notice;

		public void SetShipState (ShipUpgradeData data) {
			if (data.unlocked) {
				if (Lock != null)
					Lock.enabled = false;
				shipHealth.text = "" + 1;
				ship.color = GameManager.GetColorByHP(1, 60);
				notice.enabled = false;
			} else {
				if (Lock != null)
					Lock.enabled = true;
				shipHealth.text = "";
				ship.color = Color.black;
				if (UpgradeManager.CanUnlockShip(myShipId))
					notice.enabled = true;
				else
					notice.enabled = false;
			}
		}

		public void ViewShip () {
			SoundManager.Instance.PlayUIButtonClick();
			UpgradeManager.Instance.ViewShip(myShipId);
			GlobalEventManager.Instance.OnButtonPressed(PopupManager.Instance.scene.ToString(), "view_ship_" + myShipId);
		}
	}
}