using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : SingletonMonoBehaviour<EnemyManager>
{
    #region Member Varibales
    private Transform mTrans;
    private Dictionary<string, Stack<BaseEnemy>> ePool;
    #endregion;

    #region Unity Methods
    private void Awake()
    {
        mTrans = transform;
        ePool = new Dictionary<string, Stack<BaseEnemy>>();
    }
    #endregion;

    #region Public Methods
    public void PrepareEnemy(string id,int number =5)
    {
        Stack<BaseEnemy> stack = new Stack<BaseEnemy>();
        if (ePool.ContainsKey("enemy_" + id))
        {
            ePool["enemy_" + id] = stack;
            if(stack.Count >= 5)
                return;
        }
            
        for (int i = 0; i < number; i++)
        {
            GameObject go = Instantiate(Resources.Load<GameObject>(Const.ENEMY+id)) as GameObject;
            BaseEnemy be = go.GetComponent(typeof(BaseEnemy)) as BaseEnemy;
            be.mTrans.SetParent(mTrans);
            go.SetActive(false);
            stack.Push(be);
        }
        if (ePool.ContainsKey("enemy_" + id))
            ePool["enemy_" + id] = stack;
        else
            ePool.Add("enemy_" + id,stack);
    }

    public BaseEnemy PopEnemy(string id)
    {
        BaseEnemy be = null;
        if (ePool.ContainsKey("enemy_" + id))
        {
            Stack<BaseEnemy> stack = ePool["enemy_" + id];
            if (stack.Count > 0)
                be = stack.Pop();
        }
        if (be == null)
        {
            GameObject go = Instantiate(Resources.Load<GameObject>(Const.ENEMY + id)) as GameObject;
            be = go.GetComponent(typeof(BaseEnemy)) as BaseEnemy;
            be.mTrans.SetParent(mTrans);
            go.SetActive(false);
        }
        return be;
    }

    public void PushEnemy(BaseEnemy be)
    {
        Stack<BaseEnemy> stack = new Stack<BaseEnemy>();
        if (ePool.ContainsKey("enemy_" + be.id))
        {
            stack = ePool["enemy_" + be.id];
        }
        else
        {
            stack.Push(be);
            ePool.Add("enemy_" + be.id, stack);
        }
        be.gameObject.SetActive(false);
    }
    #endregion;

    #region Private Methods
    #endregion;
}
