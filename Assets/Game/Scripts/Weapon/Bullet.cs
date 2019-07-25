using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Bullet : MonoBehaviour {
    
	public Rigidbody2D mRigidbody2D;
	public SpriteRenderer myRender;
	public BoxCollider2D myCollider;
	public BulletType type;
	public float damage;
	public Damageable target;

	public AudioClip forceWaveSfx;

	void OnTriggerEnter2D (Collider2D col) {
		if (type == BulletType.Slow) {
			Damageable d = ColliderRef.Instance.GetDamageable(col.GetInstanceID());
			if (d != null && d is Circle) {
				Circle c = (Circle)d;
				if (c.speed == c.initSpeed) {
					c.speed *= 0.5f;
					c.myBody.velocity *= 0.5f;
				}
			}
		} else if (type == BulletType.Rocket) {
			Damageable d = ColliderRef.Instance.GetDamageable(col.GetInstanceID());
			if (ReferenceEquals(d, target)) {
				ExplodeEffect e = (ExplodeEffect)EffectManager.Instance.SpawnEffect(EFFECT_TYPE.EXPLODE, transform.position);
				e.Init(5, damage);
				gameObject.SetActive(false);
			}
		} else if (type == BulletType.Bloom) {
			Damageable d = ColliderRef.Instance.GetDamageable(col.GetInstanceID());
			if (d != null)
				d.TakeDamage(damage);
		} else {
			Damageable d = ColliderRef.Instance.GetDamageable(col.GetInstanceID());
			if (d != null)
				d.TakeDamage(damage);
			if (PlayerSettingData.Instance.graphic == GRAPHIC_QUALITY.HIGH)
				BulletManager.Instance.SpawnEffect(transform.position, myRender.color);
			gameObject.SetActive(false);
		}
	}

	public void Init (float speed, Color color, float damage) {
		mRigidbody2D.velocity = transform.right * speed;
		myRender.color = color;
		this.damage = damage;
		if (type == BulletType.Bloom) {
			CameraShake.Instance.Vibrate(100, 0.05f);
			SoundManager.Instance.PlaySfxLoop(forceWaveSfx, gameObject.GetInstanceID());
		}
	}

	void Update () {
		if (type == BulletType.Rocket) {
			if (target != null) {
				if (!target.isDead) {
					transform.rotation = Quaternion.Euler(0, 0, Vector2.Angle(Vector2.right, target.transform.position - transform.position));
					mRigidbody2D.velocity = transform.right * mRigidbody2D.velocity.magnitude;
				} else {
					target = null;
				}
			}
		}
	}

	void OnBecameInvisible () {
		// push the bullet
		target = null;
		if (type == BulletType.Bloom) {
			CameraShake.Instance.StopVibrate();
			SoundManager.Instance.StopLoopSound(forceWaveSfx, gameObject.GetInstanceID());
		}
		BulletManager.Instance.PushBullet(this);
	}
}