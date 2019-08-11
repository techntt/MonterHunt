using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class Player : MonoBehaviour {

    public Color bulletColor;
    public float magnetRadius;

    public delegate void StatEvent();
    public event StatEvent xDamageChange;
    public event StatEvent xPointChange;
    public event StatEvent becomeVulnerable;

    public Transform firePos;
    public CircleCollider2D body;
    public CircleCollider2D bonusBound;
    public CircleCollider2D magnetField;
    public SpriteRenderer myRender;
    public Weapon myBaseWeapon;
    public Sprite myBullet;
    public ParticleSystem[] myPars;
    public ShipData myShipData;
    public ShipUpgradeData myShipUpgradeData;

    public AudioClip sfxDie;
    public AudioClip sfxImpact;

    [HideInInspector] public float baseDamage;
	private float _xdam;

	public float xDamage {
		set { 
			_xdam = value;
			if (xDamageChange != null)
				xDamageChange();
			int percent = (int)(_xdam * 100 + myShipUpgradeData.powerLevel * 10);
			if (percent == 0)
				bulletColor = Color.white;
			else
				bulletColor = GameManager.GetColorByHP(percent, 650);
		}
		get {
			return _xdam;
		}
	}

	private int _xPoint;

	public int xPoint {
		set { 
			_xPoint = value;
			if (xPointChange != null)
				xPointChange();
		}
		get { 
			return _xPoint;
		}
	}

	public int shipXPoint;
	private int _power;

	public int power {
		get { 
			return _power;
		}
        set
        {
            _power = value;
            GameEventManager.Instance.OnPlayerPowerChanged(this, _health);
        }
	}

	public int maxHealth;
	private int _health;

	public int health {
		set {
			int change = _health;
			_health = Mathf.Clamp(value, 0, maxHealth);
			change = _health - change;	
			if (change > 0)
				GameEventManager.Instance.OnPlayerGainHealth(this, change);
			else if (change < 0)
				GameEventManager.Instance.OnPlayerLostHealth(this, change);			
		}
		get {
			return _health;
		}
	}
    
	void Start () {
		if (PopupManager.Instance.scene == SCENE.GAME) {
			int type = PlayerData.Instance.selectedShip;
			myShipData = ShipDataManager.Instance.shipData[type];
			myShipUpgradeData = PlayerData.Instance.shipData[type];
			// add bonus damage from upgrade
			baseDamage = myShipData.baseDamage * (1 + 0.1f * PlayerData.Instance.shipData[type].powerLevel + xDamage);
			xDamage = 0;
			xPoint = 1;
			shipXPoint = CampaignManager.campaign.id + 1;
			maxHealth = 1 ;
			//add bonus health from upgrade
			health = 1;
			// set magnet field radius			
			magnetField.radius = magnetRadius;
			WeaponManager.Instance.weaponList.Add(myBaseWeapon.weaponType, myBaseWeapon);
			GameEventManager.Instance.PlayerGetBonus += HandlePlayerGetBonus;
		}
	}

	public void UpdateStat (int type) {
		myShipUpgradeData = PlayerData.Instance.shipData[type];
		// set xDamage variable to automatically set color of bullet
		xDamage = 0;
		// add bonus damage from upgrade
		baseDamage = myShipData.baseDamage * (1 + 0.1f * PlayerData.Instance.shipData[type].powerLevel + xDamage);
		maxHealth = 0;
		//add bonus health from upgrade
		health = 1;
	}

	void HandlePlayerGetBonus (Player p, BonusType b, int variant) {
		if (b == BonusType.PowerUp)
            if (power <= 50)
                power += 1;
    }

	public void HandleCollision (Collider2D col) {
		// if the collider is a part of boss
		if (col.CompareTag(Const.TAG_BOSS))
			TakeDamage(1000);
		else {
			// get the damageable class based on instanceID of collider
			Damageable d = ColliderRef.Instance.GetDamageable(col.GetInstanceID());
			// player take damage
			TakeDamage(1);
			// if the col is a circle, fire event
			if (d is Circle) {
				Circle c = d as Circle;
				GameEventManager.Instance.OnCircleCollide(c);
				if (c.type != CircleType.HARDEN)
					c.TakeDamage(999999);
			}
			// if the collider is dummy object, it is destroyed
			if (d is DummyObject)
				d.TakeDamage(999999);
		}
	}

	void OnDestroy () {
		xDamageChange = null;
		xPointChange = null;
	}

	void Update () {
		#if UNITY_EDITOR || UNITY_STANDALONE_WIN
		if (Input.GetKey(KeyCode.Space)) {            
            if(power<=50)
                power += 1;
		}
		if (Input.GetKey(KeyCode.S)) {
			GameEventManager.Instance.OnPlayerGetScore(this, 1);
		}
		if (Input.GetKeyDown(KeyCode.Comma)) {
			Bonus b = BonusManager.Instance.GetBonus(BonusManager.Instance.GetRandomBonusType());
			if (b != null) {
				b.transform.position = new Vector3 (0, -6);
				b.Init();
			}
		}
		#endif
	}

	public void TakeDamage (float damage) {
		if (health > 0) {
			health -= Mathf.CeilToInt(damage);
			if (health == 0) {
				Explode();
			} else {
				StartCoroutine(Blink(1));
				CameraShake.Instance.Vibrate(2, 0.2f);
				SoundManager.Instance.PlaySfxOverride(sfxImpact);
			}
		}
	}

	public void Revive () {
		gameObject.SetActive(true);
        health = 1;
		magnetField.radius = magnetRadius;
		WeaponManager.Instance.ActivateWeapon(WeaponType.BASE_WEAPON);
		GameEventManager.Instance.OnPlayerGetBonus(this, BonusType.Saw);
	}
    
	void Explode () {
		SoundManager.Instance.PlaySfx(sfxDie);
		// spawn explosion effect
		EffectManager.Instance.SpawnEffect(EFFECT_TYPE.PLAYER_DEATH, transform.position).Init();
		gameObject.SetActive(false);
		CameraShake.Instance.Vibrate(2, 0.2f);
		GameEventManager.Instance.OnPlayerDead();
	}

	IEnumerator Blink (float duration) {
		body.enabled = false;
		int loop = (int)(duration * 10);
		for (int i = 0; i < loop; i++) {
			myRender.enabled = !myRender.enabled;
			yield return new WaitForSeconds(0.1f);
		}
		myRender.enabled = true;
		body.enabled = true;
		if (becomeVulnerable != null)
			becomeVulnerable();
	}
}