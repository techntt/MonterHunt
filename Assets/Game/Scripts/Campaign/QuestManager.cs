using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager {

	public static List<Quest> currentQuests;
	static List<Dictionary<string, string>> data;
	public static int highestQuestID;
	public static bool hasCompletedQuest;

	public static void InitQuest () {
		currentQuests = new List<Quest>();
		ReadData();
		hasCompletedQuest = false;
		if (PlayerPrefs.HasKey(Const.QUEST)) {
			string s = PlayerPrefs.GetString(Const.QUEST);            
            string[] a = s.Split(new char[]{'@'}, System.StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < a.Length; i++) {
				string[] c = a[i].Split(new char[]{'_'});
				int id = int.Parse(c[0]);
				Quest q = new Quest(id, StringToQuestType(data[id]["quest"]), int.Parse(data[id]["value"]), int.Parse(data[id]["value2"]), data[id]["reward"]);
				q.currentValue = int.Parse(c[1]);
                if (c.Length == 4)
                {
                    q.duration = long.Parse(c[2]);
                    q.times = int.Parse(c[3]);
                }
                else
                {
                    q.duration = 0;
                    q.times = 0;
                }
                currentQuests.Add(q);
				if (q.currentValue >= q.value)
					hasCompletedQuest = true;
				else if (IsOneGameQuest(q.questType))
					q.currentValue = 0;
			}
		} else {
			// play game the first time
			for (int i = 0; i < 3; i++) {
				currentQuests.Add(new Quest(i, StringToQuestType(data[i]["quest"]), int.Parse(data[i]["value"]), int.Parse(data[i]["value2"]), data[i]["reward"]));
			}
		}        
		highestQuestID = currentQuests[2].id;
	}

	public static void SaveQuest () {
		string save = "";
		for (int i = 0; i < currentQuests.Count; i++) {
			save += string.Format("{0}_{1}_{2}_{3}@", currentQuests[i].id, currentQuests[i].currentValue, currentQuests[i].duration, currentQuests[i].times);
		}
		PlayerPrefs.SetString(Const.QUEST, save);
	}

	public static void AddQuest () {
		int i = highestQuestID + 1;
		Quest q = new Quest(i, StringToQuestType(data[i]["quest"]), int.Parse(data[i]["value"]), int.Parse(data[i]["value2"]), data[i]["reward"]);
		currentQuests.Add(q);
		highestQuestID++;
		GlobalEventManager.Instance.OnNewQuestAdded(q);
	}
	/// <summary>
	/// Called every time player enters game or shop
	/// </summary>
	public static void TrackQuest () {
		foreach (Quest q in currentQuests) {
			q.TrackQuest();
        }
	}

	public static void UpdateQuestList () {
		// remove completed quests
		hasCompletedQuest = false;
		for (int i = currentQuests.Count - 1; i >= 0; i--) {
			if (currentQuests[i].currentValue >= currentQuests[i].value) {
				// claim quest reward
				PlayerData.Instance.rank++;
				foreach (string s in currentQuests[i].reward.Keys) {
					if (s == "gold") {
						PlayerData.Instance.gold += currentQuests[i].reward[s];
						GlobalEventManager.Instance.OnCurrencyChanged("gold", "earn", currentQuests[i].reward[s].ToString());
					}
				}
				GlobalEventManager.Instance.OnQuestCompleted(currentQuests[i]);
				currentQuests.RemoveAt(i);
			}
		}
		// add quests
		for (int i = currentQuests.Count; i < 3; i++) {
			AddQuest();
		}
	}

	public static void ReadData () {
		data = CSVReader.ReadDataToList(DataManager.Instance.quest);
	}

	public static QUEST_TYPE StringToQuestType (string s) {
		for (int i = 0; i < (int)QUEST_TYPE.NONE; i++) {
			if (s == ((QUEST_TYPE)i).ToString()) {
				return (QUEST_TYPE)i;
			}
		}
		return QUEST_TYPE.NONE;
	}

	public static bool IsOneGameQuest (QUEST_TYPE t) {
		switch (t) {
			case QUEST_TYPE.BONUS_1_GAME:
			case QUEST_TYPE.COIN_1_GAME:
			case QUEST_TYPE.COLLIDE_1_GAME:
			case QUEST_TYPE.KILL_1_GAME:
			case QUEST_TYPE.KILL_ALL:
			case QUEST_TYPE.NO_COLLIDE:
				return true;
			default:
				return false;
		}
	}
}