using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Spirit : BasePath {

    #region Inspector Variables
    public Vector2 startPos;
    #endregion;

    #region Member Variables
    private SpecialObject spObj;
    private Transform pTrans;
    private enum state
    {
        moveToDefense,
        defense,
        moveToAttack,
        attack        
    }
    private state currState;
    private Vector2 lastPos;
    private Vector2 targetPos;
    private float time;
    private float timeMove;
    private float totalTime;
    
    #endregion;

    #region Unity Methods
    private void Awake()
    {
        spObj = GetComponent(typeof(SpecialObject)) as SpecialObject;
        spObj.type = SpecialObject.SPOBJ_TYPE.SPIRIT;
        spObj.OnChildInit += Init;
    }

    private void FixedUpdate()
    {        
        if (spObj != null)
        {
            switch (currState)
            {
                case state.defense:
                    Defense();
                    break;
                case state.moveToAttack:
                    MoveToAttack();
                    break;
                case state.attack:
                    Attack();
                    break;
                case state.moveToDefense:
                    MoveToDefense();
                    break;
            }
        }
    }
    #endregion;


    #region Private Methods

    private void Init()
    {        
        spObj.trans.position = startPos;
        pTrans = GameManager.Instance.player1.transform;
        currState = state.moveToDefense;
        targetPos = Vector2.zero;
        resetTime();        
        //StartCoroutine(Controller());
    }

    private void resetTime()
    {
        timeMove = 0;
        lastPos = spObj.trans.position;
        float distace = Vector2.Distance(lastPos, targetPos);
        totalTime = distace / speed;
        time = 0;
    }
        

    private void Attack()
    {
        time++;
        if (time >= 100)
        {
            float x = Random.Range(-2.6f, 2.6f);
            float y = Random.Range(0, 3.5f);
            targetPos = new Vector2(x, y);
            resetTime();
            nextState();
            if (spObj.spiritEffect.activeSelf)
                spObj.spiritEffect.SetActive(false);
        }
    }

    private void Defense()
    {
        time++;
        if (time >= 100)
        {
            targetPos = pTrans.position;
            resetTime();
            nextState();
            spObj.spiritEffect.SetActive(true);
        }
        
    }

    private void MoveToAttack()
    {

        if (timeMove <= totalTime)
        {
            float step = timeMove / totalTime;
            spObj.trans.position = Vector2.Lerp(lastPos, targetPos, step);
            timeMove += Time.deltaTime * 3f;            
        }
        else
        {
            nextState();
        }
            
    }

    private void MoveToDefense()
    {
        if (timeMove <= totalTime)
        {
            float step = timeMove / totalTime;
            spObj.trans.position = Vector2.Lerp(lastPos, targetPos, step);
            timeMove += Time.deltaTime;
        }
        else
        {
            nextState();
        }
    }
    private void nextState()
    {
        currState++;
        if (currState > state.attack)
            currState = 0;
    }
    #endregion;

}
