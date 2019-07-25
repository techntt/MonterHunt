using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonusStatusBar : MonoBehaviour {

	public BonusStatusItem sample;
	public Stack<BonusStatusItem> pool;
	public SortedList<BonusType, BonusStatusItem> activeItem;

	void Start () {
		pool = new Stack<BonusStatusItem>();
		pool.Push(sample);
		activeItem = new SortedList<BonusType, BonusStatusItem>();
		GameEventManager.Instance.PlayerDead += HandlePlayerDead;
		GameEventManager.Instance.PlayerGetBonus += HandlePlayerGetBonus;
	}

	void HandlePlayerGetBonus (Player p, BonusType b, int variant = 0) {
		float time = WeaponManager.GetBonusDuration(WeaponManager.BonusTypeToWeaponType(b));
		if (time > 0) {
			BonusStatusItem item = GetItem(b);
			item.Init(b, time);
		}
	}

	void HandlePlayerDead () {
		foreach (BonusStatusItem item in activeItem.Values) {
			item.ForcePush();
		}
		activeItem.Clear();
	}

	public void PushItem (BonusStatusItem item) {
		activeItem.Remove(item.currentBonus);
		pool.Push(item);
	}

	public BonusStatusItem GetItem (BonusType t) {
		if (activeItem.ContainsKey(t))
			return activeItem[t];
		else {
			BonusStatusItem item = null;
			if (pool.Count > 0)
				item = pool.Pop();
			else {
				item = Instantiate(sample) as BonusStatusItem;
				item.transform.SetParent(transform, false);
				item.myBar = this;
			}
			activeItem.Add(t, item);
			return item;
		}
	}
}