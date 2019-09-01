using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnemy : Damageable
{
    #region Inspector Variables
    public string id;
    public Transform mTrans;
    public HealthBar hpBar;
    public Rigidbody2D mRigi;
    public SpriteRenderer mRender;
    public CircleCollider2D mColl;
    #endregion;

    #region Member Variables
    [HideInInspector]
    public float speed =1f , initSpeed = 1f;
    #endregion;

    #region Unity Methods
    #endregion;

    #region Public Methods
    public virtual void Init()
    {

    }

    public virtual void Die()
    {

    }
    #endregion;

    #region Private Methods
    #endregion;
}
