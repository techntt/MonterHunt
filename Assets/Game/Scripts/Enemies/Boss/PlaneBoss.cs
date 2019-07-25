using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlaneBoss : MonoBehaviour {

    #region Inspector Variables
    public AutoBoss auto;
    #endregion;

    #region Unity Methods
    private void Awake()
    {
        auto.OnBossAttack += PlaneAttack;
        auto.OnBossAttack1 += PlaneAttack1;
        auto.OnBossAttack2 += PlaneAttack2;
        //auto.OnBossAction += PlaneAction;
        //auto.OnBossAction1 += PlaneAction1;
    }
    #endregion;

    #region Private Methods
    private void PlaneAction()
    {
       
    }

    private void PlaneAction1()
    {

    }
    private void PlaneAttack()
    {
        auto.boss.myAnim.SetTrigger(AnimConst.attack);        
    }

    private void PlaneAttack1()
    {
        auto.boss.myAnim.SetTrigger(AnimConst.attack1);        
    }

    private void PlaneAttack2()
    {
        auto.boss.myAnim.SetTrigger(AnimConst.attack2);       
    }
    #endregion;

    #region Public Methods
    public void OnPlaneAttack()
    {
        Debug.Log("Attack");
        auto.bullets[0].Shot();
        auto.currState = AutoBoss.AI_STATE.IDLE;
        auto.OnChangeState();
    }

    public void OnPlaneAttack1()
    {
        Debug.Log("Attack1");
        auto.bullets[1].Shot();
        auto.currState = AutoBoss.AI_STATE.IDLE;
        auto.OnChangeState();
    }

    public void OnPlaneAttack2()
    {
        Debug.Log("Attack2");
        auto.bullets[2].Shot();
        auto.currState = AutoBoss.AI_STATE.IDLE;
        auto.OnChangeState();
    }
    #endregion;
}
