using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CentipedeHead : MonoBehaviour {

	public Transform firePos;
	public Transform root;

	public bool isRed;

	void Attack () {
		CircleType t = CircleSpawner.Instance.GetRandomCircleType();
		Circle c = CircleManager.Instance.PopCircle(t, 0.6f, firePos.position);
		int hp = isRed ? CircleSpawner.Instance.maxHP : CircleSpawner.Instance.h1;
		c.myRender.sortingOrder = -1;
		c.Init(hp, CircleOrbit.NONE, 3, false, false, true);
		float speed = isRed ? -4 : -6;
		c.myBody.velocity = root.right * speed;
	}

	void PlaySfx (AudioClip sfx) {
		SoundManager.Instance.PlaySfx(sfx);
	}
}