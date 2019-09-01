using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampaignManager {

	public const int maxId = 5;

	public static Campaign campaign;

	public static void ReadData () {
		List<Dictionary<string, string>> data = CSVReader.ReadDataToList(DataManager.Instance.campaign);
		campaign = new Campaign();
		// get campaign's id and name
		int i = Mathf.Clamp(PlayerData.Instance.currentMission, 0, maxId);
		campaign.id = i;
		campaign.name = data[i]["name"];	
		campaign.bossID = int.Parse(data[i]["boss"]);
		campaign.bossHp = int.Parse(data[i]["bossHp"]);
	}
}

public class Campaign {
	public int id;
	public string name;
	public int bossID;
	public int bossHp;
}