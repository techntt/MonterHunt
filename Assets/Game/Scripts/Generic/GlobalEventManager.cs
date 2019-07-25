using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalEventManager : SingletonMonoBehaviour<GlobalEventManager> {

	public delegate void QuestEvent (Quest q);
	public event QuestEvent QuestComplete;
	public event QuestEvent NewQuestAdded;

	public delegate void ButtonEvent (string screen, string btnName);
	public event ButtonEvent ButtonPressed;

	public delegate void CurrencyEvent (string currencyType, string status, string amount);
	public event CurrencyEvent currencyChanged;

	public delegate void AdsEvent (string type, string location, string status);
	public event AdsEvent watchAds;

	public delegate void CampaignEvent ();
	public event CampaignEvent gameStart;
	public event CampaignEvent gameEnd;
	public event CampaignEvent playerGetDailyItem;

	public delegate void IntEvent (int id);
	public event IntEvent DailyQuestCompleted;

	public void OnQuestCompleted (Quest q) {
		if (QuestComplete != null)
			QuestComplete(q);
	}

	public void OnNewQuestAdded (Quest q) {
		if (NewQuestAdded != null)
			NewQuestAdded(q);
	}

	public void OnButtonPressed (string screen, string btnName) {
		if (ButtonPressed != null)
			ButtonPressed(screen, btnName);
	}

	public void OnCurrencyChanged (string currencyType, string status, string amount) {
		if (currencyChanged != null)
			currencyChanged(currencyType, status, amount);
	}

	public void OnWatchAds (string type, string location, string status) {
		if (watchAds != null)
			watchAds(type, location, status);
	}

	public void OnGameStart () {
		if (gameStart != null)
			gameStart();
	}

	public void OnGameEnd () {
		if (gameEnd != null)
			gameEnd();
	}

	public void OnPlayerGetDailyItem () {
		if (playerGetDailyItem != null)
			playerGetDailyItem();
	}

	public void OnDailyQuestCompleted (int id) {
		if (DailyQuestCompleted != null)
			DailyQuestCompleted(id);
	}
}