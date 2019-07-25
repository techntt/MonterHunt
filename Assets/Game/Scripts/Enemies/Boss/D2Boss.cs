using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class D2Boss : MonoBehaviour {

    #region Inspector Variables
    public AutoBoss auto;
    #endregion;

    #region Member Variables
    private int gunnum;
    private bool isAttack2;
    #endregion;

    #region Unity Methods
    private void Awake()
    {
        gunnum = 0;
        auto.OnBossAction += OnD2Action;
        auto.OnBossAction1 += OnD2Action1;
        auto.OnBossAttack += OnD2Attack;
        auto.OnBossAttack1 += OnD2Attack1;
        auto.OnBossAttack2 += OnD2Attack2;
        isAttack2 = false;
    }
    #endregion;

    #region Public Methods
    public void D2Action1()
    {
        auto.bullets[gunnum].Shot();
        StartCoroutine(WaitFinishGun());
    }

    public void D2Attack()
    {
        auto.bullets[gunnum].Shot();
        StartCoroutine(WaitFinishGun());
    }

    public void D2Attack1()
    {
        gunnum = 2;
        auto.bullets[gunnum].Shot();
        gunnum = 3;
        auto.bullets[gunnum].Shot();
        StartCoroutine(WaitFinishGun());
    }

    public void D2Attack2()
    {
        if (isAttack2)
        {
            isAttack2 = false;
            gunnum = 4;
            auto.bullets[gunnum].Shot();
            gunnum = 5;
            auto.bullets[gunnum].Shot();
            StartCoroutine(WaitFinishGun());
        }
    }
    #endregion;

    #region Private Methods    
    private void OnD2Action()
    {
        Vector3 nextPos = auto.posAppear;
        nextPos.x = Random.Range(-0.5f, 0.5f);
        nextPos.y = Random.Range(-0.5f, 0.5f);
        auto.trans.DOJump(nextPos, 0.5f, 3, 1.5f).OnComplete(() =>
        {
            auto.OnCallMakeDicision(Random.Range(auto.minActTimeDelay, auto.maxActTimeDelay));
        });

    }

    private void OnD2Action1()
    {
        gunnum = 0;
        auto.boss.myAnim.SetTrigger(AnimConst.attack);
    }

    private void OnD2Attack()
    {
        gunnum = 1;
        auto.boss.myAnim.SetTrigger(AnimConst.attack1);
    }

    private void OnD2Attack1()
    {       
        auto.boss.myAnim.SetTrigger(AnimConst.attack2);
    }

    private void OnD2Attack2()
    {
        isAttack2 = true;
        auto.boss.myAnim.SetTrigger("attack3");
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
}
