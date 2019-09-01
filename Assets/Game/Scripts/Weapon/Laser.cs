using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : Weapon {

	Transform firePos;
	[SerializeField] ParticleSystem begin, end;
	float dps;
	[SerializeField] LineRenderer laser;
	[SerializeField] LayerMask mask;

	public override void Init () {
		base.Init();
		laser.sortingOrder = 5;
		firePos = GameManager.Instance.player.firePos;
	}

	public override void Activate () {
		base.Activate();
		GetWeaponData();
		begin.Clear();
		begin.Play();
		end.Clear();
		end.Play();
	}

	public override void Rewind () {
		base.Rewind();
		GetWeaponData();
	}

	public override void Expire () {
		base.Expire();
		laser.SetPosition(0, Vector3.zero);
		laser.SetPosition(1, Vector3.zero);
	}

	void GetWeaponData () {
		WeaponDataCalculator wdc = WeaponDataCalculator.Instance;
		Color laserColor = new Color();
		switch (WeaponManager.Instance.lastValue) {
			case 0:
				dps = WeaponDataCalculator.Instance.DPS(WeaponDataCalculator.MAX_WEAPON_LEVEL) * WeaponDataCalculator.LASER_CYAN * GameManager.Instance.player.baseDamage;
				laserColor = Color.cyan;
				break;
			case 1:
				dps = WeaponDataCalculator.Instance.DPS(WeaponDataCalculator.MAX_WEAPON_LEVEL) * WeaponDataCalculator.LASER_YELLOW * GameManager.Instance.player.baseDamage;
				laserColor = Color.yellow;
				break;
			case 2:
				dps = WeaponDataCalculator.Instance.DPS(WeaponDataCalculator.MAX_WEAPON_LEVEL) * WeaponDataCalculator.LASER_RED * GameManager.Instance.player.baseDamage;
				laserColor = Color.red;
				break;
		}
		laser.startColor = laserColor;
		laser.endColor = laserColor;
		ParticleSystem.MainModule main = begin.main;
		main.startColor = laserColor;
		main = end.main;
		main.startColor = laserColor;
	}

	void Update () {
		laser.SetPosition(0, firePos.position);
		begin.transform.position = firePos.position;
		Vector3 farPos = firePos.position;
		farPos.y = 8;
		RaycastHit2D hit = Physics2D.Linecast(firePos.position, farPos, mask.value);
		if (hit.collider != null) {
			Damageable d = ColliderRef.Instance.GetDamageable(hit.collider.GetInstanceID());
			if (d!=null)
				d.TakeDamage(dps * Time.deltaTime);
			laser.SetPosition(1, hit.point);
			end.transform.position = hit.point;
		} else {
			laser.SetPosition(1, farPos);
			end.transform.position = farPos;
		}

	}
}