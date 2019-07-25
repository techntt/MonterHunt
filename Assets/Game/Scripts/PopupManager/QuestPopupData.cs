using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuestPopupData : MonoBehaviour {

	public QuestItem[] quests;
	public Button okButton;

	public delegate void questItemEvent (QuestItem q);
	public event questItemEvent questClose;

	public void Init () {
		for (int i = 0; i < quests.Length; i++) {
			quests[i].Init();
			questClose += quests[i].HandleOtherQuestItemClose;
		}
	}

	public void PreOpen () {
		for (int i = 0; i < quests.Length; i++) {
			quests[i].Reset(false);
			quests[i].Init(QuestManager.currentQuests[i]);
		}
	}

	public void Open () {
		if (PopupManager.Instance.scene == SCENE.HOME) {
			okButton.interactable = false;
			// update quest list
			QuestManager.UpdateQuestList();
			// check if any quest is completed
			StartCoroutine ("CheckCompletedQuest");
		}
	}

	IEnumerator CheckCompletedQuest () {
		int completedQuest = 0;
		for (int i = 0; i < quests.Length; i++) {
			if (quests[i].isCompleted) {
				quests[i].Close();
				completedQuest++;
				yield return new WaitForSeconds(1.7f);
			}
		}
		// re-open quest popup
		for (int i = 0; i < quests.Length; i++) {
			if (i < quests.Length - completedQuest) {
				quests[i].Reset(false);
				quests[i].Init(QuestManager.currentQuests[i]);
			} else {
				quests[i].Reset(true);
				quests[i].Init(QuestManager.currentQuests[i]);
				quests[i].Open();
				yield return new WaitForSeconds(0.25f);
			}
		}
		okButton.interactable = true;
		yield return null;
	}

	public void OnQuestClose (QuestItem q) {
		if (questClose != null)
			questClose(q);
	}

	public void Close () {
		StopCoroutine("CheckCompletedQuest");
	}

	void OnDestroy () {
		questClose = null;
	}
}