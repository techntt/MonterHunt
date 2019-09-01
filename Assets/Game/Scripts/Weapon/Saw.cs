using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Saw : Weapon {

	public float DPS;

	public override void Start () {
		base.Start();
		GameManager.Instance.player.becomeVulnerable += OnPlayerBecomeVulnerable;
		DPS = WeaponDataCalculator.SAW_DPS;
		transform.SetParent(GameManager.Instance.player.transform);
		transform.localPosition = Vector3.zero;
	}

	void OnPlayerBecomeVulnerable () {
		if (gameObject.activeSelf)
			GameManager.Instance.player.body.enabled = false;
	}

	public override void Activate () {
		base.Activate();
		transform.position = GameManager.Instance.player.transform.position;
		GameManager.Instance.player.body.enabled = false;
	}

	public override void Expire () {
		base.Expire();
		GameManager.Instance.player.body.enabled = true;
	}

	void OnTriggerStay2D (Collider2D col) {
		Damageable d = ColliderRef.Instance.GetDamageable (col.GetInstanceID ());
		if (d != null)
			d.TakeDamage(DPS * Time.fixedDeltaTime);
	}
}