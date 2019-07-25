using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XDamage : Weapon {

	public override void Activate () {
		base.Activate();
		switch (WeaponManager.Instance.lastValue) {
			case 0:
				GameManager.Instance.player1.xDamage = WeaponDataCalculator.POWER_X_2;
				break;
			case 1:
				GameManager.Instance.player1.xDamage = WeaponDataCalculator.POWER_X_3;
				break;
			case 2:
				GameManager.Instance.player1.xDamage = WeaponDataCalculator.POWER_X_4;
				break;
		}
	}

	public override void Expire () {
		base.Expire();
		GameManager.Instance.player1.xDamage = 0;
	}

	public override void Remove () {
		base.Remove();
		GameManager.Instance.player1.xDamage = 0;
	}
}