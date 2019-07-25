using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialObject : Damageable {

    #region Inspector Variables
    public UnityEngine.UI.Text hpText;
    public CircleCollider2D myCollider;
    public SpriteRenderer myRender;
    public Rigidbody2D myBody;
    public Animator myAmim;
    public CircleCollider2D gravityCollider;
    public GameObject spiritEffect;
    #endregion;

    #region Member Variables
    private int id = -1;
    private float initHP;
    private float size;
    private bool hasCoin, hasScore;
    [HideInInspector] public bool wasInScene;

    public delegate void SimpleEvent();
    public event SimpleEvent OnChildInit;
    public event SimpleEvent OnInitPos;
    public event SimpleEvent OnDetachChild;
    public event SimpleEvent OnAttackTriger;

    [HideInInspector] public Transform trans;

    [HideInInspector] public SPOBJ_TYPE type = SPOBJ_TYPE.NORMAL;
    #endregion;

    #region Unity Methods   
    private void Awake()
    {
        trans = gameObject.GetComponent(typeof(Transform)) as Transform;
    }   
    #endregion;

    #region Public Methods        
    /// <summary>
    /// Set hp for enemy, also change color depend on hp value
    /// </summary>
    public float HP
    {
        get
        {
            return hp;
        }
        set
        {
            hp = value;
            if (hp > 0)
            {
                myRender.color = GameManager.GetColorByHP(Mathf.CeilToInt(hp), CircleSpawner.Instance.maxHP);
                damage = Mathf.CeilToInt(hp);
                hpText.text = "" + (int)damage;
            }
        }
    }

    public void Init(int hp, float size, bool hasScore = false, bool hasCoin = true)
    {
        wasInScene = false;
        isDead = false;
        initHP = hp;
        this.HP = hp;
        this.size = size;
        this.hasScore = hasScore;
        this.hasCoin = hasCoin;
        myCollider.radius = size / 2;
        myRender.size = new Vector2(size, size);
    }

    /// <summary>
    /// Only parent object have id
    /// </summary>
    public int Id
    {        
        get{
            return id;
        }
        set
        {
            this.id = value;
            if (OnChildInit != null)
                OnChildInit();
            if (OnInitPos != null)
                OnInitPos();            
        }
        
    }
	public override void TakeDamage (float damage) {
		if (!isDead&&wasInScene) {
            if (OnAttackTriger != null)            
                OnAttackTriger();
            HP -= damage;  
            if (HP <= 0)
            {
                Die();
            }
    }
	}

	public override void Die () {
		isDead = true;
		if (PlayerSettingData.Instance.graphic == GRAPHIC_QUALITY.HIGH)
			CircleManager.Instance.SpawnExplodeEffect(transform.position, myRender.color);
        // some code for reward
        // drop coin
        if (hasCoin)
        {
            int numOfCoin = CircleSpawner.GetNumberOfCoinBySize(size);
            for (int i = 0; i < numOfCoin; i++)
            {
                Coin c = CoinManager.Instance.PopCoin();
                c.transform.position = transform.position;
            }
        }

        if (hasScore)
        {
            // add score, create effect adding score
            int score = GameManager.Instance.player1.xPoint * (GameManager.Instance.player1.shipXPoint + CampaignManager.campaign.id);
            TextEffect t = (TextEffect)EffectManager.Instance.SpawnEffect(EFFECT_TYPE.FLOAT_TEXT, transform.position);
            if (GameManager.Instance.player1.xPoint <= 1)
                t.Init("+" + score, 25, Color.white);
            else
                t.Init("+" + score, 40, Color.green);
            GameEventManager.Instance.OnPlayerGetScore(GameManager.Instance.player1, score);
        }
        
        DummyObjInvisible();
    }


    public void DummyObjInvisible()
    {
        if (id == -2)
        {
            CircleManager.Instance.PushSPObj(this);
        }
        else
        {
            // Detach childs
            if (OnDetachChild != null)
            {
                OnDetachChild();
            }
            // Disable Object
            gameObject.SetActive(false);
        }
    }   
   
    public enum SPOBJ_TYPE
    {
        NORMAL,
        SATELLITE,
        GRAVITY,
        SPIRIT,
        DIVISION
    }
    #endregion;

    #region Private Methods
    
    #endregion;
}
