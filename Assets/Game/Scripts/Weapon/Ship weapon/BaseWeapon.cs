using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Súng bắn đạn mở góc 60 độ, xuất phát tại 1 điểm, dùng cho tàu tam giác
/// </summary>
public class BaseWeapon : Weapon, ShipBaseWeapon {
	/// <summary>
	/// The max open angle at max level
	/// </summary>
	float limitOpenAngle = 50;
	/// <summary>
	/// The last time this weapon fires.
	/// </summary>
	float lastFireTime;

	public delegate void OnShooting ();
	public event OnShooting WeaponDataChanged;

	Transform firePos;
	float maxOpenAngle;
	float damagePerBullet;
	float damage;

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
			shot.transform.rotation = Quaternion.Euler(0, 0, Random.Range(90 - maxOpenAngle / 2, 90 + maxOpenAngle / 2));
			shot.Init(firingData.bulletSpeed, c, damage);
		}
	}

	void OnDestroy () {
		WeaponDataChanged = null;
	}
}