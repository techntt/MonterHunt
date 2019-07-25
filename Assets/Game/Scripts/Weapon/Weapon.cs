using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contain data for weapons that can fire
/// </summary>
[System.Serializable]
public class FiringWeaponData {
	public float fireRate;
	public int maxRays;
	public float bulletSpeed;
}

public class Weapon : MonoBehaviour {

	public WeaponType weaponType;
	protected WeaponCategory cat;
	public AudioSource soundSource;

	public bool isCompatible;

	float duration;

	public virtual void Start () {
	}

	public virtual void Activate () {
		gameObject.SetActive(true);
		if (duration > 0) {
			Invoke("Expire", duration);
		}
	}

	public virtual void Init () {
		cat = WeaponManager.GetWeaponCatergory(weaponType);
		duration = WeaponManager.GetBonusDuration(weaponType);
	}

	/// <summary>
	/// Call when this weapon expire normally (time out)
	/// </summary>
	public virtual void Expire () {
		gameObject.SetActive(false);
		WeaponManager.Instance.OnWeaponExpire(weaponType);
	}

	/// <summary>
	/// Call when this weapon is forced to expire
	/// </summary>
	public virtual void Rewind () {
		CancelInvoke("Expire");
		Invoke("Expire", duration);
	}

	public virtual void Remove () {
		CancelInvoke("Expire");
		gameObject.SetActive(false);
	}

	/// <summary>
	/// Chek if newly added weapon is compatible with this weapon.
	/// if not, remove this weapon
	/// </summary>
	/// <param name="type">Type of new weapon.</param>
	public void CheckNewWeapon (WeaponType type) {
		isCompatible = true;
		WeaponCategory wc = WeaponManager.GetWeaponCatergory(type);
		if (cat == wc) {
			if (cat == WeaponCategory.MAIN || cat == WeaponCategory.DEFENSE) {
				isCompatible = false;
			}
		}
	}

    
}