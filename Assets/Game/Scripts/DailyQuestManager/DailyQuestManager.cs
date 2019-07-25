using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DailyQuestManager : SingletonMonoBehaviour<DailyQuestManager> {

	// daily quest icon variable
	public Sprite[] dailySprites;
	private Sprite _todaySprite;

	public Sprite todaySprite {
		get { return _todaySprite; }
	}

	/// <summary>
	/// The status of current daily quest.
	/// </summary>
	public DAILY_QUEST_STATUS status;

	/// <summary>
	/// The objective number of current quest.
	/// </summary>
	public int objective;

	/// <summary>
	/// The collected number of current quest.
	/// </summary>
	public int collected;

	/// <summary>
	/// The current identifier.
	/// </summary>
	public int currentId;

	/// <summary>
	/// The reward.
	/// </summary>
	public int reward;

	public int rankReward;

	/// <summary>
	/// The waiting time in minute to unlock next quest
	/// </summary>
	int cooldown;

	/// <summary>
	/// the waiting time to unlock the next quest
	/// </summary>
	TimeSpan waitTime;

	/// <summary>
	/// the date daily quests start
	/// </summary>
	DateTime savedDate;

	public DateTime nextQuestUnlockTime;

	int goldPerCoin;

	public int dropChance;

	List<Dictionary<string, string>> data;

	public delegate void DailyQuestEvent ();

	public event DailyQuestEvent questActivated;

	void Awake () {
		if (Instance != this)
			Destroy(gameObject);
		else
			DontDestroyOnLoad(gameObject);
		_todaySprite = dailySprites[DateTime.Today.Day - 1];
	}

	void Start () {
		// get the config data from csv file
		data = CSVReader.Read(Const.DAILY_QUEST_DATA);
		InitDailyQuest();
		GlobalEventManager.Instance.playerGetDailyItem += HandlePlayerGetDailyItem;
		InvokeRepeating("CheckQuest", 1, 1);
	}

	void HandlePlayerGetDailyItem () {
		if (status == DAILY_QUEST_STATUS.ACTIVE) {
			collected = Mathf.Clamp(collected + 1, 0, objective);
			UpdateStatus();
		}
	}

	public void InitDailyQuest () {
		// get player's saved data
		string currentQuestData = PlayerPrefs.GetString(Const.DAILY_QUEST, "2018/9/10-11:33:00@0@0@100");
		// extract data
		string[] divide1 = currentQuestData.Split(new char[] { '@' });
		string[] divide2 = divide1[0].Split(new char[]{ '/', '-', ':' });
		savedDate = new DateTime(int.Parse(divide2[0]), int.Parse(divide2[1]), int.Parse(divide2[2]), int.Parse(divide2[3]),
		                         int.Parse(divide2[4]), int.Parse(divide2[5]));
		currentId = int.Parse(divide1[1]);
		collected = int.Parse(divide1[2]);
		reward = int.Parse(divide1[3]);
		GetQuestData(currentId);
		nextQuestUnlockTime = savedDate + waitTime;
		UpdateStatus();
	}

	public void CheckQuest () {
		if (PopupManager.Instance.scene != SCENE.GAME) {
			if (savedDate.Date != DateTime.Today) {
				savedDate = DateTime.Now;
				ActivateQuest(0);
			} else {
				UpdateStatus();
			}
			if (status == DAILY_QUEST_STATUS.WAIT && IsNextQuestAvailable()) {
				ActivateQuest(currentId + 1);
			}
		}
	}

	public void ActivateQuest (int id) {
		if (id >= 0 && id < data.Count) {
			currentId = id;
			collected = 0;
			GetQuestData(id);
			goldPerCoin = 10 + CampaignManager.campaign.id + 5 * (int)PlayerData.Instance.GetHighestShip();
			reward = int.Parse(data[id]["reward"]) * goldPerCoin;
			savedDate = DateTime.Now;
			nextQuestUnlockTime = DateTime.Now + waitTime;
			status = DAILY_QUEST_STATUS.ACTIVE;
			if (questActivated != null)
				questActivated();
		}
	}

	void GetQuestData (int id) {
		if (id >= 0 && id < data.Count) {
			// get the objective, the reward data and the cooldown
			objective = int.Parse(data[id]["value"]);
			cooldown = int.Parse(data[id]["cooldown"]);
			dropChance = int.Parse(data[id]["dropChance"]);
			rankReward = int.Parse(data[id]["rank"]);
			waitTime = new TimeSpan(cooldown / 60, cooldown % 60, 0);
		}
	}

	void UpdateStatus () {
		if (collected < 0) {
			if (currentId < data.Count - 1)
				status = DAILY_QUEST_STATUS.WAIT;
			else
				status = DAILY_QUEST_STATUS.OUT_OF_QUEST;
		} else if (collected < objective)
			status = DAILY_QUEST_STATUS.ACTIVE;
		else
			status = DAILY_QUEST_STATUS.COMPLETED;
	}

	public bool IsNextQuestAvailable () {
		if (nextQuestUnlockTime > DateTime.Now)
			return false;
		else
			return true;
	}

	public void CompleteQuest () {
		if (status == DAILY_QUEST_STATUS.COMPLETED) {
			ClaimReward();
			if (IsNextQuestAvailable()) {
				ActivateQuest(currentId + 1);
			} else {
				collected = -1;
			}
			UpdateStatus();
		}
	}

	public void SaveQuest () {
		string s = string.Format("{0:yyyy}/{0:MM}/{0:dd}-{0:hh}:{0:mm}:{0:ss}@{1}@{2}@{3}", savedDate, currentId, collected, reward);
		PlayerPrefs.SetString(Const.DAILY_QUEST, s);
	}

	public void ClaimReward () {
		PlayerData.Instance.gold += reward;
		PlayerData.Instance.rank += rankReward;
		GlobalEventManager.Instance.OnCurrencyChanged("gold", "earn", reward.ToString());
		PlayerData.Instance.SaveAllData();
	}

	void OnApplicationQuit () {
		SaveQuest();
	}
}

public enum DAILY_QUEST_STATUS {
	/// <summary>
	/// the daily quest is not completed yet
	/// </summary>
	ACTIVE,
	/// <summary>
	/// the daily quest is completed but player has not claimed reward yet
	/// </summary>
	COMPLETED,
	/// <summary>
	/// the daily quest is completed and player has claimed reward, but the next quest is not unlocked yet
	/// </summary>
	WAIT,
	/// <summary>
	/// player has completed all quests
	/// </summary>
	OUT_OF_QUEST,
}