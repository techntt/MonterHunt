using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DivideWeapon : Weapon {

	float lastFireTime;
	Transform firePos;
	float maxOpenAngle;
	float deltaAngle;
	float damagePerBullet;
	BulletType bulletType;
	public FiringWeaponData firingData;

	public override void Init () {
		base.Init();
		firePos = GameManager.Instance.player1.firePos;
	}

	public override void Activate () {
		base.Activate();
		GetWeaponData();
		lastFireTime = Time.time;
	}

	public override void Rewind () {
		base.Rewind();
		GetWeaponData();
	}

	void GetWeaponData () {
		WeaponDataCalculator wdc = WeaponDataCalculator.Instance;
		firingData.bulletSpeed = WeaponDataCalculator.superBulletSpeed;
		firingData.fireRate = WeaponDataCalculator.superFireRate;
		switch (WeaponManager.Instance.lastValue) {
			case 0:
				firingData.maxRays = 5;
				maxOpenAngle = 30;
				bulletType = BulletType.Divide30;
				damagePerBullet = wdc.DPS(WeaponDataCalculator.MAX_WEAPON_LEVEL) * GameManager.Instance.player1.baseDamage * WeaponDataCalculator.DIVIDE_30 * firingData.fireRate / firingData.maxRays;
				break;
			case 1:
				firingData.maxRays = 10;
				maxOpenAngle = 180;
				bulletType = BulletType.Divide180;
				damagePerBullet = wdc.DPS(WeaponDataCalculator.MAX_WEAPON_LEVEL) * GameManager.Instance.player1.baseDamage * WeaponDataCalculator.DIVIDE_180 * firingData.fireRate / firingData.maxRays;
				break;
			case 2:
				firingData.maxRays = 20;
				maxOpenAngle = 360;
				bulletType = BulletType.Divide360;
				damagePerBullet = wdc.DPS(WeaponDataCalculator.MAX_WEAPON_LEVEL) * GameManager.Instance.player1.baseDamage * WeaponDataCalculator.DIVIDE_360 * firingData.fireRate / firingData.maxRays;
				break;
		}
		if (maxOpenAngle == 360)
			deltaAngle = maxOpenAngle / firingData.maxRays;
		else
			deltaAngle = maxOpenAngle / (firingData.maxRays - 1);
	}

	public void SpawnBullet (Vector3 firePos) {
		float limitAngle = 90 - maxOpenAngle / 2;
		if (maxOpenAngle == 360) {
			for (int i = 0; i < firingData.maxRays; i++) {
				Bullet b = BulletManager.Instance.PopBullet(bulletType);
				b.transform.position = firePos;
				b.transform.rotation = Quaternion.Euler(0,0, 90 + i * deltaAngle);
				b.Init(firingData.bulletSpeed, Color.white, damagePerBullet);
			}
		} else {
			for (int i = 0; i < firingData.maxRays; i++) {
				Bullet b = BulletManager.Instance.PopBullet(bulletType);
				b.transform.position = firePos;
				b.transform.rotation = Quaternion.Euler(0,0, limitAngle + i * deltaAngle);
				b.Init(firingData.bulletSpeed, Color.white, damagePerBullet);
			}
		}
	}

	void Update () {
		if (Time.time - lastFireTime > firingData.fireRate) {
			lastFireTime = Time.time;
			SpawnBullet(firePos.position);
		}
	}
}