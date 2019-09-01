using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour {

	public float moveSpeed;
	public float accel;
	public float burstMagnitude;
	public Sprite coin;
	public Sprite diamond;
	public bool isGold;
	public AudioClip sfx;
	public AudioClip sfx2;
	bool isPulled;
	[SerializeField]Rigidbody2D myBody;
	[SerializeField]SpriteRenderer myRender;

	Transform player;

	int gold;

	void Start () {
		player = GameManager.Instance.player.transform;
		gold = GameManager.Instance.goldPerCoin;
	}

	public void Init () {
		if (!GameManager.Instance.gameView.Contains(transform.localPosition)) {
			CoinManager.Instance.PushCoin(this);
			return;
		}
		isPulled = false;
		int c = Random.Range(0, 100);
		myBody.velocity = Vector2.zero;
		myBody.bodyType = RigidbodyType2D.Dynamic;
		myBody.AddForce(new Vector2 (Random.Range(-burstMagnitude, burstMagnitude), Random.Range(0, burstMagnitude)));
		if (c < 5) {
			myRender.sprite = diamond;
			isGold = false;
			myBody.AddTorque(Random.Range (-5, 5));
		} else {
			myRender.sprite = coin;
			isGold = true;
		}
	}

	void OnTriggerEnter2D (Collider2D col) {
		if (col.CompareTag(Const.TAG_BOUNDARY)) {
			if (isGold) {
				GameManager.Instance.coin+= gold;
				SoundManager.Instance.PlaySfx(sfx, SFX_PLAY_STYLE.DONT_REWIND);
			} else {
				GameManager.Instance.coin += gold * 10;
				SoundManager.Instance.PlaySfx(sfx2, SFX_PLAY_STYLE.OVERRIDE);
			}
			GameEventManager.Instance.OnPlayerGetCoin(GameManager.Instance.player, this);
			gameObject.SetActive(false);
		} else if (col.CompareTag(Const.TAG_MAGNET)) {
			isPulled = true;
		}
	}

	void OnBecameInvisible () {
		CoinManager.Instance.PushCoin(this);
	}

	void Update () {
		if (isPulled) {
			transform.position = Vector3.MoveTowards (transform.position, player.position, moveSpeed * Time.deltaTime);
			moveSpeed += accel * Time.deltaTime;
		}
	}
}