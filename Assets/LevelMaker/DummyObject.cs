using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Dummy object.
/// </summary>
public class DummyObject : Damageable {

	public Collider2D myCollider;
	public SpriteRenderer myRender;
	bool wasInScene;

	public delegate void DummyObjectEvent (DummyObject d);
	public event DummyObjectEvent ObjectInactive;
	public event DummyObjectEvent ObjectDie;

	/// <summary>
	/// init the object
	/// </summary>
	public void Init () {
		damage = GameManager.GetLinearValueSimilarTo(1, CircleSpawner.Instance.maxHP, 1, 50, hp);
		myCollider.enabled = false;
        if(!ColliderRef.Instance.DamageableRef.ContainsKey(myCollider.GetInstanceID()))
		    ColliderRef.Instance.DamageableRef.Add(myCollider.GetInstanceID(), this);
		myRender.color = GameManager.GetColorByHP(Mathf.CeilToInt(hp), (int)maxHp);
		wasInScene = false;
        isDead = false;
        if(!gameObject.activeSelf)
		    gameObject.SetActive(true);
	}

    public void NewInit()
    {
        damage = GameManager.GetLinearValueSimilarTo(1, CircleSpawner.Instance.maxHP, 1, 50, hp);
        myCollider.enabled = true;
        if(!ColliderRef.Instance.DamageableRef.ContainsKey(myCollider.GetInstanceID()))
            ColliderRef.Instance.DamageableRef.Add(myCollider.GetInstanceID(), this);
        myRender.color = GameManager.GetColorByHP(Mathf.CeilToInt(hp), (int)maxHp);
        isDead = false;
        wasInScene = true;
    }

    public void Reset()
    {
        damage = 0;
        hp = 0;
        myRender.color = Color.white;
        myCollider.enabled = false;
    }

    public override void TakeDamage (float damage) {
		if (wasInScene) {
			if (!isDead) {
				hp -= damage;
                
				if (hp <= 0) {
					Die();
				} else {
                    myRender.color = GameManager.GetColorByHP(Mathf.CeilToInt(hp), (int)maxHp);
                    this.damage = GameManager.GetLinearValueSimilarTo(1, CircleSpawner.Instance.maxHP, 1, 50, hp);
					this.damage = Mathf.CeilToInt(damage);
				}
			}
		}
	}

	public override void Die () {
		isDead = true;
		if (PlayerSettingData.Instance.graphic == GRAPHIC_QUALITY.HIGH)
			CircleManager.Instance.SpawnExplodeEffect(transform.position, myRender.color);
		if (ObjectDie != null)
			ObjectDie(this);
		gameObject.SetActive(false);
	}

	void OnBecameVisible () {
		wasInScene = true;
		myCollider.enabled = true;
	}

	void OnBecameInvisible () {
		if (wasInScene) {
			if (ObjectInactive != null)
			    ObjectInactive(this);
		}
	}
}