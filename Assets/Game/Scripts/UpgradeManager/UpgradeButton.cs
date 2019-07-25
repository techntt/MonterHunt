using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UpgradeUI {
	public enum UpgradeType {
		Damage,
		Health,
		MaxHealth,
		Magnet
	}

	public class UpgradeButton : MonoBehaviour {

		public UpgradeType type;

		public Text upgradeText;
		public Text maxText;
		public Text goldText;
		public Text rankText;
		public GameObject gold;
		public GameObject rank;
		public Button upgradeButton;
		public Image notice;
		public AudioClip sfxUpgrade;

		int goldRequired;
		int rankRequired;

		void Awake () {
			UpgradeManager.Instance.OnViewShip += HandleOnViewShip;
		}

		void HandleOnViewShip (ShipUpgradeData s) {
			if (s.unlocked) {
				switch (type) {
					case UpgradeType.Damage:
						if (s.damageLv == UpgradeManager.maxDamageUpgradeTime)
							upgradeButton.gameObject.SetActive(false);
						else
							upgradeButton.gameObject.SetActive(true);
						upgradeText.text = "+ " + s.damageLv * 10 + "%";
						goldRequired = UpgradeManager.GetUpgradeDamageCost(UpgradeManager.Instance.currentShip, s.damageLv);
						rankRequired = UpgradeManager.GetUpgradeDamageRank(UpgradeManager.Instance.currentShip, s.damageLv);
						break;
					case UpgradeType.Health:
						if (s.hpLv == UpgradeManager.maxHPUpgradeTime)
							upgradeButton.gameObject.SetActive(false);
						else
							upgradeButton.gameObject.SetActive(true);
						upgradeText.text = "+ " + s.hpLv;
						goldRequired = UpgradeManager.GetUpgradeHPCost(UpgradeManager.Instance.currentShip, s.hpLv);
						rankRequired = UpgradeManager.GetUpgradeHPRank(UpgradeManager.Instance.currentShip, s.hpLv);
						break;
					case UpgradeType.MaxHealth:
						if (s.hpLimitLv == UpgradeManager.maxHPLimitUpgradeTime)
							upgradeButton.gameObject.SetActive(false);
						else
							upgradeButton.gameObject.SetActive(true);
						upgradeText.text = "+ " + s.hpLimitLv;
						goldRequired = UpgradeManager.GetUpgradeMaxHPCost(UpgradeManager.Instance.currentShip, s.hpLimitLv);
						rankRequired = UpgradeManager.GetUpgradeMaxHPRank(UpgradeManager.Instance.currentShip, s.hpLimitLv);
						break;
					case UpgradeType.Magnet:
						if (s.magnetLv == UpgradeManager.maxMagnetUpgradeTime)
							upgradeButton.gameObject.SetActive(false);
						else
							upgradeButton.gameObject.SetActive(true);
						upgradeText.text = "+ " + s.magnetLv;
						goldRequired = UpgradeManager.GetUpgradeMagnetCost(UpgradeManager.Instance.currentShip, s.magnetLv);
						rankRequired = UpgradeManager.GetUpgradeMagnetRank(UpgradeManager.Instance.currentShip, s.magnetLv);
						break;
				}
				if (rankRequired > PlayerData.Instance.rank) {
					rank.SetActive(true);
					rankText.text = "" + rankRequired;
					gold.SetActive(false);
					notice.enabled = false;
				} else {
					rank.SetActive(false);
					gold.SetActive(true);
					goldText.text = "" + goldRequired;
					if (goldRequired > PlayerData.Instance.gold) {
						goldText.color = Color.red;
						notice.enabled = false;
					} else {
						goldText.color = Color.white;
						notice.enabled = true;
					}
				}
			}
		}

		public void Upgrade () {
			MoreInfo();
			SoundManager.Instance.PlayUIButtonClick();
			GlobalEventManager.Instance.OnButtonPressed(PopupManager.Instance.scene.ToString(), "upgrade_" + type.ToString());
			if (PlayerData.Instance.gold >= goldRequired && PlayerData.Instance.rank >= rankRequired) {
				switch (type) {
					case UpgradeType.Damage:
						PlayerData.Instance.shipData[UpgradeManager.Instance.currentShip].damageLv++;
						break;
					case UpgradeType.Health:
						PlayerData.Instance.shipData[UpgradeManager.Instance.currentShip].hpLv++;
						break;
					case UpgradeType.MaxHealth:
						PlayerData.Instance.shipData[UpgradeManager.Instance.currentShip].hpLimitLv++;
						break;
					case UpgradeType.Magnet:
						PlayerData.Instance.shipData[UpgradeManager.Instance.currentShip].magnetLv++;
						break;
				}
				PlayerData.Instance.gold -= goldRequired;
				GlobalEventManager.Instance.OnCurrencyChanged("gold", "spend", goldRequired.ToString());
				PlayerData.Instance.SaveAllData();
				GameEventManager.Instance.OnPlayerUseCoin(null, goldRequired);
				QuestManager.SaveQuest();
				SoundManager.Instance.PlaySfxRewind(sfxUpgrade);
			}
			// not enough rank
			else if (PlayerData.Instance.rank < rankRequired) {
				NotifyPopup.Instance.Show("Too low rank", "You can get higher rank by completing quests", "Ok", () => {
					NotifyPopup.Instance.Hide();
				});
				UpgradeManager.Instance.state = HOME_STATE.POPUP;
			}
			// not enough gold
			else if (PlayerData.Instance.gold < goldRequired) {
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

		public void MoreInfo () {
			Text t = UpgradeManager.Instance.infoText;
			switch (type) {
				case UpgradeType.Damage:
					t.text = "Upgrade damage of each bullets";
					break;
				case UpgradeType.Health:
					t.text = "Upgrade starting HP and damage of your ship";
					UpgradeManager.Instance.SetDemoHp(true);
					break;
				case UpgradeType.MaxHealth:
					t.text = "Upgrade HP and damage limit of your ship";
					UpgradeManager.Instance.SetDemoHp(false);
					break;
				case UpgradeType.Magnet:
					t.text = "Upgrade the magnet on your ship";
					break;
			}
		}

		public void ChangeDisplayHp (bool isDemoHp) {
			UpgradeManager.Instance.SetDemoHp(isDemoHp);
			UpgradeManager.Instance.ViewShip(UpgradeManager.Instance.currentShip);
		}
	}
}