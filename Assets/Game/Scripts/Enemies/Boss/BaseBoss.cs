using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseBoss : Damageable {

	public Animator myAnim;
	public Rigidbody2D myBody;
	public Collider2D[] vulnerableParts;
	public Collider2D[] invulnerableParts;
	public Transform firePos;

	public delegate void NoParamEvent ();

	public event NoParamEvent BossAttack;
	public event NoParamEvent BossAttack1;
	public event NoParamEvent BossAttack2;

	public void Attack () {
		if (BossAttack != null)
			BossAttack();
	}

	public void Attack1 () {
		if (BossAttack1 != null)
			BossAttack1();
	}

	public void Attack2 () {
		if (BossAttack2 != null)
			BossAttack2();
	}

	public void FinishAppear () {
		GameEventManager.Instance.OnBossAppear();
	}

	void Start () {
		hp = maxHp;
		damage = 1000;
		for (int i = 0; i < vulnerableParts.Length; i++) {
			vulnerableParts[i].enabled = false;
			ColliderRef.Instance.DamageableRef.Add(vulnerableParts[i].GetInstanceID(), this);
		}
		for (int i = 0; i < invulnerableParts.Length; i++) {
			invulnerableParts[i].enabled = false;
		}
		GameEventManager.Instance.BossFinishAppear += HandleBossFinishAppear;
	}

	void HandleBossFinishAppear () {
		for (int i = 0; i < vulnerableParts.Length; i++)
			vulnerableParts[i].enabled = true;
		for (int i = 0; i < invulnerableParts.Length; i++)
			invulnerableParts[i].enabled = true;
	}

	public override void TakeDamage (float damage) {
		if (!isDead)
			hp = Mathf.Max(0, hp - damage);
		if (hp == 0) {
			isDead = true;
			for (int i = 0; i < invulnerableParts.Length; i++) {
				invulnerableParts[i].enabled = false;
			}
			for (int i = 0; i < vulnerableParts.Length; i++) {
				vulnerableParts[i].enabled = false;
			}
			GameEventManager.Instance.OnBossDefeated();
		}

        // Show boss health
	}

	IEnumerator Dead () {
		for (int i = 0; i < invulnerableParts.Length; i++) {
			invulnerableParts[i].gameObject.SetActive(false);
			ExplodeEffect e =  (ExplodeEffect)EffectManager.Instance.SpawnEffect(EFFECT_TYPE.EXPLODE, invulnerableParts[i].transform.position);
			e.Init(5, 0);
			yield return new WaitForSeconds(0.3f);
		}
		for (int i = 0; i < vulnerableParts.Length; i++) {
			vulnerableParts[i].gameObject.SetActive(false);
			ExplodeEffect e =  (ExplodeEffect)EffectManager.Instance.SpawnEffect(EFFECT_TYPE.EXPLODE, vulnerableParts[i].transform.position);
			e.Init(10, 0);
			if (i < vulnerableParts.Length - 1)
				yield return new WaitForSeconds(0.3f);
		}
		gameObject.SetActive(false);
		GameEventManager.Instance.OnBossFinishDie();
	}

	void PlaySfx (AudioClip sfx) {
		SoundManager.Instance.PlaySfx(sfx);
	}
}