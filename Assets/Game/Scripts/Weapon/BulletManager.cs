using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletManager : SingletonMonoBehaviour<BulletManager> {

	public Bullet sample;
	public Stack<Bullet> pool = new Stack<Bullet>();
	public Sprite[] bulletSprite;
	public ParticleSystem impactEffect;
	Stack<ParticleSystem> effectPool = new Stack<ParticleSystem>();

	void Start () {
		List<Bullet> b = new List<Bullet>();
		for (int i = 0; i < 75; i++) {
			b.Add(PopBullet(BulletType.Bloom));
		}
		for (int i = 0; i < 75; i++)
			PushBullet(b[i]);
		if (PlayerSettingData.Instance.graphic == GRAPHIC_QUALITY.HIGH && PopupManager.Instance.scene == SCENE.GAME) {
			for (int i = 0; i < 20; i++) {
				SpawnEffect(new Vector3 (100, 100), Color.white);
			}
		}
	}

	public Bullet PopBullet (BulletType type, Sprite sprite = null) {
		Bullet b;
		if (pool.Count == 0) {
			b = Instantiate(sample) as Bullet;
			b.transform.parent = transform;
			ColliderRef.Instance.bulletRef.Add(b.myCollider.GetInstanceID(), b);
		} else {
			b = pool.Pop();
			b.gameObject.SetActive(true);
		}
		b.type = type;
		if (type == BulletType.Normal) {
			if (!sprite)
				b.myRender.sprite = GameManager.Instance.player1.myBullet;
			else
				b.myRender.sprite = sprite;
		} else
			b.myRender.sprite = bulletSprite[(int)type];
		b.myCollider.size = b.myRender.sprite.bounds.size;
		return b;
	}

	public void PushBullet (Bullet b) {
		b.gameObject.SetActive(false);
		pool.Push(b);
	}

	public ParticleSystem SpawnEffect (Vector3 pos, Color color) {
		ParticleSystem s;
		if (effectPool.Count == 0) {
			s = Instantiate(impactEffect) as ParticleSystem;
			s.transform.parent = transform;
		} else {
			s = effectPool.Pop();
			s.gameObject.SetActive(true);
		}
		s.transform.position = pos;
		ParticleSystem.MainModule m = s.main;
		m.startColor = color;
		s.Clear();
		s.Play();
		StartCoroutine(PoolEffect(s));
		return s;
	}

	IEnumerator PoolEffect (ParticleSystem s) {
		yield return new WaitForSeconds(s.main.duration);
		s.gameObject.SetActive(false);
		effectPool.Push(s);
	}
}

public enum BulletType {
	Bloom,
	Divide30,
	Divide180,
	Divide360,
	Normal,
	Rocket,
	Rotate,
	Slow,
	None
}