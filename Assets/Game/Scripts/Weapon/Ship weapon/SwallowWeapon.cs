using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwallowWeapon : Weapon, ShipBaseWeapon {

	/// <summary>
	/// The max open angle at max level
	/// </summary>
	float limitOpenAngle = 25;
	/// <summary>
	/// The last time this weapon fires.
	/// </summary>
	float lastFireTime;

	public event BaseWeapon.OnShooting WeaponDataChanged;

	Transform firePos;
	float maxOpenAngle;
	float damagePerBullet;
	float damage;
	float deltaAngle;
	float startAngle;

	public FiringWeaponData firingData;

	public Player myPlayer;

	public override void Start () {
		base.Start();
		firePos = myPlayer.firePos;
		if (PopupManager.Instance.scene == SCENE.GAME) {
			myPlayer.xDamageChange += HandleXDamageChange;
			GameEventManager.Instance.PlayerHealthChanged += HandleHealthChange;
		}
	}

	void HandleHealthChange (Player p, float hpChange) {
		GetWeaponData(p.power);
		if (WeaponDataChanged != null)
			WeaponDataChanged();
	}

	void HandleXDamageChange () {
		damage = damagePerBullet * myPlayer.baseDamage;
		if (WeaponDataChanged != null)
			WeaponDataChanged();
	}

	public override void Activate () {
		base.Activate();
		GetWeaponData(myPlayer.power);
		lastFireTime = Time.time;
	}

	public void GetWeaponData (int level) {
		WeaponDataCalculator wdc = WeaponDataCalculator.Instance;
		firingData.bulletSpeed = wdc.Speed(level);
		firingData.fireRate = wdc.FireRate(level);
		firingData.maxRays = wdc.Rays(level);
		maxOpenAngle = limitOpenAngle * (firingData.maxRays - 1) / (WeaponDataCalculator.maxNumOfBullet - 1);
		damagePerBullet = WeaponDataCalculator.Instance.CalculateBaseDamagePerBullet(level);
		damage = damagePerBullet * myPlayer.baseDamage;
		if (firingData.maxRays == 1)
			deltaAngle = 0;
		else
			deltaAngle = maxOpenAngle / (firingData.maxRays - 1);
		startAngle = 90 - maxOpenAngle / 2;
	}

	void Update () {
		if (Time.time - lastFireTime > firingData.fireRate) {
			lastFireTime = Time.time;
			Shoot();
		}
	}

	void Shoot () {
		if (!soundSource.isPlaying)
			soundSource.Play();
		Color c = myPlayer.bulletColor;
		for (int i = 0; i < firingData.maxRays; i++) {
			Bullet shot = BulletManager.Instance.PopBullet(BulletType.Normal, myPlayer.myBullet);
			shot.transform.position = firePos.position;
			shot.transform.rotation = Quaternion.Euler(0, 0, startAngle + i * deltaAngle);
			shot.Init(firingData.bulletSpeed, c, damage);
		}
	}

	void OnDestroy () {
		WeaponDataChanged = null;
	}
}
