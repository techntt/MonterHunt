using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropOnDeath : MonoBehaviour {

	public DummyObject target;

	public bool dropCoin;
	public DROP_TYPE coinDropType;
	public int minCoin, maxCoin;
	public bool dropBonus;
	public DROP_TYPE bonusDropType;
	public float bonusDropChance;
	public BonusType bonusType;

	public enum DROP_TYPE {
		RANDOM,
		FIX_VALUE,
		BASE_ON_LEVEL
	}

	void Reset () {
		target = GetComponent(typeof(DummyObject)) as DummyObject;
	}

	void Start () {
		target.ObjectDie += OnDummyDie;
	}

	void OnDummyDie (DummyObject d) {
		if (dropCoin) {
			int numOfCoin = 0;
			if (coinDropType == DROP_TYPE.RANDOM) {
				numOfCoin = Random.Range(minCoin, maxCoin + 1);
			} else if (coinDropType == DROP_TYPE.FIX_VALUE) {
				numOfCoin = minCoin;
			} else if (coinDropType == DROP_TYPE.BASE_ON_LEVEL) {
				numOfCoin = CircleSpawner.GetNumberOfCoinBySize(CircleSpawner.Instance.GetRandomSize());
			}
			for (int i = 0; i < numOfCoin; i++) {
				Coin c = CoinManager.Instance.PopCoin();
				c.transform.position = transform.position;
				c.Init();
			}
		}
		if (dropBonus) {
			float dropChance = bonusDropChance;
			BonusType type = bonusType;
			if (bonusDropType == DROP_TYPE.RANDOM) {
				type = BonusManager.Instance.GetRandomBonusType();
			} else if (bonusDropType == DROP_TYPE.BASE_ON_LEVEL) {
				dropChance = CircleSpawner.Instance.bonusDropChance;
				type = BonusManager.Instance.GetRandomBonusType();
			}
			if (Random.Range(0, 100) < dropChance) {
				Bonus b = BonusManager.Instance.GetBonus(type);
				if (b != null) {
					b.transform.position = transform.position;
					b.Init();
				}
			}
		}
	}
}