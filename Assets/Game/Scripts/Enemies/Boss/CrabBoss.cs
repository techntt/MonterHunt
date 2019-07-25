using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CrabBoss : MonoBehaviour {

	public BaseBoss myBoss;
	public Transform leftFirePos, rightFirePos;
	public Transform leftImpactPos, rightImpactPos;

	enum AI_STATE {
		IDLE,
		ATTACK,
		ATTACK1,
		ATTACK2,
		NONE
	}

	AI_STATE state;
	AI_STATE prevState;

	public Animator myAnim;
	public Vector3 mainPos;
	Vector3 tempPos;
	public float moveSpeed;
	Rect movingArea;
	/// <summary>
	/// The number of times the shooting attack would repeat.
	/// </summary>
	public int numOfShots;
	bool isMoving;
	int loopTime;

	/// <summary>
	/// turn on all particles system in this arrays when the crab uses slam attacks
	/// </summary>
	public ParticleSystem[] pincerParticles;
	/// <summary>
	/// the shock wave variables
	/// </summary>
	public ParticleSystem waveLeft, waveRight;
	public Collider2D boxLeft, boxRight;
	public Transform trLeft, trRight;

	void Start () {
		movingArea.xMin = GameManager.Instance.gameView.xMin + 1.69f;
		movingArea.xMax = GameManager.Instance.gameView.xMax - 1.69f;
		movingArea.yMin = 3.66f;
		movingArea.yMax = -1.5f;
		state = AI_STATE.NONE;
		prevState = AI_STATE.NONE;
		GameEventManager.Instance.BossFinishAppear += HandleBossFinishAppear;
		GameEventManager.Instance.BossDefeated += HandleBossDeafeated;
		myBoss.BossAttack1 += GroundSlam;
		myBoss.BossAttack2 += WaterGun;
		trLeft.SetParent(null);
		trRight.SetParent(null);
		boxLeft.enabled = false;
		boxRight.enabled = false;
		transform.DOMoveX(mainPos.x, 4).SetEase(Ease.Linear).OnComplete(() => {
			myAnim.SetTrigger(AnimConst.move);
			myBoss.FinishAppear();
		});
	}

	void WaterGun () {
		Vector3 leftDir = Quaternion.Euler(0, 0, -54) * Vector3.right;
		Vector3 rightDir = Quaternion.Euler(0, 0, 63) * Vector3.left;
		for (int i = 0; i < 3; i++) {
			CircleType t = CircleSpawner.Instance.GetRandomCircleType();
			Circle c = CircleManager.Instance.PopCircle(t, 0.75f, leftFirePos.position);
			int hp = CircleSpawner.Instance.GetRandomHP();
			c.Init(hp, CircleOrbit.NONE, 4, false, false, true);
			c.myBody.velocity = Quaternion.Euler(0, 0, (i - 1) * 20) * leftDir * 4;
			t = CircleSpawner.Instance.GetRandomCircleType();
			c = CircleManager.Instance.PopCircle(t, 0.75f, rightFirePos.position);
			hp = CircleSpawner.Instance.GetRandomHP();
			c.Init(hp, CircleOrbit.NONE, 4, false, false, true);
			c.myBody.velocity = Quaternion.Euler(0, 0, (i - 1) * 20) * rightDir * 4;
		}
		loopTime++;
		if (loopTime == numOfShots)
			myAnim.SetTrigger(AnimConst.idle);
	}

	void TurnOnPincerParticle () {
		for (int i = 0; i < pincerParticles.Length; i++) {
			pincerParticles[i].Play();
		}
	}

	void TurnOffPincerParticle () {
		for (int i = 0; i < pincerParticles.Length; i++) {
			pincerParticles[i].Stop();
			pincerParticles[i].Clear();
		}
	}

	void GroundSlam () {
		CameraShake.Instance.Vibrate(1f, 0.5f);
		ExplodeEffect e = EffectManager.Instance.SpawnEffect(EFFECT_TYPE.EXPLODE, leftImpactPos.position) as ExplodeEffect;
		e.tag = "boss";
		e.Init(7f, 0, LayerMask.NameToLayer("Default"));
		ExplodeEffect e1 = EffectManager.Instance.SpawnEffect(EFFECT_TYPE.EXPLODE, rightImpactPos.position) as ExplodeEffect;
		e1.tag = "boss";
		e1.Init(7f, 0, LayerMask.NameToLayer("Default"));
		TurnOffPincerParticle();
		// call the waves
		trLeft.position = leftImpactPos.position;
		trRight.position = rightImpactPos.position;
		boxLeft.enabled = true;
		boxRight.enabled = true;
		waveLeft.Play();
		waveRight.Play();
		Invoke("ClearWaves", 0.8f);
	}

	void ClearWaves () {
		boxLeft.enabled = false;
		boxRight.enabled = false;
		waveLeft.Stop();
		waveRight.Stop();
	}

	void HandleBossDeafeated () {
		state = AI_STATE.NONE;
		myAnim.SetTrigger(AnimConst.die);
	}

	void HandleBossFinishAppear () {
		state = AI_STATE.IDLE;
		myAnim.SetTrigger(AnimConst.idle);
		OnStateChanged();
	}

	void Update () {
		if (state == AI_STATE.IDLE) {
			transform.position = Vector3.MoveTowards(transform.position, tempPos, Time.deltaTime * moveSpeed);
			if (transform.position == tempPos)
				GetRandomDestination();
		} else if (state == AI_STATE.ATTACK1) {
			if (myAnim.GetCurrentAnimatorStateInfo(0).IsName("idle")) {
				state = AI_STATE.IDLE;
				OnStateChanged();
			}
		} else if (state == AI_STATE.ATTACK2) {
			if (isMoving) {
				transform.position = Vector3.MoveTowards(transform.position, mainPos, Time.deltaTime * moveSpeed);
				if (transform.position == mainPos) {
					isMoving = false;
					myAnim.SetTrigger(AnimConst.attack2);
				}
			} else {
				if (myAnim.GetCurrentAnimatorStateInfo(0).IsName("idle")) {
					state = AI_STATE.IDLE;
					OnStateChanged();
				}
			}
		} else if (state == AI_STATE.ATTACK) {
			if (isMoving) {
				transform.position = Vector3.MoveTowards(transform.position, mainPos, Time.deltaTime * moveSpeed);
				if (transform.position == mainPos) {
					isMoving = false;
					myAnim.SetTrigger(AnimConst.attack);
				}
			} else {
				if (myAnim.GetCurrentAnimatorStateInfo(0).IsName("idle")) {
					state = AI_STATE.IDLE;
					OnStateChanged();
				}
			}
		}
	}

	void GetRandomDestination () {
		tempPos.x = Random.Range(movingArea.xMin, movingArea.xMax);
		tempPos.y = Random.Range(movingArea.yMax, movingArea.yMin);
	}

	IEnumerator MakeDecision () {
		yield return new WaitForSeconds(Random.Range(1.5f, 3f));
		List<AI_STATE> l = new List<AI_STATE>();
		for (int i = (int)AI_STATE.ATTACK; i < (int)AI_STATE.NONE; i++) {
			if ((AI_STATE)i != prevState)
				l.Add((AI_STATE)i);
		}
		int c = Random.Range(0, l.Count);
		state = l[c];
		prevState = state;
		OnStateChanged();
	}

	void OnStateChanged () {
		switch (state) {
			case AI_STATE.ATTACK1:
				myAnim.SetTrigger(AnimConst.attack1);
				break;
			case AI_STATE.IDLE:
				GetRandomDestination();
				StartCoroutine(MakeDecision());
				break;
			case AI_STATE.ATTACK2:
				isMoving = true;
				break;
			case AI_STATE.ATTACK:
				loopTime = 0;
				isMoving = true;
				break;
		}
	}
}