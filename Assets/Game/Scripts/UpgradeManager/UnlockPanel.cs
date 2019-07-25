using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UpgradeUI {
	public class UnlockPanel : MonoBehaviour {
		public Text requirement;
		public Text gold;
		public GameObject unlockBtn;

		int unlockCost, unlockRank;

		void Awake () {
			UpgradeManager.Instance.OnViewShip += HandleOnViewShip;
		}

		void HandleOnViewShip (ShipUpgradeData s) {
			if (!s.unlocked) {
				SHIP_TYPE type = UpgradeManager.Instance.currentShip;
				ShipData data = ShipDataManager.Instance.shipData[UpgradeManager.Instance.currentShip];
				SHIP_TYPE prevType = (SHIP_TYPE)((int)type - 1);
				bool isPrevShipUnlocked = PlayerData.Instance.shipData[prevType].unlocked;
				bool isCampaignPassed = CampaignManager.campaign.id > ShipDataManager.Instance.shipData[UpgradeManager.Instance.currentShip].campaignPassed;
				unlockCost = UpgradeManager.GetUnlockShipCost(UpgradeManager.Instance.currentShip);
				unlockRank = UpgradeManager.GetUnlockShipRank(UpgradeManager.Instance.currentShip);
				// first, check if player has match the campaign passed requirement
				if (!isCampaignPassed) {
					requirement.text = string.Format("You need to pass mission {0} first", data.campaignPassed + 1);
				}
				// after that, check if the previous ship is unlocked
				else if (!isPrevShipUnlocked) {
					requirement.text = string.Format("You need to unlock {0} first", ShipDataManager.Instance.shipData[prevType].shipName);
				} else if (unlockRank > PlayerData.Instance.rank) {
					requirement.text = string.Format("Require rank {0}", unlockRank);
				} else {
					requirement.text = "";
				}
				unlockBtn.SetActive(isPrevShipUnlocked && isCampaignPassed && (unlockRank <= PlayerData.Instance.rank));
				gold.text = "" + unlockCost;
				if (unlockCost > PlayerData.Instance.gold)
					gold.color = Color.red;
				else
					gold.color = Color.white;
			}
		}

		public void Unlock () {
			SoundManager.Instance.PlayUIButtonClick();
			if (PlayerData.Instance.gold >= unlockCost && PlayerData.Instance.rank >= unlockRank) {
				PlayerData.Instance.shipData[UpgradeManager.Instance.currentShip].unlocked = true;
				PlayerData.Instance.gold -= unlockCost;
				GlobalEventManager.Instance.OnCurrencyChanged("gold", "spend", unlockCost.ToString());
				PlayerData.Instance.selectedShip = UpgradeManager.Instance.currentShip;
				PlayerData.Instance.SaveAllData();
				GameEventManager.Instance.OnPlayerUseCoin(null, unlockCost);
				QuestManager.SaveQuest();
			}
			// not enough gold
			else {
				//				NotifyPopup.Instance.Show("Not enough gold", "Do you want to buy more gold?", "Yes", () => {
				//					NotifyPopup.Instance.Hide();
				//					UnityEngine.SceneManagement.SceneManager.LoadScene(Const.SCENE_SHOP);
				//				}, "No", () => {
				//					
				//				});

				NotifyPopup.Instance.Show("Not enough gold", "Keep playing and collect more gold!", "Ok", () => {
					NotifyPopup.Instance.Hide();
				});
				UpgradeManager.Instance.state = HOME_STATE.POPUP;

			}
		}
	}
}