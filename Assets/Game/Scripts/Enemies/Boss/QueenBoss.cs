using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class QueenBoss : MonoBehaviour {

	public BaseBoss myBoss;
	public Animator myAnim;
	public Animator wingAnim;
	public float moveSpeed;

	// minion control variables
	public BaseMinion sampleMinion;
	public int maxMinion = 5;
	public int activeMinion;
	public List<BeeMinion> myMinions;
	public Vector3[] formationPos;
	Stack<BeeMinion> inactiveMinion = new Stack<BeeMinion>();
	BaseMinion currentProtector;
	Vector3 currentHidePoint;
	bool isMoving;

	// breed variables
	public Transform belly;
	public Rect breedArea;
	public Vector3 breedPos;

	// dying cutscene variables
	public Vector3 pos1, pos2;

	enum AI_State {
		IDLE,
		BREED,
		ASSEMBLE,
		DISSEMBLE,
		SLEEP,
		ANGRY,
		DIE,
	}

	AI_State state;
	AI_State prevState;

	float determineTime;
	float sleepTime = 10;
	float angryTime = 5;

	public AudioSource wingSound;
	public AudioSource angrySfx;

	void Start () {
		state = AI_State.IDLE;
		prevState = AI_State.DIE;
		GameEventManager.Instance.BossFinishAppear += HandleBossFinishAppear;
		GameEventManager.Instance.BossDefeated += HandleBossDefeated;
		breedArea.xMin = GameManager.Instance.gameView.xMin + 0.4f;
		breedArea.xMax = GameManager.Instance.gameView.xMax - 0.4f;
		breedArea.yMin = -0.68f;
		breedArea.yMax = -2.5f;
		// spawn minions to use
		for (int i = 0; i < maxMinion; i++) {
			BeeMinion bee = Instantiate(sampleMinion) as BeeMinion;
			myMinions.Add(bee);
			inactiveMinion.Push(bee);
			bee.id = i;
			bee.MinionDie += HandleMinionDie;
		}
		// make the boss appear from above
		transform.DOLocalMoveY(2.42f, 2).OnComplete(() => {
			myBoss.FinishAppear();
			myAnim.SetTrigger(AnimConst.idle);
		});
	}

	void HandleMinionDie (BaseMinion m) {
		activeMinion--;
		m.gameObject.SetActive(false);
		inactiveMinion.Push((BeeMinion)m);
		if (state == AI_State.ASSEMBLE) {
			currentProtector = SelectRandomActiveMinion();
			if (currentProtector) {
				currentHidePoint = new Vector3(currentProtector.transform.localPosition.x, transform.localPosition.y);
				isMoving = true;
			}
		}
		if (activeMinion == 0 && state == AI_State.SLEEP) {
			CancelInvoke("WakeUp");
			myAnim.SetTrigger(AnimConst.idle);
//				else if (state == AI_State.ASSEMBLE) {
//				StopCoroutine("AttackAsFormation");
//				// choice 2: the boss return to idle state
//				determineTime = 1;
//				StartCoroutine("MakeDecision");
//			}
		}
	}

	void HandleBossDefeated () {
		state = AI_State.DIE;
		myAnim.SetTrigger(AnimConst.die);
		wingAnim.SetTrigger(AnimConst.die);
		wingSound.Pause();
		angrySfx.Pause();
		// TODO: make all minions of this boss die with her
		for (int i = 0; i < myMinions.Count; i++) {
			if (!myMinions[i].isDead)
				myMinions[i].PrepareDie();
		}
		StartCoroutine("DyingCutscene");
	}

	void HandleBossFinishAppear () {
		determineTime = 1;
		StartCoroutine("MakeDecision");
	}

	void Update () {
		if (state == AI_State.DISSEMBLE) {
			if (myAnim.GetCurrentAnimatorStateInfo(0).IsName("idle")) {
				state = AI_State.IDLE;
				OnStateChanged();
			}
		} else if (state == AI_State.ASSEMBLE) {
			// while minions are attacking, the boss hide behind them.
			// If the minion in front of her is killed, she moves to another minions
			if (isMoving && currentProtector && myAnim.GetCurrentAnimatorStateInfo(0).IsName("idle")) {
				transform.localPosition = Vector3.MoveTowards(transform.localPosition, currentHidePoint, Time.deltaTime * moveSpeed);
				if (transform.localPosition == currentHidePoint)
					isMoving = false;
			}
		} else if (state == AI_State.SLEEP) {
			if (myAnim.GetCurrentAnimatorStateInfo(0).IsName("idle")) {
				state = AI_State.IDLE;
				OnStateChanged();
			}
		} else if (state == AI_State.ANGRY) {
			if (myAnim.GetCurrentAnimatorStateInfo(0).IsName("idle")) {
				state = AI_State.IDLE;
				angrySfx.Pause();
				OnStateChanged();
			}
		}
	}

	IEnumerator MakeDecision () {
		yield return new WaitForSeconds(determineTime);
		if (state == AI_State.IDLE) {
			// while the boss is in IDLE state, the minions always behave freely
			// the boss has 3 choices: breed more minions, assemble minions or go sleep

			// if the queen wakes up and realize all her minions are dead, she would be angry
			if (activeMinion == 0 && prevState == AI_State.SLEEP)
				state = AI_State.ANGRY;
			// if the queen has just dissembled her minions, she should consider going to sleep or breeding
			else if (prevState == AI_State.DISSEMBLE) {
				int c = Random.Range(0, 100);
				if (c < activeMinion * 100f / maxMinion)
					state = AI_State.SLEEP;
				else
					state = AI_State.BREED;
			} else {
				if (activeMinion < maxMinion)
					state = AI_State.BREED;
				else
					state = AI_State.ASSEMBLE;
			}
		} else if (state == AI_State.ASSEMBLE) {
			// while the boss is in ASSEMBLE state, the minions always behave as a formation
			// the boss has 1 choice: dissemble minions
			state = AI_State.DISSEMBLE;
		}
		prevState = state;
		OnStateChanged();
	}

	void OnStateChanged () {
		switch (state) {
			case AI_State.IDLE:
				determineTime = Random.Range(2f, 3f);
				StartCoroutine("MakeDecision");
				break;
			case AI_State.ASSEMBLE:
				myAnim.SetTrigger(AnimConst.attack1);
				currentProtector = SelectRandomActiveMinion();
				currentHidePoint = new Vector3(currentProtector.transform.localPosition.x, transform.localPosition.y);
				isMoving = true;
				break;
			case AI_State.DISSEMBLE:
				myAnim.SetTrigger(AnimConst.attack1);
				break;
			case AI_State.BREED:
				myAnim.SetTrigger(AnimConst.attack);
				break;
			case AI_State.SLEEP:
				myAnim.SetTrigger(AnimConst.attack2);
				Invoke("WakeUp", sleepTime);
				break;
			case AI_State.ANGRY:
				myAnim.SetTrigger(AnimConst.start);
				Invoke("StopAngry", angryTime);
				angrySfx.Play();
				break;
		}
	}

	/// <summary>
	/// this function is used as an animation event.
	/// Select a random point then fire an egg to that point
	/// </summary>
	void SelectPositionToBreed () {
		breedPos.x = Random.Range(breedArea.xMin, breedArea.xMax);
		breedPos.y = Random.Range(breedArea.yMax, breedArea.yMin);
		Vector3 dir = breedPos - belly.position;
		float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
		belly.rotation = Quaternion.Euler(0, 0, angle);
	}

	/// <summary>
	/// this function is used as an animation event
	/// breed an egg to the selected position
	/// </summary>
	void Breed () {
		BeeMinion bee = inactiveMinion.Pop();
		if (bee) {
			bee.gameObject.SetActive(true);
			bee.transform.position = myBoss.firePos.position;
			bee.Init(breedPos);
			activeMinion++;
			if (activeMinion == maxMinion) {
				state = AI_State.IDLE;
				myAnim.SetTrigger(AnimConst.idle);
				OnStateChanged();
			}
		}
	}

	/// <summary>
	/// this function is used as an animation event
	/// assemble the minions
	/// </summary>
	void Assemble () {
		if (state == AI_State.ASSEMBLE) {
			// call minions assemble
			for (int i = 0; i < myMinions.Count; i++) {
				myMinions[i].isInFormation = true;
				myMinions[i].MoveToPosition(formationPos[i]);
			}
			// start the attack coroutine
			StartCoroutine("AttackAsFormation");
		}
	}

	IEnumerator AttackAsFormation () {
		yield return new WaitForSeconds(2);
		// wave 1: one minion strike at a time
		for (int i = 0; i < myMinions.Count; i++) {
			myMinions[i].Attack();
			yield return new WaitForSeconds(0.75f);
		}
		// wave 2: 2 minions strike at a time
		myMinions[1].Attack();
		myMinions[3].Attack();
		yield return new WaitForSeconds(0.75f);
		myMinions[0].Attack();
		myMinions[2].Attack();
		myMinions[4].Attack();
		yield return new WaitForSeconds(0.75f);
		myMinions[1].Attack();
		myMinions[3].Attack();
		yield return new WaitForSeconds(0.75f);
		myMinions[0].Attack();
		myMinions[2].Attack();
		myMinions[4].Attack();
		yield return new WaitForSeconds(0.75f);
		myMinions[1].Attack();
		myMinions[3].Attack();
		yield return new WaitForSeconds(0.75f);
		// wave 3: 3 minions strike at a time
		myMinions[0].Attack();
		myMinions[1].Attack();
		myMinions[2].Attack();
		yield return new WaitForSeconds(0.75f);
		myMinions[2].Attack();
		myMinions[3].Attack();
		myMinions[4].Attack();
		yield return new WaitForSeconds(0.75f);
		myMinions[0].Attack();
		myMinions[1].Attack();
		myMinions[2].Attack();
		yield return new WaitForSeconds(0.75f);
		myMinions[2].Attack();
		myMinions[3].Attack();
		myMinions[4].Attack();
		yield return new WaitForSeconds(0.75f);
		// last wave: 5 minions strike at a time
		for (int i = 0; i < myMinions.Count; i++) {
			myMinions[i].Attack();
		}
		yield return new WaitForSeconds(0.75f);
		// after finishing, dissemble minions
		determineTime = 1;
		StartCoroutine("MakeDecision");
	}

	/// <summary>
	/// this function is used as an animation event
	/// dissemble the minions, then return to IDLE state
	/// </summary>
	void Dissemble () {
		if (state == AI_State.DISSEMBLE) {
			// call minions dissemble
			for (int i = 0; i < myMinions.Count; i++) {
				myMinions[i].isInFormation = false;
				myMinions[i].state = BeeMinion.AI_STATE.MOVE;
				myMinions[i].OnStateChanged();
			}
		}
	}

	/// <summary>
	/// this function is invoked after several seconds from the moment the boss goes sleeping.
	/// if the boss is damaged while sleeping or all her minions are killed, this function is canceled invoking
	/// and the boss wakes up immediately 
	/// </summary>
	void WakeUp () {
		myAnim.SetTrigger(AnimConst.idle);
	}

	/// <summary>
	/// this function is invoked after several seconds from the moment the boss get angry
	/// </summary>
	void StopAngry () {
		myAnim.SetTrigger(AnimConst.idle);
	}

	BaseMinion SelectRandomActiveMinion () {
		List<BaseMinion> activeMinions = new List<BaseMinion>();
		for (int i = 1; i < myMinions.Count - 1; i++) {
			if (!myMinions[i].isDead) {
				activeMinions.Add(myMinions[i]);
			}
		}
		if (activeMinions.Count > 0)
			return activeMinions[Random.Range(0, activeMinions.Count - 1)];
		else
			return null;
	}

	IEnumerator DyingCutscene () {
		Vector3 pos1 = transform.localPosition + this.pos1;
		Vector3 pos2 = transform.localPosition + this.pos2;
		yield return new WaitForSeconds(3);
		myMinions[0].gameObject.SetActive(true);
		myMinions[0].state = BeeMinion.AI_STATE.DIE;
		myMinions[0].transform.position = pos1 + Vector3.up * 4;
		myMinions[0].myAnim.Play(AnimConst.move);
		myMinions[0].wingSfx.Play();
		myMinions[0].transform.DOLocalMoveY(pos1.y, 2).SetEase(Ease.Linear);
		myMinions[1].gameObject.SetActive(true);
		myMinions[1].state = BeeMinion.AI_STATE.DIE;
		myMinions[1].transform.position = pos2 + Vector3.up * 4;
		myMinions[1].myAnim.Play(AnimConst.move);
		myMinions[1].wingSfx.Play();
		myMinions[1].transform.DOLocalMoveY(pos2.y, 2).SetEase(Ease.Linear);
		yield return new WaitForSeconds(3);
		myMinions[0].transform.SetParent(transform);
		myMinions[1].transform.SetParent(transform);
		transform.DOLocalMoveY(8, 2).SetSpeedBased(true).OnComplete(() => {
			myMinions[0].wingSfx.Pause();
			myMinions[1].wingSfx.Pause();
			GameEventManager.Instance.OnBossFinishDie();
		});
	}
}