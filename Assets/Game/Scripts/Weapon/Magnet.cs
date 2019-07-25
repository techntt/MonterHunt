using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magnet : Weapon {

	float oldRadius;
	public ParticleSystem myPar;

	public override void Init () {
		base.Init();
		transform.parent = GameManager.Instance.player1.transform;
		transform.localPosition = Vector3.zero;
	}

	public override void Activate () {
		base.Activate();
		myPar.Clear();
		GameManager.Instance.player1.magnetField.radius = 4;
	}

	public override void Expire () {
		base.Expire();
		GameManager.Instance.player1.magnetField.radius = GameManager.Instance.player1.magnetRadius;
	}

	public override void Remove () {
		base.Remove();
		GameManager.Instance.player1.magnetField.radius = GameManager.Instance.player1.magnetRadius;
	}
}