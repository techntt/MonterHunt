using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class OneShot : Weapon {

	public ParticleSystem myParticle;
	public ChainEffect[] chains;
	bool isChaining;

	public override void Start () {
		base.Start();
		myParticle.transform.SetParent(GameManager.Instance.player1.firePos);
		myParticle.transform.localPosition = Vector3.zero;
	}

	public override void Activate () {
		base.Activate();
		switch (WeaponManager.Instance.lastValue) {
			case 0: // bomb
				Bomb();
				break;
			case 1: // rocket
				Rocket();
				break;
			case 2: // barrier
				Barrier();
				break;
			case 3: // seek
				Seek();
				break;
			case 4: // lightning
				Lightning();
				break;
		}
	}

	public void Bomb () {
		ExplodeEffect e = (ExplodeEffect)EffectManager.Instance.SpawnEffect(EFFECT_TYPE.EXPLODE, GameManager.Instance.player1.transform.position);
		e.Init(10, WeaponDataCalculator.BOMB_DAMAGE * GameManager.Instance.player1.baseDamage);
	}

	public void Rocket () {
		Bullet b = BulletManager.Instance.PopBullet(BulletType.Rocket);
		b.transform.position = GameManager.Instance.player1.firePos.position;
		Vector3 des = new Vector3(0, 3, 0);
		b.transform.rotation = Quaternion.Euler(0, 0, Vector2.Angle(Vector2.right, des - b.transform.position));
		b.Init(0, Color.white, 0);
		b.target = null;
		b.transform.DOMove(des, 3).SetSpeedBased(true).OnComplete(() => {
			ExplodeEffect e = (ExplodeEffect)EffectManager.Instance.SpawnEffect(EFFECT_TYPE.EXPLODE, b.transform.position);
			e.Init(10, WeaponDataCalculator.ROCKET_DAMAGE * GameManager.Instance.player1.baseDamage);
			b.gameObject.SetActive(false);
		});
	}

	public void Barrier () {
		Bullet b = BulletManager.Instance.PopBullet(BulletType.Bloom);
		b.transform.position = GameManager.Instance.player1.firePos.position;
		b.transform.rotation = Quaternion.Euler(0, 0, 90);
		b.Init(2, Color.white, WeaponDataCalculator.BARRIER_DPS * GameManager.Instance.player1.baseDamage);
	}

	public void Seek () {
		List<Damageable> list = FindActiveEnemies();
		if (list.Count == 0)
			return;
		List<Damageable> target = new List<Damageable>();
		for (int i = 0; i < 3; i++) {
			int c = Random.Range(0, list.Count);
			target.Add(list[c]);
			list.RemoveAt(c);
			if (list.Count == 0)
				break;
		}
		for (int i = 0; i < target.Count; i++) {
			Bullet b = BulletManager.Instance.PopBullet(BulletType.Rocket);
			b.transform.position = GameManager.Instance.player1.firePos.position;
			b.transform.rotation = Quaternion.Euler(0, 0, 90);
			b.target = target[i];
			b.Init(8, Color.white, WeaponDataCalculator.SEEK_DAMAGE * GameManager.Instance.player1.baseDamage);
		}
	}

	public void Lightning () {
		if (!isChaining) {
			isChaining = true;
			List<Circle> list = FindActiveCircles();
			if (list.Count == 0)
				return;
			List<Circle> target = new List<Circle>();
			for (int i = 0; i < 3; i++) {
				int c = Random.Range(0, list.Count);
				target.Add(list[c]);
				list.RemoveAt(c);
				if (list.Count == 0)
					break;
			}
			myParticle.Clear();
			ParticleSystem.MainModule main = myParticle.main;
			main.startColor = GameManager.Instance.player1.myRender.color;
			myParticle.gameObject.SetActive(true);
			StartCoroutine(ILightning(target));
		}
	}

	IEnumerator ILightning (List<Circle> target) {
		int count = target.Count > chains.Length ? chains.Length : target.Count;
		for (int i = 0; i < count; i++) {
			chains[i].Init(target[i]);
		}
		yield return new WaitForSeconds(1);
		for (int i = 0; i < count; i++) {
			chains[i].Deactivate();
		}
		myParticle.gameObject.SetActive(false);
		target.Clear();
		isChaining = false;
	}

	public List<Damageable> FindActiveEnemies () {
		IList<Damageable> enemies = ColliderRef.Instance.DamageableRef.Values;
		List<Damageable> activeEnemies = new List<Damageable>();
		for (int i = 0; i < enemies.Count; i++) {
			if (enemies[i].gameObject.activeInHierarchy) {
				activeEnemies.Add(enemies[i]);
			}
		}
		return activeEnemies;
	}

	public List<Circle> FindActiveCircles () {
		IList<Damageable> enemies = ColliderRef.Instance.DamageableRef.Values;
		List<Circle> activeEnemies = new List<Circle>();
		for (int i = 0; i < enemies.Count; i++) {
			if (enemies[i].gameObject.activeInHierarchy && enemies[i] is Circle) {
				activeEnemies.Add((Circle)enemies[i]);
			}
		}
		return activeEnemies;
	}
}