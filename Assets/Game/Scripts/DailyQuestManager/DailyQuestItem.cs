using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class DailyQuestItem : MonoBehaviour {

	public GameObject questCard;
	public Text hiddenText;
	public Text title;
	public Image dailyIcon;
	public Text rankReward;
	public Text goldReward;
	public Text progressTxt;
	public Image progressImg;

	void Start () {
		Init();
		DailyQuestManager.Instance.questActivated += HandleQuestActivated;
	}

	void HandleQuestActivated () {
		Init();
	}

	public void Init () {
		// if there's no more quests, hide the quest card
		if (DailyQuestManager.Instance.status == DAILY_QUEST_STATUS.OUT_OF_QUEST) {
			questCard.SetActive(false);
			hiddenText.text = "No more quests!";
			return;
		}
		// if waiting for next quest, show the remaining time
		else if (DailyQuestManager.Instance.status == DAILY_QUEST_STATUS.WAIT) {
			questCard.SetActive(false);
			InvokeRepeating("UpdateTime", 0, 1);
			return;
		}
		title.text = string.Format("{0}. Collect {1}", DailyQuestManager.Instance.currentId + 1, DailyQuestManager.Instance.objective);
		dailyIcon.sprite = DailyQuestManager.Instance.todaySprite;
		rankReward.text = DailyQuestManager.Instance.rankReward.ToString();
		goldReward.text = DailyQuestManager.Instance.reward.ToString();
		progressImg.fillAmount = 0;
		// if the current quest is unfinished, show its progress
		if (DailyQuestManager.Instance.status == DAILY_QUEST_STATUS.ACTIVE) {
			hiddenText.text = "";
			questCard.SetActive(true);
			questCard.transform.DOScaleY(1, 0.2f);
			progressTxt.text = string.Format("{0}/{1}", DailyQuestManager.Instance.collected, DailyQuestManager.Instance.objective);
			progressImg.DOFillAmount((float)DailyQuestManager.Instance.collected / DailyQuestManager.Instance.objective, 1);
		}
		// if the current quest is completed
		else if (DailyQuestManager.Instance.status == DAILY_QUEST_STATUS.COMPLETED) {
			StartCoroutine("CompleteQuest");
		}
	}

	IEnumerator CompleteQuest () {
		hiddenText.text = "";
		progressTxt.text = "Completed!";
		progressImg.DOFillAmount(1, 1);
		yield return new WaitForSeconds(1);
		for (int i = 0; i < 3; i++) {
			progressTxt.enabled = false;
			yield return new WaitForSeconds(0.1f);
			progressTxt.enabled = true;
			yield return new WaitForSeconds(0.1f);
		}
		questCard.transform.DOScaleY(0, 0.2f);
		yield return new WaitForSeconds(0.2f);
		DailyQuestManager.Instance.CompleteQuest();
		Init();
	}

	void UpdateTime () {
		TimeSpan t = DailyQuestManager.Instance.nextQuestUnlockTime - DateTime.Now;
		hiddenText.text = string.Format("Next quest available in {0:D2}:{1:D2}:{2:D2}", t.Hours, t.Minutes, t.Seconds);
	}

	void OnDestroy () {
		CancelInvoke();
		StopAllCoroutines();
		DailyQuestManager.Instance.questActivated -= HandleQuestActivated;
	}
}