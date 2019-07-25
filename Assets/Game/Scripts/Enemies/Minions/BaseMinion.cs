using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseMinion : Damageable {
	public int id;
	public Animator myAnim;
	public Collider2D myCollider;

	public delegate void MinionEvent (BaseMinion m);
	public event MinionEvent MinionDie;

	public override void Die () {
		isDead = true;
	}

	public void OnMinionDie () {
		if (MinionDie != null)
			MinionDie(this);
	}

	void PlaySfx (AudioClip sfx) {
		SoundManager.Instance.PlaySfx(sfx);
	}
}