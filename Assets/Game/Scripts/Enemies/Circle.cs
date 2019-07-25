using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Circle : Damageable {

	public UnityEngine.UI.Text hpText;
	public CircleCollider2D myCollider;
	public SpriteRenderer myRender;
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

    // truong.nt
//    private bool sucking = false;
//    public bool isSucked = false;
//    private SpecialObject spObj;
//    private Gravity grav;
    [HideInInspector] public Transform trans;

    public void Init (int hp, CircleOrbit orbit, float speed, bool hasReward, bool hasCoin = true, bool hasBonus = true) {
		wasInScene = false;
		isRefected = false;
		isDead = false;
		this.hasScore = hasReward;
		this.hasCoin = hasCoin;
		this.hasBonus = hasBonus;
		maxHp = hp;
		this.hp = hp;
		myRender.color = GameManager.GetColorByHP(hp, CircleSpawner.Instance.maxHP);
		hpText.text = "" + hp;
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
//			else if (col.CompareTag(Const.DUMMY_GRAVITY)){ // for dummy object have gravity  
//                GravityCollider gc = col.gameObject.GetComponent(typeof(GravityCollider)) as GravityCollider;
//                spObj = gc.spObj;
//                grav = spObj.GetComponent(typeof(Gravity)) as Gravity;                   
//                if(grav.canAdd()&& spObj.myCollider.radius > myCollider.radius)
//                {
//                    grav.currChilds++;
//                    myBody.velocity = Vector3.zero;
//                    sucking = true;
//                }               
//            }
        }
	}

//    private void Update()
//    {
//        if (sucking && spObj != null)
//        {
//            trans.position = Vector2.MoveTowards(trans.position, spObj.trans.position, 3f * Time.deltaTime);
//            float distance = Vector2.Distance(trans.position, spObj.trans.position);            
//            if(distance <= spObj.myCollider.radius)
//            {
//                sucking = false;
//                isSucked = true;                
//                grav.AddChild(gameObject.GetComponent(typeof(Circle)) as Circle);
//            }
//        } 
//    }

	public override void TakeDamage (float damage) {
		if (wasInScene) {
			if (type == CircleType.HARDEN)
				damage /= 3;
			if (!isDead) {
				hp -= damage;
				if (hp > 0) {
					myRender.color = GameManager.GetColorByHP(Mathf.CeilToInt(hp), CircleSpawner.Instance.maxHP);
					if (type != CircleType.HARDEN) {
						this.damage = GameManager.GetLinearValueSimilarTo(1, CircleSpawner.Instance.maxHP, 1, 50, hp);
						this.damage = Mathf.CeilToInt(this.damage);
					}
					hpText.text = "" + Mathf.CeilToInt(hp);
				} else {
					Die();
				}
			}
		}
	}

	public override void Die () {
		isDead = true;
		SoundManager.Instance.PlaySfx(explodeSfx, SFX_PLAY_STYLE.DONT_REWIND);
		if (PlayerSettingData.Instance.graphic == GRAPHIC_QUALITY.HIGH)
			CircleManager.Instance.SpawnExplodeEffect(transform.position, myRender.color);
		// drop coin and daily item
		if (hasCoin) {
			int numOfCoin = CircleSpawner.GetNumberOfCoinBySize(size);
			for (int i = 0; i < numOfCoin; i++) {
				Coin c = CoinManager.Instance.PopCoin();
				c.transform.position = transform.localPosition;
				c.Init();
			}
			if (DailyQuestManager.Instance.status == DAILY_QUEST_STATUS.ACTIVE) {
				if (Random.Range(0, 100) < DailyQuestManager.Instance.dropChance) {
					DailyItem d = DailyItemManager.Instance.PopItem();
					d.transform.position = transform.localPosition;
					d.Init();
				}
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
	/// <summary>
	/// damage of object, used when it collides with player
	/// </summary>
	public float damage;

	public float maxHp;
	public float hp;

	public virtual void TakeDamage (float damage) {
	}

	public virtual void Die () {
	}
}