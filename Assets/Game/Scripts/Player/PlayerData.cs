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

	public SHIP_TYPE selectedShip;
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

	public SortedList<SHIP_TYPE, ShipUpgradeData> shipData;

	public int currentMission;
	public int bestScore;
	/// <summary>
	/// number of times player plays this campaign
	/// </summary>
	public int retryTimes;

	public PlayerData () {
		selectedShip = (SHIP_TYPE)PlayerPrefs.GetInt(Const.SELECTED_SHIP, 0);
		gold = PlayerPrefs.GetInt(Const.GOLD, 0);
		rank = PlayerPrefs.GetInt(Const.RANK, 0);
//		gold = 696969;
//		rank = 1069;
		currentMission = PlayerPrefs.GetInt(Const.MISSION, 0);
		retryTimes = PlayerPrefs.GetInt(Const.RETRY, 0);
		bestScore = PlayerPrefs.GetInt(Const.BEST_SCORE, 0);
		shipData = new SortedList<SHIP_TYPE, ShipUpgradeData>();
		for (int i = 0; i < (int)SHIP_TYPE.NONE; i++) {
			string result = PlayerPrefs.GetString(((SHIP_TYPE)i).ToString(), "");
			ShipUpgradeData sud = null;
			if (result != "")
				sud = JsonUtility.FromJson<ShipUpgradeData>(result);
			else
				sud = new ShipUpgradeData();
			shipData.Add((SHIP_TYPE)i, sud);
		}
		shipData[SHIP_TYPE.STING].unlocked = true;
	}

	public void SaveAllData () {
		PlayerPrefs.SetInt(Const.SELECTED_SHIP, (int)selectedShip);
		PlayerPrefs.SetInt(Const.GOLD, gold);
		PlayerPrefs.SetInt(Const.RANK, rank);
		PlayerPrefs.SetInt(Const.MISSION, currentMission);
		PlayerPrefs.SetInt(Const.RETRY, retryTimes);
		PlayerPrefs.SetInt(Const.BEST_SCORE, bestScore);
		for (int i = 0; i < shipData.Count; i++) {
			SaveShipData((SHIP_TYPE)i);
		}
	}

	public void SaveShipData (SHIP_TYPE ship) {
		PlayerPrefs.SetString(ship.ToString(), JsonUtility.ToJson(shipData[ship]));
	}

	public SHIP_TYPE GetHighestShip () {
		for (int i = (int)SHIP_TYPE.NONE - 1; i >= 0; i--)
			if (shipData[(SHIP_TYPE)i].unlocked)
				return (SHIP_TYPE)i;
		return SHIP_TYPE.STING;
	}
}