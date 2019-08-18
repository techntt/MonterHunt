using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HomeManager : SingletonMonoBehaviour<HomeManager> {
	//----------new UI content--------------
	public Text goldText;
	public Text rankText;
	public Image questNotice;
	public Image upgradeNotice;
	public Sprite[] missionBg;
	public RectTransform PlayBtn;
	public RectTransform questBtn;
	public RectTransform upgradeBtn;

    public GameObject missionPass, missionFuture;
    public Text tvPass, tvFuture;
    public Image missionBoss;
	//--------------------------------------

	public HOME_STATE state;
	List<int> availableShip;
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
				state = HOME_STATE.NO_POPUP;
			}
		}

        if (Input.GetKey(KeyCode.G))
            PlayerData.Instance.gold += 10;
        if (Input.GetKey(KeyCode.C))
            PlayerData.Instance.crystal += 10;
        if (Input.GetKey(KeyCode.Delete))
            PlayerPrefs.DeleteAll();
    }

	void InitUI () {
		// get list of available ships
		availableShip = new List<int>();
		for (int i = 0; i < ShipDataManager.Instance.shipData.Count; i++) {
			if (PlayerData.Instance.shipData[i].unlocked)
				availableShip.Add(i);
		}
        
		PlayerData_Instance_OnGoldChange(0);
		PlayerData_Instance_OnCrystalChange(0);
		ShipContainer.Instance.ShowShip(PlayerData.Instance.selectedShip);
		PlayerData.Instance.OnGoldChange += PlayerData_Instance_OnGoldChange;
		PlayerData.Instance.OnCrystalChange += PlayerData_Instance_OnCrystalChange;
		// get information of current campaign		
		Campaign c = CampaignManager.campaign;
        if (c.id > 0)
        {
            tvPass.text = c.id.ToString();
            missionPass.SetActive(true);
        }
        else
            missionPass.SetActive(false);
        tvFuture.text = (c.id + 2).ToString();
		missionBoss.sprite = missionBg[c.bossID];
		// check if player can upgrade current ship
		UpgradeNotice();
		// check if there's any completed quest
		if (QuestManager.hasCompletedQuest)
			questNotice.enabled = true;
		else
			questNotice.enabled = false;
	}

	public void UpgradeNotice () {
		int gold = PlayerData.Instance.gold;
		int crystal = PlayerData.Instance.crystal;
		int id = PlayerData.Instance.selectedShip;
		ShipUpgradeData data = PlayerData.Instance.shipData[id];
		bool result = false;		
		upgradeNotice.enabled = result;
	}

	void PlayerData_Instance_OnGoldChange (int gold) {
		goldText.text = "" + PlayerData.Instance.gold;
	}

	void PlayerData_Instance_OnCrystalChange (int crystal) {
		rankText.text = "" + PlayerData.Instance.crystal;
	}
    
	public void PlayGame () {
		SoundManager.Instance.PlayUIButtonClick();
		SceneManager.LoadScene(Const.SCENE_GAME);
		GlobalEventManager.Instance.OnButtonPressed(PopupManager.Instance.scene.ToString(), "play");
	}
    
	void OnDestroy () {
		PlayerData.Instance.OnGoldChange -= PlayerData_Instance_OnGoldChange;
		PlayerData.Instance.OnCrystalChange -= PlayerData_Instance_OnCrystalChange;
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