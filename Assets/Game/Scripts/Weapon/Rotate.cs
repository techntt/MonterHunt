using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : Weapon {

	float lastFireTime;
	float rotateSpeed;
	float currentAngle;
	float deltaAngle;
	Transform firePos;
	float maxOpenAngle;
	float damagePerBullet;
	Color bulletColor;

	public FiringWeaponData firingData;

	public override void Init () {
		base.Init();
		firePos = GameManager.Instance.player.firePos;
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
		currentAngle = 0;
		WeaponDataCalculator wdc = WeaponDataCalculator.Instance;
		switch (WeaponManager.Instance.lastValue) {
			case 0:
				firingData.bulletSpeed = WeaponDataCalculator.minBulletSpeed;
				firingData.fireRate = WeaponDataCalculator.superFireRate;
				firingData.maxRays = 1;
				damagePerBullet = wdc.DPS(WeaponDataCalculator.MAX_WEAPON_LEVEL) * GameManager.Instance.player.baseDamage * WeaponDataCalculator.ROTATE_CYAN * firingData.fireRate / firingData.maxRays;
				maxOpenAngle = 0;
				rotateSpeed = 360;
				bulletColor = Color.cyan;
				break;
			case 1:
				firingData.bulletSpeed = WeaponDataCalculator.maxBulletSpeed;
				firingData.fireRate = 0.02f;
				firingData.maxRays = 2;
				damagePerBullet = wdc.DPS(WeaponDataCalculator.MAX_WEAPON_LEVEL) * GameManager.Instance.player.baseDamage * WeaponDataCalculator.ROTATE_YELLOW * firingData.fireRate / firingData.maxRays;
				maxOpenAngle = 30;
				rotateSpeed = 540;
				bulletColor = Color.yellow;
				break;
			case 2:
				firingData.bulletSpeed = WeaponDataCalculator.superBulletSpeed;
				firingData.fireRate = 0.015f;
				firingData.maxRays = 3;
				damagePerBullet = wdc.DPS(WeaponDataCalculator.MAX_WEAPON_LEVEL) * GameManager.Instance.player.baseDamage * WeaponDataCalculator.ROTATE_RED * firingData.fireRate / firingData.maxRays;
				maxOpenAngle = 90;
				rotateSpeed = 720;
				bulletColor = Color.red;
				break;
		}
		if (firingData.maxRays > 1)
			deltaAngle = maxOpenAngle / (firingData.maxRays - 1);
		else
			deltaAngle = 0;
	}

	public void SpawnBullet(Vector3 firePos) {
		for (int i = 0; i < firingData.maxRays; i++) {
			Bullet b = BulletManager.Instance.PopBullet(BulletType.Rotate);
			b.transform.position = firePos;
			b.transform.rotation = Quaternion.Euler(0,0, currentAngle + i * deltaAngle);
			b.Init(firingData.bulletSpeed, bulletColor, damagePerBullet);
		}
	}

	void Update () {
		if (Time.time - lastFireTime > firingData.fireRate) {
			lastFireTime = Time.time;
			SpawnBullet(firePos.position);
		}
		currentAngle += rotateSpeed * Time.deltaTime;
	}
}