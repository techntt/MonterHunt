using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
/// <summary>
/// AI for boss spider
/// </summary>
public class SpiderBoss : MonoBehaviour {

	Player p;
	BaseBoss boss;

	public float moveSpeed;
	[SerializeField]Transform myTransform;

	enum AI_STATE {
		MOVE,
		ATTACK,
		ATTACK1,
		ATTACK2,
		NONE
	}

	[SerializeField]
	AI_STATE AIState;
	[SerializeField]
	bool toLeft;
	[SerializeField]
	Vector3 leftBound, rightBound;

	bool strike;
	int loopTime;

	void Start () {
		p = GameManager.Instance.player1;
		boss = GetComponent<BaseBoss>();
		AIState = AI_STATE.NONE;
		leftBound = new Vector3(GameManager.Instance.gameView.xMin + 1, 2.28f, 0);
		rightBound = new Vector3(GameManager.Instance.gameView.xMax - 1, 2.28f, 0);
		GameEventManager.Instance.BossFinishAppear += HandleBossFinishAppear;
		GameEventManager.Instance.BossDefeated += HandleBossDefeated;
		boss.BossAttack2 += Strike;
	}

	void HandleBossDefeated () {
		AIState = AI_STATE.NONE;
		boss.myAnim.SetTrigger(AnimConst.die);
	}

	void HandleBossFinishAppear () {
		AIState = AI_STATE.MOVE;
		boss.myAnim.SetTrigger(AnimConst.idle);
		OnStateChanged();
	}

	void Update () {
		if (AIState == AI_STATE.MOVE) {
			if (toLeft) {
				transform.position = Vector3.MoveTowards(transform.position, leftBound, moveSpeed * Time.deltaTime);
				if (transform.position == leftBound) {
					toLeft = false;
				}
			} else {
				transform.position = Vector3.MoveTowards(transform.position, rightBound, moveSpeed * Time.deltaTime);
				if (transform.position == rightBound) {
					toLeft = true;
				}
			}
		} else if (AIState == AI_STATE.ATTACK) {
			if (boss.myAnim.GetCurrentAnimatorStateInfo(0).IsName("idle")) {
				AIState = AI_STATE.MOVE;
				OnStateChanged();
			}
		} else if (AIState == AI_STATE.ATTACK1) {
			if (!strike) {
				Vector3 temp = new Vector3(0, 2.28f, 0);
				transform.position = Vector3.MoveTowards(transform.position, temp, moveSpeed * Time.deltaTime);
				if (transform.position == temp) {
					strike = true;
					boss.myAnim.SetTrigger(AnimConst.attack);
				}
			} else {
				if (boss.myAnim.GetCurrentAnimatorStateInfo(0).IsName("idle")) {
					AIState = AI_STATE.MOVE;
					OnStateChanged();
				}
			}
		} else if (AIState == AI_STATE.ATTACK2) {
			if (!strike) {
				Vector3 temp = Vector3.zero;
				temp.x = Mathf.Clamp(p.transform.position.x, leftBound.x, rightBound.x);
				temp.y = 2.28f;
				transform.position = Vector3.MoveTowards(transform.position, temp, 0.5f * Time.deltaTime);
			}
			if (boss.myAnim.GetCurrentAnimatorStateInfo(0).IsName("idle")) {
				AIState = AI_STATE.MOVE;
				OnStateChanged();
			}
		}
	}

	void OnStateChanged () {
		if (AIState == AI_STATE.MOVE) {
			StartCoroutine(MakeDecision());
		} else if (AIState == AI_STATE.ATTACK) {
			boss.myAnim.SetTrigger(AnimConst.attack);
			boss.BossAttack += NormalAttack;
		} else if (AIState == AI_STATE.ATTACK1) {
			loopTime = 0;
			strike = false;
			boss.BossAttack += MultiAttack;
		} else if (AIState == AI_STATE.ATTACK2) {
			boss.myAnim.SetTrigger(AnimConst.attack2);
			strike = false;
		}
	}

	void NormalAttack () {
		CircleType t = CircleSpawner.Instance.GetRandomCircleType();
		Circle c = CircleManager.Instance.PopCircle(t, 0.8f, boss.firePos.position);
		int hp = CircleSpawner.Instance.GetRandomHP();
		c.Init(hp, CircleOrbit.NONE, 4, false, false, true);
		c.myBody.velocity = (p.transform.position - boss.firePos.position).normalized * moveSpeed;
		boss.myAnim.SetTrigger(AnimConst.idle);
		boss.BossAttack -= NormalAttack;
	}

	void MultiAttack () {
		for (int i = 0; i < 5; i++) {
			CircleType t = CircleSpawner.Instance.GetRandomCircleType();
			Circle c = CircleManager.Instance.PopCircle(t, 0.8f, boss.firePos.position);
			int hp = CircleSpawner.Instance.GetRandomHP();
			c.Init(hp, CircleOrbit.NONE, 4, false, false, true);
			c.myBody.velocity = Quaternion.Euler(0, 0, (i - 2) * 20) * Vector3.down * moveSpeed;
		}
		loopTime++;
		if (loopTime == 3) {
			boss.myAnim.SetTrigger(AnimConst.idle);
			boss.BossAttack -= MultiAttack;
		}
	}

	void Strike () {
		strike = true;
		Sequence se = DOTween.Sequence();
		se.Append(transform.DOMoveY(-3, 0.33f).SetEase(Ease.OutQuart)).Append(transform.DOMoveY (2.28f, 0.5f).SetEase(Ease.Linear));
	}

	IEnumerator MakeDecision () {
		yield return new WaitForSeconds(Random.Range(1, 1.5f));
		int c = Random.Range(0,3);
		if (c == 0)
			AIState = AI_STATE.ATTACK;
		else if (c == 1)
			AIState = AI_STATE.ATTACK1;
		else
			AIState = AI_STATE.ATTACK2;
		OnStateChanged();
	}

	void ShowUp () {
		transform.DOMoveY(10, 2).From().SetEase(Ease.OutBack);
	}
}