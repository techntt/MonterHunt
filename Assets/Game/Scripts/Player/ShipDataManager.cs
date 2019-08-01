using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipDataManager : SingletonMonoBehaviour<ShipDataManager> {
    [HideInInspector]
	public Dictionary<int, ShipData> shipData;

	[SerializeField]
	List<ShipData> data;

	void Awake () {
        DontDestroyOnLoad(gameObject);        
    }

    public void InitData()
    {
        shipData = new Dictionary<int, ShipData>();
        List<Dictionary<string, string>> temp = CSVReader.ReadDataToList(DataManager.Instance.ship);
        if (temp == null)
        {
            Debug.Log("[ShipDataManager] temp data is null");
            return;
        }
        for (int i = 0; i < data.Count; i++)
        {
            data[i].id = int.Parse(temp[i]["id"]);
            data[i].shipName = temp[i]["name"];
            data[i].campaignPassed = int.Parse(temp[i]["campaign"]);
            data[i].baseDamage = float.Parse(temp[i]["damage"]);
            data[i].crystal = int.Parse(temp[i]["crystal"]);
            string strSpeed = temp[i]["speed"];
            string[] speedData = strSpeed.Trim().Split(new char[] { '_' });
            data[i].minSpeed = float.Parse(speedData[0].Trim());
            data[i].maxSpeed = float.Parse(speedData[1].Trim());
            string strSkill = temp[i]["special"];
            string[] skillData = strSkill.Split(new char[] {'_'});
            data[i].baseSkillDamage = float.Parse(skillData[0].Trim());
            data[i].timeDelay = float.Parse(skillData[1].Trim());
            // add data to dictionary
            shipData.Add(data[i].id, data[i]);
        }
        Debug.Log("[ShipDataManager] shipData inited ("+shipData.Count+")");
    }
}