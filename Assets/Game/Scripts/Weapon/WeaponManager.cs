using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : SingletonMonoBehaviour<WeaponManager> {
	public Weapon[] weapons;

	public SortedList<WeaponType, Weapon> weaponList = new SortedList<WeaponType, Weapon>();
	List<Weapon> activatingWeapons;

	[HideInInspector] public int lastValue;

	public AudioClip sfxMissle;

	void Start () {
		for (int i = 0; i < weapons.Length; i++) {
			weapons[i].Init();
			weaponList.Add(weapons[i].weaponType, weapons[i]);
		}
		activatingWeapons = new List<Weapon>();
		GameEventManager.Instance.GameStart += HandleOnGameStart;
		GameEventManager.Instance.PlayerDead += HandlePlayerDead;
		GameEventManager.Instance.PlayerGetBonus += HandlePlayerGetBonus;
	}

	void HandlePlayerGetBonus (Player p, BonusType b, int variant) {
		switch (b) {
			case BonusType.PowerUp:
				return;
			case BonusType.Bomb:
				lastValue = 0;
				break;
			case BonusType.Rocket:
				SoundManager.Instance.PlaySfx(sfxMissle, SFX_PLAY_STYLE.DONT_REWIND);
				lastValue = 1;
				break;
			case BonusType.Bloom:
				lastValue = 2;
				break;
			case BonusType.Seek:
				SoundManager.Instance.PlaySfx(sfxMissle, SFX_PLAY_STYLE.DONT_REWIND);
				lastValue = 3;
				break;
			case BonusType.Lightning:
				lastValue = 4;
				break;
			case BonusType.Divide30:
				lastValue = 0;
				break;
			case BonusType.Divide180:
				lastValue = 1;
				break;
			case BonusType.Divide360:
				lastValue = 2;
				break;
			default:
				lastValue = variant;
				break;
		}
		ActivateWeapon(BonusTypeToWeaponType(b));
	}

	void HandlePlayerDead () {
		DeactivateAllWeapons();
	}

	void HandleOnGameStart () {
		ActivateWeapon(WeaponType.BASE_WEAPON);
	}

	public void ActivateWeapon (WeaponType type) {
		if (weaponList.ContainsKey(type)) {
			if (type == WeaponType.ONE_SHOT) {
				weaponList[type].Activate();
			} else {
				// if the weapon is currently active, just rewind it
				for (int i = 0; i < activatingWeapons.Count; i++) {
					if (type == activatingWeapons[i].weaponType) {
						activatingWeapons[i].Rewind();
						return;
					}
				}
				// check if current weapons are compatible with the new weapon
				for (int i = 0; i < activatingWeapons.Count; i++) {
					activatingWeapons[i].CheckNewWeapon(type);
				}
				// remove incompatible weapons
				for (int i = activatingWeapons.Count - 1; i >= 0; i--) {
					if (!activatingWeapons[i].isCompatible) {
						DeactivateWeapon(activatingWeapons[i]);
					}
				}
				// activate the weapon
				Weapon w = weaponList[type];
				w.Activate();
				activatingWeapons.Add(w);
			}
		}
	}

	public void DeactivateWeapon (Weapon weapon) {
		weapon.Remove();
		activatingWeapons.Remove(weapon);
	}

	public void DeactivateAllWeapons () {
		foreach (Weapon w in weaponList.Values) {
			w.Remove();
		}
		activatingWeapons.Clear();
	}

	public void OnWeaponExpire (WeaponType type) {
		// if the weapon is among main category, restore ship's base weapon
		if (GetWeaponCatergory(type) == WeaponCategory.MAIN) {
			ActivateWeapon(WeaponType.BASE_WEAPON);
		}
		activatingWeapons.Remove(weaponList[type]);
	}

	public static float GetBonusDuration (WeaponType type) {
		switch (type) {
			case WeaponType.DIVIDED_RAY:
			case WeaponType.LASER:
			case WeaponType.MAX_WEAPON:
			case WeaponType.ROTATE:
			case WeaponType.SLOW_WEAPON:
				return 6;
			case WeaponType.SAW:
			case WeaponType.SHIELD:
			case WeaponType.X_POINT:
			case WeaponType.X_DAMAGE:
			case WeaponType.MAGNET:
				return 8;
			case WeaponType.ONE_SHOT:
			case WeaponType.BASE_WEAPON:
				return 0;
			default:
				return 0;
		}
	}

	public static WeaponType BonusTypeToWeaponType (BonusType t) {
		switch (t) {
			case BonusType.Bloom:
			case BonusType.Rocket:
			case BonusType.Bomb:
			case BonusType.Lightning:
			case BonusType.Seek:
				return WeaponType.ONE_SHOT;
			case BonusType.Laser:
				return WeaponType.LASER;
			case BonusType.Rotate:
				return WeaponType.ROTATE;
			case BonusType.Saw:
				return WeaponType.SAW;
			case BonusType.Shield:
				return WeaponType.SHIELD;
			case BonusType.Slow:
				return WeaponType.SLOW_WEAPON;
			case BonusType.Divide30:
			case BonusType.Divide180:
			case BonusType.Divide360:
				return WeaponType.DIVIDED_RAY;
			case BonusType.SuperWeapon:
				return WeaponType.MAX_WEAPON;
			case BonusType.XDam:
				return WeaponType.X_DAMAGE;
			case BonusType.XPoint:
				return WeaponType.X_POINT;
			case BonusType.Magnet:
				return WeaponType.MAGNET;
			default:
				return WeaponType.NONE;
		}
	}

	public static WeaponCategory GetWeaponCatergory (WeaponType type) {
		switch (type) {
			case WeaponType.DIVIDED_RAY:
			case WeaponType.LASER:
			case WeaponType.BASE_WEAPON:
				return WeaponCategory.MAIN;
			case WeaponType.ONE_SHOT:
			case WeaponType.ROTATE:
			case WeaponType.SLOW_WEAPON:
			case WeaponType.X_DAMAGE:
			case WeaponType.X_POINT:
			case WeaponType.MAGNET:
			case WeaponType.MAX_WEAPON:
				return WeaponCategory.SUPPORT;
			case WeaponType.SHIELD:
			case WeaponType.SAW:
				return WeaponCategory.DEFENSE;
			default:
				return WeaponCategory.NONE;
		}
	}

	void Update () {
		if (Input.GetKeyDown(KeyCode.S)) {
			ActivateWeapon(WeaponType.SAW);
		}
		if (Input.GetKeyDown(KeyCode.H)) {
			ActivateWeapon(WeaponType.SHIELD);
		}
		if (Input.GetKeyDown(KeyCode.R)) {
			lastValue = Random.Range(0,3);
			ActivateWeapon(WeaponType.ROTATE);
		}
		if (Input.GetKeyDown(KeyCode.D)) {
			lastValue = Random.Range(0,3);
			ActivateWeapon(WeaponType.DIVIDED_RAY);
		}
		if (Input.GetKeyDown(KeyCode.L)) {
			lastValue = Random.Range(0,3);
			ActivateWeapon(WeaponType.LASER);
		}
		if (Input.GetKeyDown(KeyCode.X)) {
			lastValue = Random.Range(0,3);
			ActivateWeapon(WeaponType.X_DAMAGE);
		}
		if (Input.GetKeyDown(KeyCode.P)) {
			lastValue = Random.Range(0,3);
			ActivateWeapon(WeaponType.X_POINT);
		}
		if (Input.GetKeyDown(KeyCode.M)) {
			ActivateWeapon(WeaponType.MAX_WEAPON);
		}
		if (Input.GetKeyDown(KeyCode.F)) {
			ActivateWeapon(WeaponType.SLOW_WEAPON);
		}
		if (Input.GetKeyDown(KeyCode.A)) {
			ActivateWeapon(WeaponType.MAGNET);
		}
		if (Input.GetKeyDown(KeyCode.Alpha0)) {
			lastValue = 0;
			ActivateWeapon(WeaponType.ONE_SHOT);
		}
		if (Input.GetKeyDown(KeyCode.Alpha1)) {
			lastValue = 1;
			ActivateWeapon(WeaponType.ONE_SHOT);
		}
		if (Input.GetKeyDown(KeyCode.Alpha2)) {
			lastValue = 2;
			ActivateWeapon(WeaponType.ONE_SHOT);
		}
		if (Input.GetKeyDown(KeyCode.Alpha3)) {
			lastValue = 3;
			ActivateWeapon(WeaponType.ONE_SHOT);
		}
		if (Input.GetKeyDown(KeyCode.Alpha4)) {
			lastValue = 4;
			ActivateWeapon(WeaponType.ONE_SHOT);
		}
	}
}

public enum WeaponType {
	ROTATE,
	DIVIDED_RAY,
	BASE_WEAPON,
	LASER,
	SAW,
	SHIELD,
	X_DAMAGE,
	X_POINT,
	/// <summary>
	/// change base weapon to max lv and x10 damage
	/// </summary>
	MAX_WEAPON,
	SLOW_WEAPON,
	ONE_SHOT,
	MAGNET,
	NONE
}

public enum WeaponCategory {
	MAIN,
	DEFENSE,
	SUPPORT,
	NONE
}