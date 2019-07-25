using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : Weapon {

	public override void Start () {
		base.Start();
		GameManager.Instance.player1.becomeVulnerable += OnPlayerBecomeVulnerable;
		transform.parent = GameManager.Instance.player1.transform;
		transform.localPosition = Vector3.zero;
	}

	void OnPlayerBecomeVulnerable () {
		if (gameObject.activeSelf)
			GameManager.Instance.player1.body.enabled = false;
	}

	public override void Activate () {
		base.Activate();
		GameManager.Instance.player1.body.enabled = false;
	}

	public override void Expire () {
		base.Expire();
		GameManager.Instance.player1.body.enabled = true;
	}
}