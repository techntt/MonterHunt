using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class KingBoss : MonoBehaviour {

	BaseBoss boss;
	[SerializeField]Transform crown;
	[SerializeField]Transform saw;
	[SerializeField]Transform gun;

	enum AI_STATE {
		IDLE,
		ATTACK1,
		ATTACK2,
		NONE
	}

	[SerializeField]
	AI_STATE AIState;
	public Rect movingArea;

	Vector3 des;

	public Animator crownAnim;
	public Animator gunAnim;
	public Animator sawAnim;

	void Start () {
		boss = GetComponent<BaseBoss>();
		movingArea.xMin = GameManager.Instance.gameView.xMin + 1.33f;
		movingArea.xMax = GameManager.Instance.gameView.xMax - 1.33f;
		movingArea.yMin = 3;
		movingArea.yMax = 0;
		GameEventManager.Instance.BossFinishAppear += HandleBossFinishAppear;
		GameEventManager.Instance.BossDefeated += HandleBossDefeated;
		boss.myAnim.SetTrigger(AnimConst.idle);
		transform.DOMoveX(0, 2).SetSpeedBased().SetEase(Ease.Linear).OnComplete( () => {
			boss.myAnim.SetTrigger (AnimConst.start);
		});
	}

	void HandleBossDefeated () {
		AIState = AI_STATE.NONE;
		boss.BossAttack -= JumpAttack;
		boss.myAnim.SetBool(AnimConst.isDead, true);
	}

	void HandleBossFinishAppear () {
		AIState = AI_STATE.IDLE;
		boss.BossAttack += JumpAttack;
		boss.myAnim.Play("jump attack");
		StartCoroutine(MakeDecision());
	}

	void OnStateChanged () {
		if (AIState == AI_STATE.ATTACK1) {
			boss.myAnim.SetTrigger(AnimConst.attack1);
		}
	}

	IEnumerator MakeDecision () {
		AIState = AI_STATE.IDLE;
		yield return new WaitForSeconds(Random.Range(2.5f, 4.5f));
		int c = Random.Range(0, 2);
		if (c == 0)
			AIState = AI_STATE.ATTACK1;
		else
			AIState = AI_STATE.ATTACK2;
		OnStateChanged();
	}

	//********** attack functions ***********************************
	void GetJumpTarget () {
		if (!boss.isDead) {
			if (AIState == AI_STATE.ATTACK2) {
				des.x = Random.Range(movingArea.xMin, movingArea.xMax);
				des.y = 0;
				boss.myAnim.SetTrigger(AnimConst.attack2);
			} else {
				des.x = Random.Range(movingArea.xMin, movingArea.xMax);
				des.y = Random.Range(movingArea.yMax, movingArea.yMin);
			}
		} else {
			des.x = 0;
			des.y = 2.1f;
			boss.myAnim.SetTrigger(AnimConst.die);
		}
		if (des.y >= transform.position.y) {
			transform.DOMove(des, 0.5f).SetEase(Ease.Linear);
		} else {
			transform.DOMoveX(des.x, 0.5f).SetEase(Ease.Linear).OnComplete(
				() => {
					transform.DOMoveY(des.y, 0.5f).SetEase(Ease.Linear);
				});
		}
	}

	void JumpAttack () {
		CircleType t = CircleType.HARDEN;
		Circle c = CircleManager.Instance.PopCircle(t, 1, saw.position);
		int hp = CircleSpawner.Instance.GetRandomHP();
		c.Init(hp, CircleOrbit.ZL, 4, false, false, true);
//		t = CircleSpawner.Instance.GetRandomCircleType();
//		c = CircleManager.Instance.PopCircle(t, 1, gun.position);
//		hp = CircleSpawner.Instance.GetRandomHP();
//		c.Init(hp, CircleOrbit.ZR, 4, false, false, true);
	}

	void GunAttack (CircleOrbit o) {
		for (int i = 0; i < 3; i++) {
			CircleType t = CircleSpawner.Instance.GetRandomCircleType();
			Circle c = CircleManager.Instance.PopCircle(t, 0.8f, gun.position);
			int hp = CircleSpawner.Instance.GetRandomHP();
			c.Init(hp, o, 4, false, false, true);
			c.myBody.velocity = Quaternion.Euler(0, 0, (i - 1) * 20) * -gun.up * 4;
		}
	}

	//********** functions set in animation events ******************
	void GunShot () {
		gunAnim.SetTrigger(AnimConst.attack);
	}

	void SawRotate () {
		sawAnim.enabled = true;
	}

	void CrownBlink () {
		crownAnim.SetTrigger(AnimConst.idle);
	}

	void ShakeCamera () {
		CameraShake.Instance.Vibrate(0.3f, 0.1f);
	}

	void CrownExplode () {
		ExplodeEffect e = (ExplodeEffect)EffectManager.Instance.SpawnEffect(EFFECT_TYPE.EXPLODE, crown.position);
		e.Init(5, 0);
	}

	IEnumerator FinishDead () {
		yield return new WaitForSeconds(2);
		transform.DOMoveX(8, 2).SetSpeedBased().SetEase(Ease.Linear).OnComplete( () => {
			gameObject.SetActive(false);
			GameEventManager.Instance.OnBossFinishDie();
		});
	}
}
