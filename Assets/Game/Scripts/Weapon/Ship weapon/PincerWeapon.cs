using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PincerWeapon : Weapon, ShipBaseWeapon {

	/// <summary>
	/// The last time this weapon fires.
	/// </summary>
	float lastFireTime;

	public Transform[] firePos;
	public List<Transform> currentFirePos = new List<Transform>();
	float damagePerBullet;
	float damage;

	public FiringWeaponData firingData;

	public Player myPlayer;

	public event BaseWeapon.OnShooting WeaponDataChanged;

	void OnDestroy () {
		WeaponDataChanged = null;
	}

	public override void Start () {
		base.Start();

		if (PopupManager.Instance.scene == SCENE.GAME) {
			myPlayer.xDamageChange += HandleXDamageChange;
			GameEventManager.Instance.PlayerHealthChanged += HandleHealthChange;
		}
	}

	public override void Activate () {
		base.Activate();
		GetWeaponData(myPlayer.power);
		lastFireTime = Time.time;
	}

	public void GetWeaponData (int level) {
		WeaponDataCalculator wdc = WeaponDataCalculator.Instance;
		firingData.bulletSpeed = wdc.Speed(level) * 1.5f;
		firingData.fireRate = wdc.FireRate(level);
		firingData.maxRays = Mathf.Clamp(level / 7, 1, 7);
		damagePerBullet = WeaponDataCalculator.Instance.DPS(level) * firingData.fireRate / firingData.maxRays;
		damage = damagePerBullet * myPlayer.baseDamage;
		GetCurrentFirePos();
	}

	void GetCurrentFirePos () {
		currentFirePos.Clear();
		switch (firingData.maxRays) {
			case 1:
				currentFirePos.Add(firePos[0]);
				break;
			case 2:
				currentFirePos.Add(firePos[1]);
				currentFirePos.Add(firePos[2]);
				break;
			case 3:
				currentFirePos.Add(firePos[0]);
				currentFirePos.Add(firePos[3]);
				currentFirePos.Add(firePos[4]);
				break;
			case 4:
				currentFirePos.Add(firePos[2]);
				currentFirePos.Add(firePos[3]);
				currentFirePos.Add(firePos[5]);
				currentFirePos.Add(firePos[6]);
				break;
			case 5:
				currentFirePos.Add(firePos[0]);
				currentFirePos.Add(firePos[1]);
				currentFirePos.Add(firePos[2]);
				currentFirePos.Add(firePos[5]);
				currentFirePos.Add(firePos[6]);
				break;
			case 6:
				currentFirePos.Add(firePos[1]);
				currentFirePos.Add(firePos[2]);
				currentFirePos.Add(firePos[3]);
				currentFirePos.Add(firePos[4]);
				currentFirePos.Add(firePos[5]);
				currentFirePos.Add(firePos[6]);
				break;
			case 7:
				currentFirePos.Add(firePos[0]);
				currentFirePos.Add(firePos[1]);
				currentFirePos.Add(firePos[2]);
				currentFirePos.Add(firePos[3]);
				currentFirePos.Add(firePos[4]);
				currentFirePos.Add(firePos[5]);
				currentFirePos.Add(firePos[6]);
				break;
		}
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
		for (int i = 0; i < currentFirePos.Count; i++) {
			Bullet shot = BulletManager.Instance.PopBullet(BulletType.Normal, myPlayer.myBullet);
			shot.transform.position = currentFirePos[i].position;
			shot.transform.rotation = Quaternion.Euler(0, 0, 90);
			shot.Init(firingData.bulletSpeed, c, damage);
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
}