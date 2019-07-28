using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipUpgradeData {

	public int damageLv;
	public int hpLv;
	public int hpLimitLv;
	public int magnetLv;
	public bool unlocked;

	public ShipUpgradeData(){
		damageLv = 0;
		hpLv = 0;
		hpLimitLv = 0;
		magnetLv = 0;
		unlocked = false;
	}

	public override string ToString () {
		return string.Format(
			"damage level: {0}\nhp level: {1}\nmax hp level: {2}\nmagnet level: {3}", new object[]{damageLv, hpLv, hpLimitLv, magnetLv});
	}
}

public class PlayerData : Singleton<PlayerData> {

	public int selectedShip;
	private int _gold;
	public int gold {
		get { 
			return _gold;
		}
		set { 
			_gold = value;
			if (OnGoldChange != null)
				OnGoldChange(_gold);
		}
	}
	public delegate void GoldEvent (int gold);
	public event GoldEvent OnGoldChange;

	private int _rank;
	public int rank {
		get { 
			return _rank;
		}
		set { 
			_rank = value;
			if (OnRankChange != null)
				OnRankChange(_rank);
		}
	}
	public event GoldEvent OnRankChange;

	public SortedList<int, ShipUpgradeData> shipData;

	public int currentMission;
	public int bestScore;
	/// <summary>
	/// number of times player plays this campaign
	/// </summary>
	public int retryTimes;

	public PlayerData () {
		selectedShip = PlayerPrefs.GetInt(Const.SELECTED_SHIP, 0);
		gold = PlayerPrefs.GetInt(Const.GOLD, 0);
		rank = PlayerPrefs.GetInt(Const.RANK, 0);

		currentMission = PlayerPrefs.GetInt(Const.MISSION, 0);
		retryTimes = PlayerPrefs.GetInt(Const.RETRY, 0);
		bestScore = PlayerPrefs.GetInt(Const.BEST_SCORE, 0);
		shipData = new SortedList<int, ShipUpgradeData>();
        Debug.Log("Ship number: "+ ShipDataManager.Instance.shipData.Count);
		for (int i = 0; i < ShipDataManager.Instance.shipData.Count; i++) {
			string result = PlayerPrefs.GetString((i).ToString(), "");
			ShipUpgradeData sud = null;
			if (result != "")
				sud = JsonUtility.FromJson<ShipUpgradeData>(result);
			else
				sud = new ShipUpgradeData();
			shipData.Add(i, sud);
		}
		shipData[0].unlocked = true;
	}

	public void SaveAllData () {
		PlayerPrefs.SetInt(Const.SELECTED_SHIP, (int)selectedShip);
		PlayerPrefs.SetInt(Const.GOLD, gold);
		PlayerPrefs.SetInt(Const.RANK, rank);
		PlayerPrefs.SetInt(Const.MISSION, currentMission);
		PlayerPrefs.SetInt(Const.RETRY, retryTimes);
		PlayerPrefs.SetInt(Const.BEST_SCORE, bestScore);
		for (int i = 0; i < shipData.Count; i++) {
			SaveShipData(i);
		}
	}

	public void SaveShipData (int ship) {
		PlayerPrefs.SetString(ship.ToString(), JsonUtility.ToJson(shipData[ship]));
	}

	public int GetHighestShip () {
		for (int i = (ShipDataManager.Instance.shipData.Count - 1); i >= 0; i--)
			if (shipData[i].unlocked)
				return i;
		return 0;
	}
}