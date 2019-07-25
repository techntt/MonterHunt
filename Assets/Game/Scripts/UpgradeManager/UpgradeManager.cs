using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UpgradeUI {
	public class UpgradeManager : SingletonMonoBehaviour<UpgradeManager> {
		public const int maxDamageUpgradeTime = 50;
		public const int maxHPUpgradeTime = 30;
		public const int maxHPLimitUpgradeTime = 20;
		public const int maxMagnetUpgradeTime = 5;

		// HUD components
		public Text totalGoldText;
		public Text rankText;
		public Text infoText;

		public Button nextShipBtn;
		public Button prevShipBtn;

		[HideInInspector]public SHIP_TYPE currentShip;
		ShipUpgradeData currentShipData;

		public GameObject prevNotice, nextNotice;
		public GameObject upgradePanel;
		public GameObject unlockPanel;
		public Text shipName;

		Player[] ships;
		Player displayedShip;

		public delegate void ViewShipEvent (ShipUpgradeData s);
		public event ViewShipEvent OnViewShip;

		public HOME_STATE state;
		/// <summary>
		/// will the ship display its weapon based on its Hp or max Hp?
		/// </summary>
		public bool isDemoHp;

		void Awake () {
			PopupManager.Instance.scene = SCENE.UPGRADE;
		}

		void Start () {
			// spawn all ships the game has
			ships = new Player[(int)SHIP_TYPE.NONE];
			for (int i = 0; i < ships.Length; i++) {
				GameObject go = Instantiate(Resources.Load(Const.SHIP + i)) as GameObject;
				ships[i] = (Player)go.GetComponent(typeof(Player));
				go.GetComponent<InputController>().enabled = false;
				go.transform.position = new Vector3(10, -1.34f);
				go.SetActive(false);
			}
			// display total gold of player
			totalGoldText.text = "" + PlayerData.Instance.gold;
			// display rank of player
			rankText.text = "" + PlayerData.Instance.rank;
			// listen the change of gold and rank of player
			PlayerData.Instance.OnGoldChange += PlayerData_Instance_OnGoldChange;
			PlayerData.Instance.OnRankChange += PlayerData_Instance_OnRankChange;
			QuestManager.TrackQuest();
			state = HOME_STATE.NO_POPUP;
			// check tutorial
			if (PlayerPrefs.GetInt(Const.TUT_UPGRADE, 0) == 0) {
				TutorialManager.Instance.UpgradeShip();
				PlayerPrefs.SetInt(Const.TUT_UPGRADE, 1);
			}
			// display current selected ship
			ViewShip(PlayerData.Instance.selectedShip);
		}

		void PlayerData_Instance_OnRankChange (int gold) {
			rankText.text = "" + PlayerData.Instance.rank;
			ViewShip(currentShip);
		}

		void PlayerData_Instance_OnGoldChange (int gold) {
			totalGoldText.text = "" + PlayerData.Instance.gold;
			ViewShip(currentShip);
		}

		/// <summary>
		/// Views a ship based on ship type
		/// </summary>
		public void ViewShip (SHIP_TYPE ship) {
			currentShip = ship;
			currentShipData = PlayerData.Instance.shipData[currentShip];

			// if this is the first or the last ship, hide next/prev button
			nextShipBtn.interactable = true;
			prevShipBtn.interactable = true;
			if ((int)ship == 0)
				prevShipBtn.interactable = false;
			else if ((int)ship == (int)SHIP_TYPE.NONE - 1)
				nextShipBtn.interactable = false;
			// check if the prev ship is unlockable
			if (prevShipBtn.interactable) {
				prevNotice.SetActive(CanUnlockShip((SHIP_TYPE)((int)ship - 1)));
			} else
				prevNotice.SetActive(false);
			// check if the next ship is unlockable
			if (nextShipBtn.interactable) {
				nextNotice.SetActive(CanUnlockShip((SHIP_TYPE)((int)ship + 1)));
			} else
				nextNotice.SetActive(false);
			// deactivate the currently displayed ship
			if (displayedShip != null)
				displayedShip.gameObject.SetActive(false);

			// display the ship at the center of screen
			displayedShip = ships[(int)ship];
			displayedShip.gameObject.SetActive(true);

			// modify ship's data before showing it
			displayedShip.UpdateStat(ship);
			if (isDemoHp) {
				displayedShip.health = 1 + currentShipData.hpLv;
			} else {
				displayedShip.health = displayedShip.maxHealth;
			}
			displayedShip.myBaseWeapon.Activate();

			// display the ship name
			shipName.text = ShipDataManager.Instance.shipData[ship].shipName;
			// if the ship has been unlocked, show the upgrade panel
			if (currentShipData.unlocked) {
				unlockPanel.SetActive(false);
				upgradePanel.SetActive(true);
			}
			// if not, show the unlock panel
			else {
				unlockPanel.SetActive(true);
				upgradePanel.SetActive(false);
			}

			// fire event view ship
			if (OnViewShip != null)
				OnViewShip(currentShipData);
		}

		public void NextShip () {
			int id = (int)currentShip + 1;
			ViewShip((SHIP_TYPE)id);
		}

		public void PrevShip () {
			int id = (int)currentShip - 1;
			ViewShip((SHIP_TYPE)id);
		}

		public void BackToMenu () {
			SoundManager.Instance.PlayUIButtonClick();
			GlobalEventManager.Instance.OnButtonPressed(PopupManager.Instance.scene.ToString(), "back");
			PlayerData.Instance.SaveAllData();
			SceneManager.LoadScene(Const.SCENE_HOME);
		}

		public void SelectShip () {
			if (currentShipData.unlocked) {
				PlayerData.Instance.selectedShip = currentShip;
				ViewShip(currentShip);
				SoundManager.Instance.PlayUIButtonClick();
				GlobalEventManager.Instance.OnButtonPressed(PopupManager.Instance.scene.ToString(), "select_ship");
			}
		}

		public void SetDemoHp (bool isDemoHp) {
			this.isDemoHp = isDemoHp;
		}

		public static int GetUpgradeDamageCost (SHIP_TYPE shipType, int currentLv) {
			currentLv = Mathf.Clamp(currentLv, 0, maxDamageUpgradeTime);
			switch (shipType) {
				case SHIP_TYPE.STING:
					if (currentLv < 3)
						return 250 + 500 * currentLv;
					else
						return 1250 + 1250 * (currentLv - 2);
				case SHIP_TYPE.SWALLOW:
					return 2500 + (1750 * currentLv);
				default:
					return 0;
			}
		}

		public static int GetUpgradeDamageRank (SHIP_TYPE ship, int currentLv) {
			return ShipDataManager.Instance.shipData[ship].GetRequiredRankForDamage(currentLv);
		}

		public static int GetUpgradeHPCost (SHIP_TYPE shipType, int currentLv) {
			currentLv = Mathf.Clamp(currentLv, 0, maxHPUpgradeTime);
			switch (shipType) {
				case SHIP_TYPE.STING:
					return 5000 + 1500 * currentLv;
				case SHIP_TYPE.SWALLOW:
					return 7500 + 2750 * currentLv;
				default:
					return 0;
			}
		}

		public static int GetUpgradeHPRank (SHIP_TYPE ship, int currentLv) {
			return ShipDataManager.Instance.shipData[ship].GetRequiredRankForHp(currentLv);
		}

		public static int GetUpgradeMaxHPCost (SHIP_TYPE shipType, int currentLv) {
			currentLv = Mathf.Clamp(currentLv, 0, maxHPLimitUpgradeTime);
			switch (shipType) {
				case SHIP_TYPE.STING:
					return 2500 + 2500 * currentLv;
				case SHIP_TYPE.SWALLOW:
					return 3500 + currentLv * 3500;
				default:
					return 0;
			}
		}

		public static int GetUpgradeMaxHPRank (SHIP_TYPE ship, int currentLv) {
			return ShipDataManager.Instance.shipData[ship].GetRequiredRankForMaxHp(currentLv);
		}

		public static int GetUpgradeMagnetCost (SHIP_TYPE shipType, int currentLv) {
			currentLv = Mathf.Clamp(currentLv, 0, maxMagnetUpgradeTime);
			switch (shipType) {
				case SHIP_TYPE.STING:
					return 20000 + 15000 * currentLv;
				case SHIP_TYPE.SWALLOW:
					return 30000 + currentLv * 25000;
				default:
					return 0;
			}
		}

		public static int GetUpgradeMagnetRank (SHIP_TYPE ship, int currentLv) {
			return ShipDataManager.Instance.shipData[ship].GetRequiredRankForMagnet(currentLv);
		}

		public static int GetUnlockShipCost (SHIP_TYPE ship) {
			return ShipDataManager.Instance.shipData[ship].gold;
		}

		public static int GetUnlockShipRank (SHIP_TYPE ship) {
			return ShipDataManager.Instance.shipData[ship].rank;
		}

		public static bool CanUnlockShip (SHIP_TYPE ship) {
			if (ship == SHIP_TYPE.STING)
				return false;
			else if (PlayerData.Instance.shipData[ship].unlocked)
				return false;
			else {
				SHIP_TYPE prevShip = (SHIP_TYPE)((int)ship - 1);
				bool isCampaignPassed = CampaignManager.campaign.id > ShipDataManager.Instance.shipData[ship].campaignPassed;
				bool isPrevShipUnlocked = PlayerData.Instance.shipData[prevShip].unlocked;
				bool isEnoughRank = PlayerData.Instance.rank >= ShipDataManager.Instance.shipData[ship].rank;
				return isCampaignPassed && isPrevShipUnlocked && isEnoughRank;
			}
		}

		void Update () {
			#if UNITY_EDITOR || UNITY_STANDALONE_WIN
			if (Input.GetKey(KeyCode.C))
				PlayerData.Instance.gold += 10000;
			if (Input.GetKey(KeyCode.R))
				PlayerData.Instance.rank += 1;
			if (Input.GetKeyDown(KeyCode.Delete))
				PlayerPrefs.DeleteAll();
			#endif

			if (Input.GetKeyDown(KeyCode.Escape)) {
				if (state == HOME_STATE.NO_POPUP) {
					BackToMenu();
				} else {
					ABIPlugins.PopupManager.Instance.SequenceHidePopup();
					state = HOME_STATE.NO_POPUP;
				}
			}
		}

		void OnDestroy () {
			PlayerData.Instance.OnGoldChange -= PlayerData_Instance_OnGoldChange;
			PlayerData.Instance.OnRankChange -= PlayerData_Instance_OnRankChange;
		}
	}
}