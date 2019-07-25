using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// List of ship type. Add new ship type before NONE.
/// </summary>
public enum SHIP_TYPE {
	STING,
	SWALLOW,
	PINCER,
	NONE
}
[System.Serializable]
public class ShipData {
	public Sprite sprite;
	public SHIP_TYPE type;
	public float baseDamage;
	public int baseScore;
	public string shipName;
	public int campaignPassed;
	public int gold;
	public int rank;
	public int[] damageUpgradeLimitRank;
	public int[] damageUpgradeLimitLevel;
	public int[] hpUpgradeLimitRank;
	public int[] hpUpgradeLimitLevel;
	public int[] maxHpUpgradeLimitRank;
	public int[] maxHpUpgradeLimitLevel;
	public int[] magnetUpgradeLimitRank;
	public int[] magnetUpgradeLimitLevel;

	public int GetRequiredRankForDamage (int currentLevel) {
		for (int i = 0; i < damageUpgradeLimitLevel.Length; i++) {
			if (currentLevel + 1 == damageUpgradeLimitLevel[i]) {
				return damageUpgradeLimitRank[i];
			}
		}
		return 0;
	}

	public int GetRequiredRankForHp (int currentLevel) {
		for (int i = 0; i < hpUpgradeLimitLevel.Length; i++) {
			if (currentLevel + 1 == hpUpgradeLimitLevel[i]) {
				return hpUpgradeLimitRank[i];
			}
		}
		return 0;
	}

	public int GetRequiredRankForMaxHp (int currentLevel) {
		for (int i = 0; i < maxHpUpgradeLimitLevel.Length; i++) {
			if (currentLevel + 1 == maxHpUpgradeLimitLevel[i]) {
				return maxHpUpgradeLimitRank[i];
			}
		}
		return 0;
	}

	public int GetRequiredRankForMagnet (int currentLevel) {
		for (int i = 0; i < magnetUpgradeLimitLevel.Length; i++) {
			if (currentLevel + 1 == magnetUpgradeLimitLevel[i]) {
				return magnetUpgradeLimitRank[i];
			}
		}
		return 0;
	}
}