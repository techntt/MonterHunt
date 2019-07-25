using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;


public class D4Boss : MonoBehaviour {

    #region Inspector Variables
    public AutoBoss auto;
    #endregion;

    #region Member Variables
    private int gunnum;
    private int type;
    private int turn;
    #endregion;

    #region Unity Methods
    private void Awake()
    {
        gunnum = 0;
        type = 0;
        turn = 0;
        auto.OnBossAction += OnD4Action;
        auto.OnBossAction1 += OnD4Action1;
        auto.OnBossAttack += OnD4Attack;
        auto.OnBossAttack1 += OnD4Attack1;
        auto.OnBossAttack2 += OnD4Attack2;
    }
    #endregion;

    #region Public Methods
    public void D4Attack()
    {
        switch (type)
        {
            case 0:
                auto.bullets[gunnum = 0].Shot();
                StartCoroutine(WaitFinishShoot());
                break;
            case 1:                
                auto.bullets[gunnum = 1].Shot();
                auto.bullets[gunnum = 2].Shot();
                auto.bullets[gunnum = 3].Shot();
                StartCoroutine(WaitFinishShoot());
                break;
            case 2:
                if(turn == 0)
                {
                    auto.bullets[gunnum = 4].Shot();
                    turn = 1;
                    Invoke("MoveNext", 1.5f);

                }else if( turn == 1)
                {
                    auto.bullets[gunnum = 4].Shot();
                    turn = 2;
                    Invoke("MoveNext", 1.5f);
                }
                else
                {
                    auto.bullets[gunnum = 4].Shot();
                    turn = 0;
                    StartCoroutine(WaitFinishShoot());
                }
                break;
        }
    }
    #endregion;

    #region Private Methods
    private void OnD4Action()
    {
        Vector3 next = auto.trans.position;
        next.x = Random.Range(-1.8f, 1.8f);
        auto.trans.DOMove(next, 1f).OnComplete(() =>
        {
            auto.OnCallMakeDicision(Random.Range(auto.minActTimeDelay, auto.maxActTimeDelay));
        });
    }

    private void OnD4Action1()
    {
        Vector3 next = auto.trans.position;
        next.x = Random.Range(-1.8f, 1.8f);
        next.y = Random.Range(1.5f, 2.6f);
        auto.trans.DOJump(next,0.5f,2, 1f).OnComplete(() =>
        {
            auto.OnCallMakeDicision(Random.Range(auto.minActTimeDelay, auto.maxActTimeDelay));
        });
    }

    private void OnD4Attack()
    {
        type = 0;
        auto.boss.myAnim.SetTrigger(AnimConst.attack);
    }

    private void OnD4Attack1()
    {
        type = 1;
        Vector3 next = auto.posAppear;
        auto.trans.DOMove(next, 0.5f).OnComplete(() =>
        {
            auto.boss.myAnim.SetTrigger(AnimConst.attack);
        });
    }

    private void OnD4Attack2()
    {
        type = 2;
        turn = 0;        
        MoveNext();
    }

    private void MoveNext()
    {
        Vector3 next = auto.trans.position;
        next.y = 2.5f;
        if(turn == 0)
        {
            next.x = -1.5f;
        }else if(turn == 1)
        {
            next.x = 0;
        }
        else
        {
            next.x = 1.5f;
        }
        auto.trans.DOMove(next, 0.5f).OnComplete(() =>
        {
            auto.boss.myAnim.SetTrigger(AnimConst.attack);
        });

    }

    private IEnumerator WaitFinishShoot()
    {
        yield return new WaitForSeconds(0.3f);
        if (auto.bullets[gunnum]._Shooting)
            StartCoroutine(WaitFinishShoot());
        else
        {
            auto.OnCallMakeDicision(Random.Range(auto.minAtkTimeDelay, auto.maxAtkTimeDelay));
        }
    }
    #endregion;
}
