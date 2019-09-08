using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : SingletonMonoBehaviour<EnemyManager>
{
    #region Member Varibales
    private Transform mTrans;
    private Queue<BaseEnemy> ePool;
    public GameObject enemySample;
    public Sprite[] sprites;
    #endregion;

    #region Unity Methods
    private void Awake()
    {
        mTrans = transform;
        ePool = new Queue<BaseEnemy>();
        PrepareEnemy();
    }
    #endregion;

    #region Public Methods
    public void PrepareEnemy(int number =5)
    {
        for (int i = 0; i < number; i++)
        {
            GameObject go = Instantiate(enemySample) as GameObject;
            BaseEnemy be = go.GetComponent(typeof(BaseEnemy)) as BaseEnemy;
            be.mTrans.SetParent(mTrans);
            go.SetActive(false);
            ePool.Enqueue(be);
        }
    }

    public BaseEnemy PopEnemy(ENEMY_STYLE style,float size, Vector2 startPos)
    {
        BaseEnemy be = null;
        if (ePool.Count > 0)
            be = ePool.Dequeue();
        if (be == null)
        {
            GameObject go = Instantiate(enemySample) as GameObject;
            be = go.GetComponent(typeof(BaseEnemy)) as BaseEnemy;
            be.mTrans.SetParent(mTrans);
            go.SetActive(false);
        }
        be.mRender.sprite = sprites[(int)style];
        be.mTrans.position = startPos;
        be.mRender.size = new Vector2(size, size);
        be.mColl.radius = size / 2;
        return be;
    }

    public void PushEnemy(BaseEnemy be)
    {
        be.gameObject.SetActive(false);
        ePool.Enqueue(be);
    }
    #endregion;

    #region Private Methods
    #endregion;
}
