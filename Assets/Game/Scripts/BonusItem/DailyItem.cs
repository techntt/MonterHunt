using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DailyItem : MonoBehaviour {

	public float moveSpeed;
	public float accel;
	public float burstMagnitude;
	public AudioClip sfx;
	bool isPulled;
	public Rigidbody2D myBody;
	public SpriteRenderer myRender;
	Transform player;

	void Start () {
		player = GameManager.Instance.player1.transform;
	}

	public void Init () {
		if (!GameManager.Instance.gameView.Contains(transform.localPosition)) {
			DailyItemManager.Instance.PushItem(this);
			return;
		}
		isPulled = false;
		myBody.velocity = Vector2.zero;
		myBody.bodyType = RigidbodyType2D.Dynamic;
		myBody.AddForce(new Vector2 (Random.Range(-burstMagnitude, burstMagnitude), Random.Range(0, burstMagnitude)));
		myBody.AddTorque(Random.Range (-5, 5));
	}

	void Update () {
		if (isPulled) {
			transform.position = Vector3.MoveTowards (transform.position, player.position, moveSpeed * Time.deltaTime);
			moveSpeed += accel * Time.deltaTime;
		}
	}

	void OnTriggerEnter2D (Collider2D col) {
		if (col.CompareTag(Const.TAG_BOUNDARY)) {
			GlobalEventManager.Instance.OnPlayerGetDailyItem();
			SoundManager.Instance.PlaySfx (sfx);
			gameObject.SetActive(false);
		} else if (col.CompareTag(Const.TAG_MAGNET)) {
			isPulled = true;
		}
	}

	void OnBecameInvisible () {
		DailyItemManager.Instance.PushItem (this);
	}
}