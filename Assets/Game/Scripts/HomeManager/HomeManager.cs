using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UpgradeUI;

public class HomeManager : SingletonMonoBehaviour<HomeManager> {

	//----------new UI content--------------
	public Text goldText;
	public Text rankText;
	public Text waveText;
	public Text highScoreText;
	public Text missionName;
	public Image missionImage;
	public Image questNotice;
	public Image upgradeNotice;
	public GameObject nextShip, prevShip;
	public Text target;
	public Sprite[] missionBg;
	public RectTransform PlayBtn;
	public RectTransform questBtn;
	public RectTransform upgradeBtn;
	//--------------------------------------

	public HOME_STATE state;

	List<SHIP_TYPE> availableShip;

	string currentVersion = "1.1";

	void Awake () {
		CheckVersion();
		CampaignManager.ReadData();
		QuestManager.InitQuest();
		state = HOME_STATE.NO_POPUP;
	}

	void Start () {
		PopupManager.Instance.scene = SCENE.HOME;
		if (!SoundManager.Instance.music.isPlaying)
			SoundManager.Instance.PlayGameMusic();
		InitUI();
		// check tutorial
		if (PlayerPrefs.GetInt(Const.TUT_PLAY, 0) == 0)
			TutorialManager.Instance.PointToStartButton();
		else if (PlayerPrefs.GetInt(Const.TUT_QUEST, 0) == 0 && questNotice.enabled) {
			TutorialManager.Instance.CheckQuest();
			PlayerPrefs.SetInt(Const.TUT_QUEST, 1);
		}
		else if (PlayerPrefs.GetInt(Const.TUT_UPGRADE, 0) == 0 && upgradeNotice.enabled) {
			TutorialManager.Instance.CheckUpgrade();
		}

		FireBaseManager.Instance.SendUserProperty();
	}

	void Update () {
		if (Input.GetKeyDown(KeyCode.Escape)) {
			if (state == HOME_STATE.NO_POPUP) {
				NotifyPopup.Instance.Show("Quit game", "Do you want to quit?", "Yes", () => {
					Application.Quit();
				}, "No", () => {
					
				});
				state = HOME_STATE.POPUP;
			} else {
				ABIPlugins.PopupManager.Instance.SequenceHidePopup();
				state = HOME_STATE.NO_POPUP;
			}
		}
	}

	void InitUI () {
		// get list of available ships
		availableShip = new List<SHIP_TYPE>();
		for (int i = 0; i < (int)SHIP_TYPE.NONE; i++) {
			if (PlayerData.Instance.shipData[(SHIP_TYPE)i].unlocked)
				availableShip.Add((SHIP_TYPE)i);
		}
		if (availableShip.Count > 1) {
			nextShip.SetActive(true);
			prevShip.SetActive(true);
		} else {
			nextShip.SetActive(false);
			prevShip.SetActive(false);
		}
		PlayerData_Instance_OnGoldChange(0);
		PlayerData_Instance_OnRankChange(0);
		UpdateShip();
		PlayerData.Instance.OnGoldChange += PlayerData_Instance_OnGoldChange;
		PlayerData.Instance.OnRankChange += PlayerData_Instance_OnRankChange;
		// get information of current campaign
		waveText.text = "" + (PlayerData.Instance.currentMission + 1);
		highScoreText.text = PlayerData.Instance.bestScore.ToString();
		Campaign c = CampaignManager.campaign;
		missionName.text = c.name;
		missionImage.sprite = missionBg[c.bossID];
		target.text = string.Format("{0}", c.objective);
		// check if player can upgrade current ship
		UpgradeNotice();
		// check if there's any completed quest
		if (QuestManager.hasCompletedQuest || DailyQuestManager.Instance.status == DAILY_QUEST_STATUS.COMPLETED)
			questNotice.enabled = true;
		else
			questNotice.enabled = false;
	}

	public void UpgradeNotice () {
		int gold = PlayerData.Instance.gold;
		int rank = PlayerData.Instance.rank;
		SHIP_TYPE type = PlayerData.Instance.selectedShip;
		ShipUpgradeData data = PlayerData.Instance.shipData[type];
		bool result = false;
		if (gold >= UpgradeManager.GetUpgradeDamageCost(type, data.damageLv)
		    && rank >= UpgradeManager.GetUpgradeDamageRank(type, data.damageLv)) {
			result = true;
		} else if (gold >= UpgradeManager.GetUpgradeHPCost(type, data.hpLv)
		           && rank >= UpgradeManager.GetUpgradeHPRank(type, data.hpLv)) {
			result = true;
		} else if (gold >= UpgradeManager.GetUpgradeMaxHPCost(type, data.hpLimitLv)
		           && rank >= UpgradeManager.GetUpgradeMaxHPRank(type, data.hpLimitLv)) {
			result = true;
		} else if (gold >= UpgradeManager.GetUpgradeMagnetCost(type, data.magnetLv)
		         && rank >= UpgradeManager.GetUpgradeMagnetRank(type, data.magnetLv)) {
			result = true;
		}
		for (int i = (int)type; i < (int)SHIP_TYPE.NONE; i++) {
			if (UpgradeManager.CanUnlockShip((SHIP_TYPE)i)) {
				result = true;
				break;
			}
		}
		upgradeNotice.enabled = result;
	}

	void PlayerData_Instance_OnGoldChange (int gold) {
		goldText.text = "" + PlayerData.Instance.gold;
	}

	void PlayerData_Instance_OnRankChange (int rank) {
		rankText.text = "" + PlayerData.Instance.rank;
	}

	void UpdateShip () {
		ShipContainer.Instance.ShowShip(PlayerData.Instance.selectedShip);
	}

	public void SelectNextShip () {
		SoundManager.Instance.PlayUIButtonClick();
		int current = availableShip.IndexOf(PlayerData.Instance.selectedShip);
		if (current == availableShip.Count - 1)
			SelectShip(availableShip[0]);
		else
			SelectShip(availableShip[current + 1]);
		GlobalEventManager.Instance.OnButtonPressed(PopupManager.Instance.scene.ToString(), "next_ship");
	}

	public void SelectPrevShip () {
		SoundManager.Instance.PlayUIButtonClick();
		int current = availableShip.IndexOf(PlayerData.Instance.selectedShip);
		if (current == 0)
			SelectShip(availableShip[availableShip.Count - 1]);
		else
			SelectShip(availableShip[current - 1]);
		GlobalEventManager.Instance.OnButtonPressed(PopupManager.Instance.scene.ToString(), "prev_ship");
	}

	void SelectShip (SHIP_TYPE type) {
		PlayerData.Instance.selectedShip = type;
		UpdateShip();
	}

	public void UpgradeShip () {
		SoundManager.Instance.PlayUIButtonClick();
		SceneManager.LoadScene(Const.SCENE_UPGRADE);
		GlobalEventManager.Instance.OnButtonPressed(PopupManager.Instance.scene.ToString(), "upgrade");
	}

	public void PlayGame () {
		SoundManager.Instance.PlayUIButtonClick();
		SceneManager.LoadScene(Const.SCENE_GAME);
		GlobalEventManager.Instance.OnButtonPressed(PopupManager.Instance.scene.ToString(), "play");
	}

	public void Shop () {
		SoundManager.Instance.PlayUIButtonClick();
		SceneManager.LoadScene(Const.SCENE_SHOP);
		GlobalEventManager.Instance.OnButtonPressed(PopupManager.Instance.scene.ToString(), "shop");
	}

	public void Quest () {
		SoundManager.Instance.PlayUIButtonClick();
		questNotice.enabled = false;
		QuestPopup.Instance.Show();
		state = HOME_STATE.POPUP;
		GlobalEventManager.Instance.OnButtonPressed(PopupManager.Instance.scene.ToString(), "quest");
	}

	public void Setting () {
		SoundManager.Instance.PlayUIButtonClick();
		SettingPopup.Instance.Show();
		state = HOME_STATE.POPUP;
		GlobalEventManager.Instance.OnButtonPressed(PopupManager.Instance.scene.ToString(), "setting");
	}

	void OnDestroy () {
		PlayerData.Instance.OnGoldChange -= PlayerData_Instance_OnGoldChange;
		PlayerData.Instance.OnRankChange -= PlayerData_Instance_OnRankChange;
		QuestManager.SaveQuest();
		PlayerData.Instance.SaveAllData();
	}

	void CheckVersion () {
		if (PlayerPrefs.GetString(Const.VERSION, "1.0") != currentVersion) {
			PlayerPrefs.DeleteAll();
			PlayerPrefs.SetString(Const.VERSION, currentVersion);
		}
	}
}

public enum HOME_STATE {
	POPUP,
	NO_POPUP
}