using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowWeapon : Weapon {

	Transform firePos;
	float lastFireTime;
	public FiringWeaponData firingData;

	public override void Init () {
		base.Init();
		firePos = GameManager.Instance.player1.firePos;
		firingData.bulletSpeed = WeaponDataCalculator.maxBulletSpeed;
		firingData.fireRate = WeaponDataCalculator.minFireRate;
	}

	public override void Activate () {
		base.Activate();
		lastFireTime = Time.time;
	}

	public void SpawnBullet(Vector3 firePos) {
		Bullet b = BulletManager.Instance.PopBullet(BulletType.Slow);
		b.transform.position = firePos;
		b.transform.rotation = Quaternion.Euler(0,0, 90);
		b.Init(firingData.bulletSpeed, Color.white, 0);
	}

	void Update () {
		if (Time.time - lastFireTime > firingData.fireRate) {
			lastFireTime = Time.time;
			SpawnBullet(firePos.position);
		}
	}
}