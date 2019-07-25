using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Frog2Boss : MonoBehaviour {

    #region Inspector Variables
    public AutoBoss auto;
    public MiniFrog[] frogs;
    public Transform moutPosition;
    #endregion;

    #region Member Variables
    private bool isEggSpawned;
    private int gunnum;
    private bool isAtack;
    #endregion;

    #region Unity Methods
    private void Awake()
    {
        isEggSpawned = false;
        gunnum = 0;
        auto.OnBossAction += OnFrog2Action;
        auto.OnBossAttack += OnFrog2Attack;
        auto.OnBossAttack1 += OnFrog2Attack1;
        auto.OnBossAttack2 += OnFrog2Attack2;
        isAtack = false;
    }
    #endregion;

    #region Public Methods
    public void Frog2SpawnEgg()
    {
        OnActiveMiniFrog(0);
    }

    public void Frog2Attack()
    {
        if (isAtack)
        {
            isAtack = false;
            auto.bullets[gunnum].Shot();
            StartCoroutine(WaitFinishShoot());
        }
    }

    public void Frog2Die()
    {
        foreach(MiniFrog frog in frogs)
        {
            frog.OnKillFrog();
        }
        auto.Die();
    }
    #endregion;

    #region Private Methods

    private void OnFrog2Action()
    {
        if (!isEggSpawned)
        {
            auto.boss.myAnim.SetTrigger("spawn");
        }
        else
        {
            Vector3 next = auto.trans.position;
            next.x = Random.Range(-1.8f, 1.8f);
            next.y = Random.Range(-1f, 0.5f);
            auto.trans.DOJump(next, 0.5f, 3, 1.5f).OnComplete(() =>
            {
                auto.OnCallMakeDicision(0.5f);
            });
        }
    }

    private void OnFrog2Action1()
    {
        Vector3 next = auto.trans.position;
        next.x = Random.Range(-1.8f, 1.8f);
        auto.trans.DOJump(next, 0.5f, 2, 1f).OnComplete(() =>
        {
            auto.OnCallMakeDicision(0.5f);
        });
    }

    private void OnFrog2Attack()
    {
        isAtack = true;
        gunnum = 0;
        auto.boss.myAnim.SetTrigger(AnimConst.attack);
    }

    private void OnFrog2Attack1()
    {
        isAtack = true;
        gunnum = 1;
        auto.boss.myAnim.SetTrigger(AnimConst.attack);
    }

    private void OnFrog2Attack2()
    {
        Vector3 next = auto.trans.position;
        next.x = Random.Range(-1.8f, 1.8f);
        auto.trans.DOJump(next, 0.5f, 2, 1f).OnComplete(() =>
        {
            auto.OnCallMakeDicision(0.5f);
        });
    }

    private void OnActiveMiniFrog(int i)
    {
        if (i >= frogs.Length)
        {
            auto.boss.myAnim.SetTrigger(AnimConst.idle);
            isEggSpawned = true;
            auto.OnCallMakeDicision(Random.Range(0.5f, 1.5f));
            return;
        }
        
        MiniFrog frog = frogs[i]; 
        Transform trans = frog.gameObject.GetComponent(typeof(Transform)) as Transform;
        trans.parent = null;
        trans.position = moutPosition.position;
        Vector3 endPoint = Vector3.zero;
        endPoint.x = Random.Range(-1.5f + i * 1.5f, -1.2f+i*1.5f);
        endPoint.y = Random.Range(-1.2f, -1f);
        trans.gameObject.SetActive(true);
        trans.DOMove(endPoint, 1f).OnComplete(() =>
        {
            frog.Init(i, 0 - i * 1f);
            frog.myAnim.SetTrigger("appear");
            i++;
            OnActiveMiniFrog(i);
        });
    }

    private IEnumerator WaitFinishShoot()
    {
        yield return new WaitForSeconds(0.3f);
        if (auto.bullets[gunnum]._Shooting)
        {
            StartCoroutine(WaitFinishShoot());
        }
        else
        {
            auto.boss.myAnim.SetTrigger(AnimConst.idle);
            auto.OnCallMakeDicision(Random.Range(1f, 1.5f));
        }
    }
    #endregion;
}
