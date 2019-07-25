﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase.Analytics;

public class FireBaseManager : SingletonMonoBehaviour<FireBaseManager> {

    protected bool firebaseInitialized = false;
    void Awake () {
		Firebase.FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task => {
			var dependencyStatus = task.Result;
			if (dependencyStatus == Firebase.DependencyStatus.Available) {
                // Set a flag here indiciating that Firebase is ready to use by your
                // application.
                FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
                firebaseInitialized = true;

            } else {
				UnityEngine.Debug.LogError(System.String.Format(
					"Could not resolve all Firebase dependencies: {0}", dependencyStatus));
				// Firebase Unity SDK is not safe to use here.
			}
		});
	}

	void Start () {
		GlobalEventManager.Instance.NewQuestAdded += HandleQuestAdded;
		GlobalEventManager.Instance.QuestComplete += HandleQuestCompleted;
		GlobalEventManager.Instance.ButtonPressed += HandleButtonPressed;
		GlobalEventManager.Instance.currencyChanged += HandleCurrencyChanged;
		GlobalEventManager.Instance.watchAds += HandleWatchAds;
		GlobalEventManager.Instance.gameStart += HandleGameStart;
		GlobalEventManager.Instance.gameEnd += HandleGameEnd;
	}

	void HandleGameStart () {
        if (!firebaseInitialized)
            return;
        Parameter[] p = new Parameter[] { 
			new Parameter("level", CampaignManager.campaign.id.ToString()),
			new Parameter("tryTime", PlayerData.Instance.retryTimes.ToString()),
			new Parameter("ship", PlayerData.Instance.selectedShip.ToString()),
			new Parameter("damage", PlayerData.Instance.shipData[PlayerData.Instance.selectedShip].damageLv.ToString()),
			new Parameter("hp", PlayerData.Instance.shipData[PlayerData.Instance.selectedShip].hpLv.ToString()),
			new Parameter("maxHp", PlayerData.Instance.shipData[PlayerData.Instance.selectedShip].hpLimitLv.ToString()),
			new Parameter("magnet", PlayerData.Instance.shipData[PlayerData.Instance.selectedShip].magnetLv.ToString()),
		};
        FirebaseAnalytics.LogEvent(FirebaseEvent.campaignStart, p);
	}

	void HandleGameEnd () {
        if (!firebaseInitialized)
            return;
        Parameter[] p = new Parameter[] { 
			new Parameter("level", CampaignManager.campaign.id.ToString()),
			new Parameter("tryTime", PlayerData.Instance.retryTimes.ToString()),
			new Parameter("duration", GameManager.Instance.timePlay.ToString()),
			new Parameter("progressPercent", (GameManager.Instance.score / (float)CampaignManager.campaign.objective).ToString("P0")),
			new Parameter ("result", GameManager.Instance.gameResult.ToString()),
			new Parameter("ship", PlayerData.Instance.selectedShip.ToString()),
			new Parameter("damage", PlayerData.Instance.shipData[PlayerData.Instance.selectedShip].damageLv.ToString()),
			new Parameter("hp", PlayerData.Instance.shipData[PlayerData.Instance.selectedShip].hpLv.ToString()),
			new Parameter("maxHp", PlayerData.Instance.shipData[PlayerData.Instance.selectedShip].hpLimitLv.ToString()),
			new Parameter("magnet", PlayerData.Instance.shipData[PlayerData.Instance.selectedShip].magnetLv.ToString()),
		};
        FirebaseAnalytics.LogEvent(FirebaseEvent.campaignEnd, p);
	}

	void HandleWatchAds (string type, string location, string status) {
        if (!firebaseInitialized)
            return;
        Parameter[] p = new Parameter[] { 
			new Parameter("type", type),
			new Parameter("location", location),
			new Parameter("status", status)
		};
        FirebaseAnalytics.LogEvent(FirebaseEvent.watchVideoAds, p);
	}

	void HandleCurrencyChanged (string currencyType, string status, string amount) {
        if (!firebaseInitialized)
            return;

        string source = "";
		switch (PopupManager.Instance.scene) {
			case SCENE.HOME:
				source = "quest";
				break;
			case SCENE.UPGRADE:
				source = "upgrade";
				break;
			case SCENE.GAME:
				source = "play game";
				break;
			case SCENE.SHOP:
				source = "IAP";
				break;
		}
        
        Parameter[] p = new Parameter[] { 
			new Parameter("type", currencyType),
			new Parameter("status", status),
			new Parameter("amount", amount),
			new Parameter("source", source),
			new Parameter("ship", PlayerData.Instance.selectedShip.ToString()),
			new Parameter("damage", PlayerData.Instance.shipData[PlayerData.Instance.selectedShip].damageLv.ToString()),
			new Parameter("hp", PlayerData.Instance.shipData[PlayerData.Instance.selectedShip].hpLv.ToString()),
			new Parameter("maxHp", PlayerData.Instance.shipData[PlayerData.Instance.selectedShip].hpLimitLv.ToString()),
			new Parameter("magnet", PlayerData.Instance.shipData[PlayerData.Instance.selectedShip].magnetLv.ToString()),
		};
        FirebaseAnalytics.LogEvent(FirebaseEvent.currencyChanged, p);
	}

	void HandleButtonPressed (string screen, string btnName) {
        if (!firebaseInitialized)
            return;
        Parameter[] p = new Parameter[] {
			new Parameter("scene", screen),
			new Parameter("button", btnName)
		};
        FirebaseAnalytics.LogEvent(FirebaseEvent.buttonPressed, p);
	}

	void HandleQuestCompleted (Quest q) {
        if (!firebaseInitialized)
            return;
        Parameter[] p = new Parameter[] {
			new Parameter("id", q.id.ToString()),
			new Parameter("type", q.questType.ToString()),
			new Parameter("value", q.value.ToString()),
			new Parameter("value2", q.value2.ToString()),
            new Parameter("duration", q.duration.ToString()),
            new Parameter("times", q.times.ToString())
        };
        FirebaseAnalytics.LogEvent(FirebaseEvent.questComplete, p);
	}

	void HandleQuestAdded (Quest q) {
        if (!firebaseInitialized)
            return;
        Parameter[] p = new Parameter[] {
			new Parameter("id", q.id.ToString()),
			new Parameter("type", q.questType.ToString()),
			new Parameter("value", q.value.ToString()),
			new Parameter("value2", q.value2.ToString())
		};
        FirebaseAnalytics.LogEvent(FirebaseEvent.questAdded, p);
	}

	public void SendUserProperty () {
        if (!firebaseInitialized)
            return;
		FirebaseAnalytics.SetUserProperty(UserProperty.currentMission, CampaignManager.campaign.id.ToString());
		FirebaseAnalytics.SetUserProperty(UserProperty.rank, PlayerData.Instance.rank.ToString());
		for (int i = PlayerData.Instance.shipData.Keys.Count - 1; i >= 0; i--) {
			if (PlayerData.Instance.shipData[(SHIP_TYPE)i].unlocked) {
				FirebaseAnalytics.SetUserProperty(UserProperty.bestShip, ((SHIP_TYPE)i).ToString());
				break;
			}
		}
		FirebaseAnalytics.SetUserProperty(UserProperty.gold, PlayerData.Instance.gold.ToString());
		FirebaseAnalytics.SetUserProperty(UserProperty.controlStyle, PlayerSettingData.Instance.controlStyle.ToString());
	}
}

public class FirebaseEvent {
	public const string questComplete = "QuestCompleted";
	public const string questAdded = "QuestAdded";
	public const string buttonPressed = "ButtonPressed";
	public const string currencyChanged = "CurrencyChanged";
	public const string campaignStart = "CampaignStart";
	public const string campaignEnd = "CampaignEnd";
	public const string watchVideoAds = "WatchVideoAds";
}

public class UserProperty {
	public const string currentMission = "current_mission";
	public const string rank = "rank";
	public const string bestShip = "best_ship";
	public const string gold = "gold";
	public const string controlStyle = "control_style";
}