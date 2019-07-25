using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragonBoss : MonoBehaviour {
    #region Inspector Variables
    public AutoBoss auto;
    #endregion;

    #region Member Variables
    private int gunnum;
    #endregion;

    #region Unity Methods
    private void Awake()
    {
        gunnum = 0;
        //auto.OnBossAction += OnDragonAction;
        //auto.OnBossAction1 += OnDragonAction1;
        auto.OnBossAttack += OnDragonAttack;
        auto.OnBossAttack1 += OnDragonAttack1;
        auto.OnBossAttack2 += OnDragonAttack2;
    }
    #endregion;

    #region Public Methods
    public void DragonAction()
    {

    }

    public void DragonAction1()
    {

    }

    public void DragonAttack() // left hand
    {
        gunnum = 0;
        auto.bullets[gunnum].Shot();
    }

    public void DragonAttack1() // right hand
    {
        gunnum = 1;
        auto.bullets[gunnum].Shot();
        auto.OnCallMakeDicision(Random.Range(auto.minAtkTimeDelay,auto.maxAtkTimeDelay));
    }

    public void DragonAttack2()
    {
        auto.bullets[gunnum].Shot();
        auto.OnCallMakeDicision(Random.Range(auto.minAtkTimeDelay, auto.maxAtkTimeDelay));
    }
    #endregion;

    #region Private Methods
    private void OnDragonAction()
    {

    }

    private void OnDragonAction1()
    {

    }

    private void OnDragonAttack()
    {
        auto.boss.myAnim.SetTrigger(AnimConst.attack);
    }

    private void OnDragonAttack1()
    {
        gunnum = 2;
        auto.boss.myAnim.SetTrigger(AnimConst.attack1);
    }

    private void OnDragonAttack2()
    {
        gunnum = 3;
        auto.boss.myAnim.SetTrigger(AnimConst.attack1);
    }
    #endregion;

}
