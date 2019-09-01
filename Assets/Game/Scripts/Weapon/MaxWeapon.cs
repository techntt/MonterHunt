using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaxWeapon : Weapon {

	public ShipBaseWeapon main;

	public override void Init () {
		base.Init();
		main = (ShipBaseWeapon)GameManager.Instance.player.myBaseWeapon;
	}

	public override void Activate () {
		base.Activate();
		main.WeaponDataChanged += HandleWeaponDataChanged;
		HandleWeaponDataChanged();
	}

	public override void Expire () {
		base.Expire();
		main.WeaponDataChanged -= HandleWeaponDataChanged;
		main.GetWeaponData(GameManager.Instance.player.power);
	}

	void HandleWeaponDataChanged () {
		main.GetWeaponData(WeaponDataCalculator.MAX_WEAPON_LEVEL);
	}

	public override void Remove () {
		base.Remove();
		main.WeaponDataChanged -= HandleWeaponDataChanged;
		main.GetWeaponData(GameManager.Instance.player.power);
	}
}

public interface ShipBaseWeapon {
	event BaseWeapon.OnShooting WeaponDataChanged;
	void GetWeaponData (int level);
}