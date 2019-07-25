using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Rendering;

public class BeeMinion : BaseMinion {

	public Image healthBar;

	// stats variables
	public float speed;
	public float reactTime;

	public SortingGroup mySortGroup;
	public bool isInFormation;

	public enum AI_STATE {
		EGG,
		IDLE,
		MOVE,
		ATTACK,
		DIE
	}

	public AI_STATE state;
	AI_STATE prevState;

	Rect movingArea;
	Vector3 destination;

	public AudioSource wingSfx;

	void Start () {
		movingArea.xMin = GameManager.Instance.gameView.xMin + 0.4f;
		movingArea.xMax = GameManager.Instance.gameView.xMax - 0.4f;
		movingArea.yMin = 3.16f;
		movingArea.yMax = -2.5f;
		damage = 5;
		ColliderRef.Instance.DamageableRef.Add(myCollider.GetInstanceID(), this);
	}

	void Update () {
		if (!isInFormation) {
			if (state == AI_STATE.MOVE) {
				if (myAnim.GetCurrentAnimatorStateInfo(0).IsName("move")) {
					transform.localPosition = Vector3.MoveTowards(transform.localPosition, destination, Time.deltaTime * speed);
					if (transform.localPosition == destination) {
						state = AI_STATE.IDLE;
						myAnim.SetTrigger(AnimConst.idle);
						OnStateChanged();
					}
				}
			} else if (state == AI_STATE.ATTACK) {
				if (myAnim.GetCurrentAnimatorStateInfo(0).IsName("idle")) {
					state = AI_STATE.IDLE;
					OnStateChanged();
				}
			}
		} else {
			if (state == AI_STATE.MOVE) {
				if (myAnim.GetCurrentAnimatorStateInfo(0).IsName("move")) {
					transform.localPosition = Vector3.MoveTowards(transform.localPosition, destination, Time.deltaTime * speed);
					if (transform.localPosition == destination) {
						state = AI_STATE.IDLE;
						myAnim.SetTrigger(AnimConst.idle);
					}
				}
			} else if (state == AI_STATE.ATTACK) {
				if (myAnim.GetCurrentAnimatorStateInfo(0).IsName("idle")) {
					state = AI_STATE.IDLE;
				}
			}
		}
	}

	public void Init (Vector3 des) {
		isInFormation = false;
		hp = maxHp;
		healthBar.fillAmount = 1;
		healthBar.enabled = false;
		isDead = false;
		myAnim.Play("egg");
		destination = des;
		state = AI_STATE.EGG;
		OnStateChanged();
	}

	public override void TakeDamage (float damage) {
		if (!isDead && state != AI_STATE.EGG) {
			hp = Mathf.Clamp(hp - damage, 0, maxHp);
			if (hp > 0) {
				healthBar.enabled = true;
				healthBar.fillAmount = hp / maxHp;
			} else {
				PrepareDie();
			}
		}
	}

	public void PrepareDie () {
		isDead = true;
		state = AI_STATE.DIE;
		healthBar.enabled = false;
		myAnim.SetTrigger(AnimConst.die);
		Bonus b = BonusManager.Instance.GetBonus(BonusManager.Instance.GetRandomBonusType());
		if (b != null) {
			b.transform.position = transform.position;
			b.Init();
		}
		mySortGroup.sortingOrder = 2;
		wingSfx.Pause();
	}

	public override void Die () {
		isDead = true;
		OnMinionDie();
	}

	public void Attack () {
		if (!isDead)
			myAnim.SetTrigger(AnimConst.attack);
	}

	public void MoveToPosition (Vector3 position) {
		state = AI_STATE.MOVE;
		if (!myAnim.GetCurrentAnimatorStateInfo(0).IsName("move"))
			myAnim.SetTrigger(AnimConst.move);
		destination = position;
		StopCoroutine("MakeDecision");
	}

	public void GetRandomPosition () {
		destination.x = Random.Range(movingArea.xMin, movingArea.xMax);
		destination.y = Random.Range(movingArea.yMax, movingArea.yMin);
	}

	IEnumerator MakeDecision () {
		yield return new WaitForSeconds(reactTime);
		if (prevState == AI_STATE.ATTACK || transform.localPosition.y < -1)
			state = AI_STATE.MOVE;
		else {
			int c = Random.Range(0, 100);
			if (c < 50)
				state = AI_STATE.MOVE;
			else
				state = AI_STATE.ATTACK;
		}
		prevState = state;
		OnStateChanged();
	}

	public void OnStateChanged () {
		switch (state) {
			case AI_STATE.IDLE:
				StartCoroutine("MakeDecision");
				break;
			case AI_STATE.MOVE:
				GetRandomPosition();
				myAnim.SetTrigger(AnimConst.move);
				break;
			case AI_STATE.ATTACK:
				myAnim.SetTrigger(AnimConst.attack);
				break;
			case AI_STATE.EGG:
				transform.DOMove(destination, 0.5f).OnComplete(() => {
					myAnim.SetTrigger (AnimConst.start);
				});
				break;
		}
	}

	// this function is used as animation event
	void AfterHatch () {
		state = AI_STATE.IDLE;
		OnStateChanged();
		mySortGroup.sortingOrder = 4;
		wingSfx.Play();
	}

}