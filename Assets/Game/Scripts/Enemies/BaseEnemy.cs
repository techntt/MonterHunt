using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class BaseEnemy : Damageable
{
    #region Inspector Variables
    public string id;
    public Transform mTrans;
    public TextMeshPro textHP;
    public Rigidbody2D mRigi;
    public SpriteRenderer mRender;
    public CircleCollider2D mColl;
    public float speed = 1f, initSpeed = 1f;
    public bool inScreen;
    #endregion;

    #region Member Variables
    private ENEMY_STYLE style;
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
                mRender.color = PopupManager.GetColorByHP((int)hp, EnemySpawer.Instance.maxHP);
                textHP.text = ""+PopupManager.BigIntToString((int)hp);
            }
                
        }
    }
    #endregion;

    #region Unity Methods

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag(Const.TAG_WALL) && inScreen)
        {
            string colName = col.gameObject.name;
            if (colName.Equals("left"))
            {
                mRigi.velocity = Vector2.Reflect(mRigi.velocity, Vector2.right);
            }else if (colName.Equals("right"))
            {
                mRigi.velocity = Vector2.Reflect(mRigi.velocity, Vector2.left);
            }
        }
    }
    #endregion;

    #region Public Methods
    public void Init(ENEMY_STYLE style, int hp, Vector2 direct, float speed)
    {
        inScreen = Camera189.gameView.Contains(mTrans.position);
        isDead = false;
        mColl.enabled = true;
        this.maxHp = hp;
        HP = maxHp;
        if (!ColliderRef.Instance.DamageableRef.ContainsKey(mColl.GetInstanceID()))
            ColliderRef.Instance.DamageableRef.Add(mColl.GetInstanceID(), this);
        this.speed = speed;
        gameObject.SetActive(true);
        mRigi.velocity = direct * speed;
    }


    public override void TakeDamage(float damage)
    {
        if (!isDead)
        {
            HP -= damage;
            if (HP >= 1)
            {
                TakeDamageEffect();
            }
            else
            {
                Die();
            }
        }
    }

    #endregion;

    #region Private Methods

    public void Die()
    {
        Deactive();
        GameEventManager.Instance.OnEnemyExplode(this);
    }

    public void Deactive()
    {
        isDead = true;
        mColl.enabled = false;
        if (ColliderRef.Instance.DamageableRef.ContainsKey(mColl.GetInstanceID()))
            ColliderRef.Instance.DamageableRef.Remove(mColl.GetInstanceID());
        EnemyManager.Instance.PushEnemy(this);
    }


    bool isAffect = false;
    protected void TakeDamageEffect()
    {
        if (!isAffect)
        {
            isAffect = true;
            mTrans.DOPunchScale(new Vector3(0.2f, 0.2f, 0), 0.3f).OnComplete(() =>
            {
                isAffect = false;
            });
        }
    }
    #endregion;
}
public enum ENEMY_STYLE
{
    NORMAL,
    HARDER,
    BOOM,
    BONUS,
    CRAZY,
    NONE
}
