using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipDataManager : MonoBehaviour {

	public static ShipDataManager Instance;

	public Dictionary<SHIP_TYPE, ShipData> shipData;

	[SerializeField]
	List<ShipData> data;

	void Awake () {
		if (Instance == null) {
			Instance = this;
			DontDestroyOnLoad(gameObject);
			shipData = new Dictionary<SHIP_TYPE, ShipData>();
			List<Dictionary<string, string>> temp = CSVReader.Read(Const.SHIP_DATA);
			char[] c = new char[]{'_'};
			char[] c1 = new char[]{'@'};
			for (int i = 0; i < data.Count; i++) {
				data[i].shipName = temp[i]["name"];
				data[i].baseDamage = float.Parse(temp[i]["damage"]);
				data[i].baseScore = int.Parse(temp[i]["score"]);
				data[i].campaignPassed = int.Parse(temp[i]["campaign"]);
				data[i].gold = int.Parse(temp[i]["gold"]);
				data[i].rank = int.Parse(temp[i]["rank"]);
				// read damage upgrade data
				string[] s = temp[i]["damageUpgrade"].Split(c1, System.StringSplitOptions.RemoveEmptyEntries);
				data[i].damageUpgradeLimitRank = new int[s.Length];
				data[i].damageUpgradeLimitLevel = new int[s.Length];
				for (int j = 0; j < s.Length; j++) {
					string[] s1 = s[j].Split(c, System.StringSplitOptions.RemoveEmptyEntries);
					data[i].damageUpgradeLimitLevel[j] = int.Parse(s1[0]);
					data[i].damageUpgradeLimitRank[j] = int.Parse(s1[1]);
				}
				// read hp upgrade data
				s = temp[i]["hpUpgrade"].Split(c1, System.StringSplitOptions.RemoveEmptyEntries);
				data[i].hpUpgradeLimitRank = new int[s.Length];
				data[i].hpUpgradeLimitLevel = new int[s.Length];
				for (int j = 0; j < s.Length; j++) {
					string[] s1 = s[j].Split(c, System.StringSplitOptions.RemoveEmptyEntries);
					data[i].hpUpgradeLimitLevel[j] = int.Parse(s1[0]);
					data[i].hpUpgradeLimitRank[j] = int.Parse(s1[1]);
				}
				// read max hp upgrade data
				s = temp[i]["maxHpUpgrade"].Split(c1, System.StringSplitOptions.RemoveEmptyEntries);
				data[i].maxHpUpgradeLimitRank = new int[s.Length];
				data[i].maxHpUpgradeLimitLevel = new int[s.Length];
				for (int j = 0; j < s.Length; j++) {
					string[] s1 = s[j].Split(c, System.StringSplitOptions.RemoveEmptyEntries);
					data[i].maxHpUpgradeLimitLevel[j] = int.Parse(s1[0]);
					data[i].maxHpUpgradeLimitRank[j] = int.Parse(s1[1]);
				}
				// read magnet upgrade data
				s = temp[i]["magnetUpgrade"].Split(c1, System.StringSplitOptions.RemoveEmptyEntries);
				data[i].magnetUpgradeLimitRank = new int[s.Length];
				data[i].magnetUpgradeLimitLevel = new int[s.Length];
				for (int j = 0; j < s.Length; j++) {
					string[] s1 = s[j].Split(c, System.StringSplitOptions.RemoveEmptyEntries);
					data[i].magnetUpgradeLimitLevel[j] = int.Parse(s1[0]);
					data[i].magnetUpgradeLimitRank[j] = int.Parse(s1[1]);
				}
				// add data to dictionary
				shipData.Add (data[i].type, data[i]);
			}
		} else
			Destroy(gameObject);
	}
}