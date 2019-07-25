using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dragon2Boss : MonoBehaviour {
    #region Inspector Variables
    public AutoBoss auto;
    #endregion;

    #region Member Varibales
    private int gunnum;
    private bool isAttack;
    #endregion;

    #region Unity Methods
    private void Awake()
    {
        gunnum = 0;
        isAttack = false;
        //auto.OnBossAction += OnDragon2Action;
        //auto.OnBossAction1 += OnDragon2Action1;
        auto.OnBossAttack += OnDragon2Attack;
        auto.OnBossAttack1 += OnDragon2Attack1;
        auto.OnBossAttack2 += OnDragon2Attack2;
    }
    #endregion;

    #region Public Methods
    public void Dragon2Attack()
    {
        if(gunnum == 2)
        {
            if (isAttack)
            {
                isAttack = false;
                auto.bullets[gunnum].Shot();
                StartCoroutine(WaitFinishGun());
            }
        }
        else
        {
            auto.bullets[gunnum].Shot();
            auto.OnCallMakeDicision(Random.Range(auto.minAtkTimeDelay,auto.maxAtkTimeDelay));
        }                
    }

    #endregion;

    #region Private Methods
    private void OnDragon2Action()
    {

    }
    private void OnDragon2Action1()
    {

    }

    private void OnDragon2Attack()
    {
        gunnum = 0;
        auto.boss.myAnim.SetTrigger(AnimConst.attack);
    }

    private void OnDragon2Attack1()
    {
        gunnum = 1;
        auto.boss.myAnim.SetTrigger(AnimConst.attack1);
    }

    private void OnDragon2Attack2()
    {
        gunnum = 2;
        isAttack = true;
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
            auto.OnCallMakeDicision(Random.Range(auto.minAtkTimeDelay, auto.maxAtkTimeDelay));
        }
    }
    #endregion;

}
