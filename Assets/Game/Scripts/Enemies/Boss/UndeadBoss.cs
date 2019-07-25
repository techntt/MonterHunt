using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UndeadBoss : MonoBehaviour {

	public Transform leftApple;
	public Transform rightApple;
	public Transform leftEye;
	public Transform rightEye;
	public GameObject laser1, laser2;
	float eyeDistance = 0.04f;
	Transform player;
	Player p;
	BaseBoss boss;

	enum AI_STATE {
		IDLE,
		MOVE,
		ATTACK,
		ATTACK1,
		ATTACK2,
		NONE
	}

	[SerializeField]
	AI_STATE AIState;
	public Rect movingArea;
	public float moveSpeed;


	public AudioClip warcrySfx;
	public AudioClip laserSfx;

	Vector3 des;
	int normalAttackCount;
	bool isAiming = true;

	void Start () {
		p = GameManager.Instance.player1;
		player = p.transform;
		boss = GetComponent<BaseBoss>();
		movingArea.xMin = GameManager.Instance.gameView.xMin + 1.5f;
		movingArea.xMax = GameManager.Instance.gameView.xMax - 1.5f;
		movingArea.yMin = 2.17f;
		movingArea.yMax = 0;
		GameEventManager.Instance.BossFinishAppear += HandleBossFinishAppear;
		GameEventManager.Instance.BossDefeated += HandleBossDefeated;
		transform.DOLocalMoveY(7.68f, 2).From().SetSpeedBased().OnComplete( () => {
			boss.FinishAppear();
		});
		SoundManager.Instance.PlaySfx(warcrySfx);
	}

	void HandleBossDefeated () {
		AIState = AI_STATE.NONE;
		boss.myAnim.SetTrigger(AnimConst.die);
	}

	void HandleBossFinishAppear () {
		AIState = AI_STATE.IDLE;
		StartCoroutine(MakeDecision());
	}

	void Update () {
		// make the boss always look at player
		if (isAiming) {
			Vector3 pPos = player.position;
			leftApple.position = Vector3.MoveTowards(leftEye.position, pPos, eyeDistance);
			rightApple.position = Vector3.MoveTowards(rightEye.position, pPos, eyeDistance);
		}
		if (AIState == AI_STATE.MOVE) {
			transform.position = Vector3.MoveTowards(transform.position, des, moveSpeed * Time.deltaTime);
			if (transform.position == des) {
				AIState = AI_STATE.IDLE;
				OnStateChanged();
			}
		} else if (AIState == AI_STATE.ATTACK) {
			if (boss.myAnim.GetCurrentAnimatorStateInfo(0).IsName("idle")) {
				AIState = AI_STATE.IDLE;
				OnStateChanged();
			}
		} else if (AIState == AI_STATE.ATTACK1) {
			if (boss.myAnim.GetCurrentAnimatorStateInfo(0).IsName("idle")) {
				AIState = AI_STATE.IDLE;
				OnStateChanged();
			}
		} else if (AIState == AI_STATE.ATTACK2) {
			if (boss.myAnim.GetCurrentAnimatorStateInfo(0).IsName("idle")) {
				AIState = AI_STATE.IDLE;
				OnStateChanged();
			}
		}
	}

	void OnStateChanged () {
		if (AIState == AI_STATE.IDLE) {
			StartCoroutine(MakeDecision());
		} else if (AIState == AI_STATE.MOVE) {
			// select a destination
			des.x = Random.Range (movingArea.xMin, movingArea.xMax);
			des.y = Random.Range(movingArea.yMax, movingArea.yMin);
		} else if (AIState == AI_STATE.ATTACK) {
			normalAttackCount = 0;
			boss.myAnim.SetTrigger(AnimConst.attack);
			boss.BossAttack += NormalAttack;
		} else if (AIState == AI_STATE.ATTACK1) {
			boss.BossAttack1 += HeavyAttack;
			boss.myAnim.SetTrigger(AnimConst.attack2);
		} else if (AIState == AI_STATE.ATTACK2) {
			boss.BossAttack1 += BlinkAttack;
			boss.myAnim.SetTrigger(AnimConst.attack2);
		}
	}

	IEnumerator MakeDecision () {
		yield return new WaitForSeconds(Random.Range(2, 2.5f));
		int c = Random.Range(0,4);
		if (c == 0) {
			AIState = AI_STATE.ATTACK;
		} else if (c == 1)
			AIState = AI_STATE.ATTACK1;
		else if (c == 2)
			AIState = AI_STATE.ATTACK2;
		else if (c == 3)
			AIState = AI_STATE.MOVE;
		OnStateChanged();
	}

	void NormalAttack () {
		CircleType t = CircleSpawner.Instance.GetRandomCircleType();
		Circle c = CircleManager.Instance.PopCircle(t, 0.8f, boss.firePos.position);
		int hp = CircleSpawner.Instance.GetRandomHP();
		c.Init(hp, CircleOrbit.NONE, 4, false, false, true);
		c.myBody.velocity = (player.position - boss.firePos.position).normalized * 4;
		normalAttackCount++;
		if (normalAttackCount == 3)
			boss.BossAttack -= NormalAttack;
	}

	void HeavyAttack () {
		StartCoroutine(IHeavyAttack());
		boss.BossAttack1 -= HeavyAttack;
	}

	IEnumerator IHeavyAttack () {
		float left = GameManager.Instance.gameView.xMin + 0.4f;
		float right = GameManager.Instance.gameView.xMax - 0.4f;
		for (int i = 0; i < 50; i++) {
			Vector3 pos = new Vector3(Random.Range (left, right), 6, 0);
			int hp = CircleSpawner.Instance.GetRandomHP();
			CircleType t = CircleSpawner.Instance.GetRandomCircleType();
			Circle c = CircleManager.Instance.PopCircle(t, 0.8f, pos);
			c.Init(hp, CircleOrbit.D, 6, false, false, true);
			yield return new WaitForSeconds(0.1f);
		}
		boss.myAnim.SetTrigger(AnimConst.idle);
	}

	void BlinkAttack () {
		StartCoroutine(IBlinkAttack());
		boss.BossAttack1 -= BlinkAttack;
	}

	IEnumerator IBlinkAttack () {
		isAiming = false;
		leftApple.localPosition = Vector3.zero;
		rightApple.localPosition = Vector3.zero;
		yield return new WaitForSeconds(1.5f);
		SoundManager.Instance.PlaySfxLoop(laserSfx, gameObject.GetInstanceID());
		leftApple.localPosition = new Vector3 (0.04f, 0, 0);
		rightApple.localPosition = new Vector3 (0.04f, 0, 0);
		leftEye.localRotation = Quaternion.Euler(0, 0, -30);
		rightEye.localRotation = Quaternion.Euler(0, 0, -30);
		laser1.SetActive(true);
		laser2.SetActive(true);
		leftEye.DOLocalRotate(new Vector3 (0, 0, -150), 3).SetEase(Ease.Linear);
		rightEye.DOLocalRotate(new Vector3 (0, 0, -150), 3).SetEase(Ease.Linear);
		yield return new WaitForSeconds(3.2f);
		SoundManager.Instance.StopLoopSound(laserSfx, gameObject.GetInstanceID());
		laser1.SetActive(false);
		laser2.SetActive(false);
		leftEye.localRotation = Quaternion.Euler(0, 0, 0);
		rightEye.localRotation = Quaternion.Euler(0, 0, 0);
		isAiming = true;
		boss.myAnim.SetTrigger(AnimConst.idle);
	}
}
