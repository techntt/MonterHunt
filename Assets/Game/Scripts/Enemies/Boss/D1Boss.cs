using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class D1Boss : MonoBehaviour
{

    #region Inspector variables
    public AutoBoss auto;
    #endregion;

    #region Member Variables
    private int gunnum;
    private bool isAttack2;
    #endregion;

    #region Unity Methods
    private void Awake()
    {
        //auto.OnBossAction += OnD1Action;
        //auto.OnBossAction1 += OnD1Action1;
        auto.OnBossAttack += OnD1Attack;
        auto.OnBossAttack1 += OnD1Attack1;
        auto.OnBossAttack2 += OnD1Attack2;
        gunnum = 0;
        isAttack2 = false;
    }
    #endregion;

    #region Private Methods
    private void OnD1Action()
    {

    }

    private void OnD1Action1()
    {

    }

    private void OnD1Attack()
    {
        gunnum = 0;
        auto.boss.myAnim.SetTrigger(AnimConst.attack);
    }

    private void OnD1Attack1()
    {
        auto.boss.myAnim.SetTrigger(AnimConst.attack1);
    }

    private void OnD1Attack2()
    {
        gunnum = 3;
        isAttack2 = true;
        auto.boss.myAnim.SetTrigger(AnimConst.attack2);
    }

    private IEnumerator WaitFinishGun()
    {
        yield return new WaitForSeconds(0.5f);
        if (auto.bullets[gunnum]._Shooting)
        {
            StartCoroutine(WaitFinishGun());
        }
        else
        {
            auto.boss.myAnim.SetTrigger(AnimConst.idle);
            auto.OnCallMakeDicision(Random.Range(auto.minAtkTimeDelay,auto.maxAtkTimeDelay));
        }
    }
    #endregion;

    #region Public Methods
    
    public void D1Attack()
    {
        auto.bullets[gunnum].Shot();
        StartCoroutine(WaitFinishGun());
    }

    public void D1Attack1()
    {
        gunnum = 1;
        auto.bullets[gunnum].Shot();
        gunnum = 2;
        auto.bullets[gunnum].Shot();
        StartCoroutine(WaitFinishGun());
    }

    public void D1Attack2()
    {
        if (isAttack2)
        {
            isAttack2 = false;
            auto.bullets[gunnum].Shot();
            StartCoroutine(WaitFinishGun());
        }
    }
    #endregion;

}
