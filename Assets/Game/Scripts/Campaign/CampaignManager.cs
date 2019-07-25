using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampaignManager {

	public const int maxId = 5;

	public static Campaign campaign;

	public static void ReadData () {
		List<Dictionary<string, string>> data = CSVReader.Read (Const.CAMPAIGN_DATA);
		campaign = new Campaign();
		// get campaign's id and name
		int i = Mathf.Clamp(PlayerData.Instance.currentMission, 0, maxId);
		campaign.id = i;
		campaign.name = data[i]["name"];
		campaign.objective = int.Parse(data[i]["objective"]);
		char[] se = new char[]{'_'};
		string[] temp = data[i]["phase1"].Split(se, System.StringSplitOptions.RemoveEmptyEntries);
		for (int j = 0; j < temp.Length; j++) {
			campaign.phase1[j] = int.Parse(temp[j]);
		}
		temp = data[i]["phase2"].Split(se, System.StringSplitOptions.RemoveEmptyEntries);
		for (int j = 0; j < temp.Length; j++) {
			campaign.phase2[j] = int.Parse(temp[j]);
		}
		temp = data[i]["phase3"].Split(se, System.StringSplitOptions.RemoveEmptyEntries);
		for (int j = 0; j < temp.Length; j++) {
			campaign.phase3[j] = int.Parse(temp[j]);
		}
		campaign.form1ID = int.Parse(data[i]["form1"]);
		campaign.form2ID = int.Parse(data[i]["form2"]);
		campaign.bossID = int.Parse(data[i]["boss"]);
		campaign.bossHp = int.Parse(data[i]["bossHp"]);
	}
}

public class Campaign {

	public int id;
	public string name;
	public int objective;
	public int[] phase1 = new int[2];
	public int[] phase2 = new int[2];
	public int[] phase3 = new int[2];
	public int form1ID, form2ID;
	public int bossID;
	public int bossHp;

}