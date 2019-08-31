using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Circle : Damageable {

    public HealthBar health;
    public SpriteRenderer myRender;
	public CircleCollider2D myCollider;
	public Rigidbody2D myBody;
	public AudioClip explodeSfx;

	public CircleType type;
	public CircleOrbit orbit;

	public float size;
	public float initSpeed;
	public float speed;
	/// <summary>
	/// was this circle enter the scene?
	/// </summary>
	public bool wasInScene;
	bool isRefected;
	/// <summary>
	/// does this circle give player scores and coins on death
	/// </summary>
	public bool hasScore;
	public bool hasCoin;
	public bool hasBonus;

    public Transform trans;

    public void Init (int hp, CircleOrbit orbit, float speed, bool hasReward, bool hasCoin = true, bool hasBonus = true) {
		wasInScene = false;
		isRefected = false;
		isDead = false;
		this.hasScore = hasReward;
		this.hasCoin = hasCoin;
		this.hasBonus = hasBonus;
		maxHp = hp;
		this.hp = hp;
        trans.localScale = new Vector3(1, 1, 1);
        health.Init(myRender.size.y,hp);
		initSpeed = speed;
		this.speed = speed;
		if (type == CircleType.HARDEN)
			damage = 1000;
		else
			damage = GameManager.GetLinearValueSimilarTo(1, CircleSpawner.Instance.maxHP, 1, 50, hp);
		this.orbit = orbit;
		switch (orbit) {
			case CircleOrbit.D:
				myBody.velocity = Vector2.down * speed;
				break;
			case CircleOrbit.L:
				myBody.velocity = Quaternion.Euler(0, 0, -45) * Vector2.right * speed;
				break;
			case CircleOrbit.R:
				myBody.velocity = Quaternion.Euler(0, 0, 45) * Vector2.left * speed;
				break;
			case CircleOrbit.ZL:
				myBody.velocity = Quaternion.Euler(0, 0, Random.Range(-53, -35)) * Vector2.right * speed;
				break;
			case CircleOrbit.ZR:
				myBody.velocity = Quaternion.Euler(0, 0, Random.Range(35, 53)) * Vector2.left * speed;
				break;
		}
		if (hasReward)
			GameEventManager.Instance.OnCircleSpawned(this);
	}

//    public void OnStop()
//    {
//        myBody.velocity = Vector2.zero;
//    }
//
//    public float Sub1HP()
//    {       
//        hp -= 1;
//        return hp;
//    }

	void OnTriggerEnter2D (Collider2D col) {
//		if (!isDead && !isSucked) {
		if (!isDead) {
			if (col.CompareTag(Const.WALL_LEFT)) {
				if (orbit == CircleOrbit.ZR || (orbit == CircleOrbit.ZL && isRefected)) {
					isRefected = true;
					myBody.velocity = Vector2.Reflect(myBody.velocity, Vector2.right);
				}
			} else if (col.CompareTag(Const.WALL_RIGHT)) {
				if (orbit == CircleOrbit.ZL || (orbit == CircleOrbit.ZR && isRefected)) {
					isRefected = true;
					myBody.velocity = Vector2.Reflect(myBody.velocity, Vector2.left);
				}
			}
        }
	}

    bool isScale = false;
	public override void TakeDamage (float damage) {
		if (wasInScene) {
			if (type == CircleType.HARDEN)
				damage /= 3;
			if (!isDead) {
				hp -= damage;
                Vector2 txtPos = trans.position;
                txtPos.y += (myRender.size.y / 2 + 0.15f);
                BaseEffect dmgText =  EffectManager.Instance.SpawnEffect(EFFECT_TYPE.DAMAGE_TEXT, txtPos);
                dmgText.Init(damage);
				if (hp > 0)
                {
                    if (!isScale)
                    {
                        isScale = true;
                        trans.DOPunchScale(new Vector3(0.3f, 0.3f, 1f), 0.2f).OnComplete(() => {
                            isScale = false;
                        });
                    }   
                    health.SetHealthBar(hp);
				} else {
					Die();
				}
			}
		}
	}

	public override void Die () {
		isDead = true;
		SoundManager.Instance.PlaySfx(explodeSfx, SFX_PLAY_STYLE.DONT_REWIND);
		// drop coin and daily item
		if (hasCoin) {
			int numOfCoin = CircleSpawner.GetNumberOfCoinBySize(size);
			for (int i = 0; i < numOfCoin; i++) {
				Coin c = CoinManager.Instance.PopCoin();
				c.transform.position = transform.localPosition;
				c.Init();
			}			
		}
		if (hasScore) {
			// add score, create effect adding score
			int score = GameManager.Instance.player1.xPoint * GameManager.Instance.player1.shipXPoint;
			TextEffect t = (TextEffect)EffectManager.Instance.SpawnEffect(EFFECT_TYPE.FLOAT_TEXT, transform.position);
			if (GameManager.Instance.player1.xPoint <= 1)
				t.Init("+" + score, 25, Color.white);
			else
				t.Init("+" + score, 40, Color.green);
			GameEventManager.Instance.OnPlayerGetScore(GameManager.Instance.player1, score);
			GameEventManager.Instance.OnCircleExplode(this);
		}
		if (hasBonus) {
			// drop bonus
			if (type == CircleType.GLOW || Random.Range(0, 100) < CircleSpawner.Instance.bonusDropChance) {
				Bonus b = BonusManager.Instance.GetBonus(BonusManager.Instance.GetRandomBonusType());
				if (b != null) {
					b.transform.position = transform.localPosition;
					b.Init();
				}
			}
		}
		if (type == CircleType.BOMB) {
			ExplodeEffect e = (ExplodeEffect)EffectManager.Instance.SpawnEffect(EFFECT_TYPE.EXPLODE, transform.localPosition);
			e.Init(5, CircleSpawner.Instance.maxHP);
			Camera.main.DOShakePosition(1, 0.2f, 20);
		}
		CircleManager.Instance.PushCircle(this);

	}

	void OnBecameInvisible () {
		if (wasInScene && gameObject.activeSelf) {
			if (hasScore)
				GameEventManager.Instance.OnCircleExit(this);
			CircleManager.Instance.PushCircle(this);
		}
	}

	void OnBecameVisible () {
		wasInScene = true;
	}
}

public class Damageable : MonoBehaviour {
	public bool isDead;
	public float damage;

	public float maxHp;
	public float hp;

	public virtual void TakeDamage (float damage) {
	}

	public virtual void Die () {
	}
}