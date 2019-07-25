using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Mapping collider's instance ID to desired class
/// </summary>
public class ColliderRef : SingletonMonoBehaviour<ColliderRef> {

	public SortedList<int, Bullet> bulletRef;
	public SortedList<int, Bonus> bonusRef;
	public SortedList<int, Damageable> DamageableRef;

	public ColliderRef () {
		bulletRef = new SortedList<int, Bullet>();
		bonusRef = new SortedList<int, Bonus>();
		DamageableRef = new SortedList<int, Damageable>();
	}

	public Bullet GetBullet (int id) {
		if (bulletRef.ContainsKey(id))
			return bulletRef[id];
		else
			return null;
	}

	public Bonus GetBonus (int id) {
		if (bonusRef.ContainsKey(id))
			return bonusRef[id];
		else
			return null;
	}

	public Damageable GetDamageable (int id) {
		if (DamageableRef.ContainsKey(id))
			return DamageableRef[id];
		else
			return null;
	}

	void OnDestroy () {
		bulletRef.Clear();
		bonusRef.Clear();
		DamageableRef.Clear();
	}
}