using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XPoint : Weapon {

	public override void Activate () {
		base.Activate();
		switch (WeaponManager.Instance.lastValue) {
			case 0:
				GameManager.Instance.player.xPoint = WeaponDataCalculator.POINT_X2;
				break;
			case 1:
				GameManager.Instance.player.xPoint = WeaponDataCalculator.POINT_X3;
				break;
			case 2:
				GameManager.Instance.player.xPoint = WeaponDataCalculator.POINT_X4;
				break;
		}
	}

	public override void Expire () {
		base.Expire();
		GameManager.Instance.player.xPoint = 1;
	}

	public override void Remove () {
		base.Remove();
		GameManager.Instance.player.xPoint = 1;
	}
}
