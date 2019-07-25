using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Rendering;

public class SkullBoss : MonoBehaviour {

	public CentipedeBoss greenCentipede;
	public CentipedeBoss redCentipede;
	public Animator myAnim;
	/// <summary>
	/// left = red, right = green
	/// </summary>
	public Animator leftEyeAnim, rightEyeAnim;
	public SortingGroup leftCentipedeGroup, rightCentipedeGroup;
	public Transform leftEyeTrans, rightEyeTrans;
	public Collider2D leftCol, rightCol;
	public List<Vector3> leftPath, rightPath;
	public List<Vector3> returnPath, returnPath2;
	public BaseBoss myBoss;

	public SpriteRenderer leftMask, rightMask;
	public Transform sprayPos;
	public AudioClip laughSfx;

	public Rect movingArea;
	public float moveSpeed;

	enum AI_STATE {
		IDLE,
		CHASE,
		SPRAY,
		SHOOT,
		DIE
	}

	AI_STATE state, prevState;
	bool isMoving;
	bool redFinish, greenFinish;
	Vector3 destination;
	Transform player;

	// difficulty variables
	public float chaseTime;
	public float sprayRate;
	public float sprayTime;
	public float shootTime;
	float lastFireTime;

	void Start () {
		movingArea.xMin = GameManager.Instance.gameView.xMin + 1;
		movingArea.xMax = GameManager.Instance.gameView.xMax - 1;
		movingArea.yMin = 2.42f;
		movingArea.yMax = -3.7f;
		player = GameManager.Instance.player1.transform;
		leftEyeAnim.SetFloat(AnimConst.delay, 0.5f);
		rightEyeAnim.SetFloat(AnimConst.speed, 1.5f);
		GameEventManager.Instance.BossFinishAppear += HandleBossFinishAppear;
		GameEventManager.Instance.BossDefeated += HandleBossDefeated;
		// instantiate 2 centipedes
		greenCentipede = Instantiate(greenCentipede) as CentipedeBoss;
		redCentipede = Instantiate(redCentipede) as CentipedeBoss;
		// listen to events of each centipede
		greenCentipede.FinishPath += HandleFinishPath;
		redCentipede.FinishPath += HandleFinishPath;
		greenCentipede.ReachDestination += HandleReachDestination;
		redCentipede.ReachDestination += HandleReachDestination;
		// make the boss appear from above
		transform.DOLocalMoveY(movingArea.yMin, 2).OnComplete(() => {
			leftMask.DOFade(0, 1).SetEase(Ease.Linear);
			rightMask.DOFade(0, 1).SetEase(Ease.Linear).OnComplete(() => {
				leftEyeAnim.SetTrigger(AnimConst.appear);
				rightEyeAnim.SetTrigger(AnimConst.appear);
				Invoke("FinishAppear", 1);
			});
		});
	}

	void HandleBossDefeated () {
		if (state == AI_STATE.SPRAY || state == AI_STATE.SHOOT) {
			CancelInvoke("StopAttacking");
			StopAttacking();
		} else if (state == AI_STATE.IDLE) {
			StopCoroutine("MakeDecision");
			OnStateChanged();
		}
	}

	void HandleBossFinishAppear () {
		state = AI_STATE.IDLE;
		prevState = AI_STATE.IDLE;
		OnStateChanged();
	}

	void Update () {
		if (state == AI_STATE.CHASE) {
			if (isMoving) {
				transform.localPosition = Vector3.MoveTowards(transform.localPosition, destination, Time.deltaTime * moveSpeed);
				if (transform.localPosition == destination) {
					isMoving = false;
					StartCoroutine("ReleaseCentipede");
				}
			}
		} else if (state == AI_STATE.SPRAY) {
			if (isMoving) {
				transform.localPosition = Vector3.MoveTowards(transform.localPosition, destination, Time.deltaTime * moveSpeed);
				if (transform.localPosition == destination) {
					if (destination.x == movingArea.xMin)
						destination.x = movingArea.xMax;
					else
						destination.x = movingArea.xMin;
				}
				if (Time.time - lastFireTime > sprayRate) {
					lastFireTime = Time.time;
					// spawn circle
					CircleType t = CircleSpawner.Instance.GetRandomCircleType();
					Circle c = CircleManager.Instance.PopCircle(t, 0.75f, sprayPos.position);
					int hp = 0;
					if (Random.Range(0, 100) < 50)
						hp = CircleSpawner.Instance.h1;
					else
						hp = CircleSpawner.Instance.maxHP;
					c.myRender.sortingOrder = -2;
					c.Init(hp, CircleOrbit.NONE, 3, false, false, true);
					float speed = Random.Range(3f, 5f);
					c.myBody.velocity = Quaternion.Euler(0, 0, Random.Range(-20f, 20f)) * Vector3.down * speed;
				}
			}
		} else if (state == AI_STATE.SHOOT) {
			if (isMoving) {
				transform.localPosition = Vector3.MoveTowards(transform.localPosition, destination, Time.deltaTime * moveSpeed);
				if (transform.localPosition == destination) {
					if (destination.x == movingArea.xMin) {
						if (destination.y == movingArea.yMin) {
							destination.x = movingArea.xMax;
						} else {
							destination.y = movingArea.yMin;
						}
					} else {
						if (destination.y == movingArea.yMin) {
							destination.y = movingArea.yMax;
						} else {
							destination.x = movingArea.xMin;
						}
					}
				}
				Vector3 dir = player.localPosition - transform.localPosition;
				float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + 180;
				leftEyeTrans.rotation = Quaternion.Euler(0, 0, angle);
				rightEyeTrans.rotation = Quaternion.Euler(0, 0, angle);
			}
		} else if (state == AI_STATE.DIE) {
			if (isMoving) {
				transform.localPosition = Vector3.MoveTowards(transform.localPosition, destination, Time.deltaTime * moveSpeed);
				if (transform.localPosition == destination) {
					isMoving = false;
					StartCoroutine("Dying");
				}
			}
		}
	}

	void OnStateChanged () {
		switch (state) {
			case AI_STATE.IDLE:
				SoundManager.Instance.PlaySfx(laughSfx);
				StartCoroutine("MakeDecision");
				break;
			case AI_STATE.CHASE:
				isMoving = true;
				destination.x = 0;
				destination.y = movingArea.yMin;
				leftCol.enabled = false;
				rightCol.enabled = false;
				break;
			case AI_STATE.SPRAY:
				isMoving = true;
				destination.x = movingArea.xMin;
				destination.y = movingArea.yMin;
				Invoke("StopAttacking", sprayTime);
				break;
			case AI_STATE.SHOOT:
				isMoving = true;
				destination.x = movingArea.xMax;
				destination.y = movingArea.yMin;
				leftEyeAnim.SetTrigger(AnimConst.attack);
				rightEyeAnim.SetTrigger(AnimConst.attack);
				Invoke("StopAttacking", shootTime);
				break;
			case AI_STATE.DIE:
				isMoving = true;
				destination.x = 0;
				destination.y = movingArea.yMin;
				break;
		}
	}

	void StopAttacking () {
		state = AI_STATE.IDLE;
		leftEyeTrans.rotation = Quaternion.Euler(0, 0, 90);
		rightEyeTrans.rotation = Quaternion.Euler(0, 0, 90);
		leftEyeAnim.Play(AnimConst.idle);
		rightEyeAnim.Play(AnimConst.idle);
		OnStateChanged();
	}

	IEnumerator ReleaseCentipede () {
		redFinish = false;
		greenFinish = false;
		leftEyeAnim.SetTrigger(AnimConst.move);
		rightEyeAnim.SetTrigger(AnimConst.move);
		yield return new WaitForSeconds(1);
		leftMask.DOFade(1, 1).SetEase(Ease.Linear);
		rightMask.DOFade(1, 1).SetEase(Ease.Linear);
		yield return new WaitForSeconds(1);
		leftCentipedeGroup.sortingOrder = 0;
		rightCentipedeGroup.sortingOrder = 0;
		myAnim.SetTrigger(AnimConst.attack);
		yield return new WaitForSeconds(0.5f);
		// centipedes go out
		greenCentipede.nodes[0].localPosition = new Vector3(0, 3f);
		redCentipede.nodes[0].localPosition = new Vector3(0, 3f);
		greenCentipede.nodes[0].localRotation = Quaternion.Euler(0, 0, 90);
		redCentipede.nodes[0].localRotation = Quaternion.Euler(0, 0, 90);
		greenCentipede.gameObject.SetActive(true);
		redCentipede.gameObject.SetActive(true);
		greenCentipede.SetPathState(rightPath);
		redCentipede.SetPathState(leftPath);
	}

	IEnumerator MakeDecision () {
		if (myBoss.isDead) {
			state = AI_STATE.DIE;
			yield return null;
		} else {
			yield return new WaitForSeconds(2);
			List<AI_STATE> l = new List<AI_STATE>();
			for (int i = (int)AI_STATE.CHASE; i < (int)AI_STATE.DIE; i++) {
				if ((AI_STATE)i != prevState)
					l.Add((AI_STATE)i);
			}
			int c = Random.Range(0, l.Count);
			state = l[c];
			prevState = state;
		}
		OnStateChanged();
	}

	void FinishAppear () {
		myBoss.FinishAppear();
		myAnim.enabled = true;
	}

	void HandleFinishPath (CentipedeBoss c) {
		if (state == AI_STATE.CHASE) {
			if (Object.ReferenceEquals(c, greenCentipede)) {
				greenCentipede.SetChaseState(4, 6);
				Invoke("StopChasing", chaseTime);
			} else
				redCentipede.SetChaseState(6, 3);
		} else if (state == AI_STATE.IDLE) {
			c.SetReturnState(new Vector3(0, 2.42f));
		}
	}

	void StopChasing () {
		greenCentipede.SetReturnState(new Vector3(5, 0));
		redCentipede.SetReturnState(new Vector3(-5, 0));
	}

	IEnumerator CentipedeReturn () {
		yield return new WaitForSeconds(0.5f);
		leftCentipedeGroup.sortingOrder = 10;
		rightCentipedeGroup.sortingOrder = 10;
		leftMask.DOFade(0, 1).SetEase(Ease.Linear);
		rightMask.DOFade(0, 1).SetEase(Ease.Linear).OnComplete(() => {
			leftEyeAnim.SetTrigger(AnimConst.appear);
			rightEyeAnim.SetTrigger(AnimConst.appear);
		});
		yield return new WaitForSeconds(2);
		leftCol.enabled = true;
		rightCol.enabled = true;
		myAnim.SetTrigger(AnimConst.idle);
		OnStateChanged();
	}

	IEnumerator Dying () {
		myAnim.SetTrigger(AnimConst.die);
		leftEyeAnim.SetTrigger(AnimConst.die);
		rightEyeAnim.SetTrigger(AnimConst.die);
		yield return new WaitForSeconds(2.5f);
		for (int i = 0; i < 5; i++) {
			ExplodeEffect e =  (ExplodeEffect)EffectManager.Instance.SpawnEffect(EFFECT_TYPE.EXPLODE, transform.localPosition + new Vector3 (Random.Range(-1f, 1), Random.Range(-1f, 1)));
			e.Init(4, 0);
			yield return new WaitForSeconds(0.3f);
		}
		ExplodeEffect e1 =  (ExplodeEffect)EffectManager.Instance.SpawnEffect(EFFECT_TYPE.EXPLODE, transform.localPosition);
		e1.Init(8, 0);
		gameObject.SetActive(false);
		GameEventManager.Instance.OnBossFinishDie();
	}

	void HandleReachDestination (CentipedeBoss c) {
		if (state == AI_STATE.CHASE) {
			if (Object.ReferenceEquals(c, greenCentipede)) {
				greenCentipede.SetPathState(returnPath2);
				redFinish = true;
			} else {
				redCentipede.SetPathState(returnPath);
				greenFinish = true;
			}
			if (redFinish && greenFinish)
				state = AI_STATE.IDLE;
		} else if (state == AI_STATE.IDLE) {
			c.gameObject.SetActive(false);
			if (!greenCentipede.gameObject.activeInHierarchy && !redCentipede.gameObject.activeInHierarchy) {
				// both centipedes have returned
				StartCoroutine("CentipedeReturn");
			}
		}
	}
}