using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class IceBoss : MonoBehaviour {

    #region Inspector Variables
    public AutoBoss auto;
    public UbhBaseShot iceBullet;
    public Transform right_fire;
    #endregion;

    #region Member Variables
    private int gunnum ;
    #endregion;

    #region Unity Methods
    private void Awake()
    {
        gunnum = 0;
        auto.OnBossAction += OnIceAction;
        auto.OnBossAction1 += OnIceAction1;
        auto.OnBossAttack += OnIceAttack;
        auto.OnBossAttack1 += OnIceAttack1;
        auto.OnBossAttack2 += OnIceAttack2;
        // detach gun bullet
        Transform iTrans = iceBullet.gameObject.GetComponent(typeof(Transform)) as Transform;
        iTrans.parent = null;
        iTrans.position = new Vector3(0, 8, 0);
    }
    #endregion;

    #region Public Methods
    public void IceAttack()
    {
        auto.bullets[gunnum = 0].Shot();
        StartCoroutine(WaitFinishShoot());
    }

    public void IceAttack1()
    {

        for (int i = 0; i < 3; i++)
        {
            CircleType t = CircleSpawner.Instance.GetRandomCircleType();
            Circle c = CircleManager.Instance.PopCircle(t, 0.8f, right_fire.position);
            int hp = CircleSpawner.Instance.GetRandomHP();
            c.Init(hp, CircleOrbit.ZR, 4, false, false, true);
            c.myBody.velocity = Quaternion.Euler(0, 0, -75 + i * 5) * -right_fire.up * 4;
        }
        auto.OnCallMakeDicision(Random.Range(0.8f, 1.5f));
    }

    public void IceAttack2()
    {        
        auto.OnCallMakeDicision(Random.Range(2f, 2.5f));
    }
    #endregion;

    #region Private Methods

    private void OnIceAction()
    {
        Vector3 next = auto.trans.position;
        next.x = Random.Range(-0.7f, 0.7f);
        next.y = Random.Range(2.7f, 3.3f);
        auto.trans.DOMove(next,1).OnComplete(() =>
        {
            auto.OnCallMakeDicision(Random.Range(0.8f, 1f));
        });
        
    }

    private void OnIceAction1()
    {
        Vector3 next = auto.trans.position;
        next.x = Random.Range(-0.7f, 0.7f);
        next.y = Random.Range(2.7f, 3.3f);
        auto.trans.DOJump(next,0.5f,2, 1).OnComplete(() =>
        {
            auto.OnCallMakeDicision(Random.Range(0.8f, 1f));
        });
    }
       

    private void OnIceAttack()
    {
        auto.boss.myAnim.SetTrigger(AnimConst.attack);
    }

    private void OnIceAttack1()
    {
        Vector3 next = auto.trans.position;
        next.x = -0.7f;
        auto.trans.DOMove(next,1).OnComplete(() =>
        {
            auto.boss.myAnim.SetTrigger(AnimConst.attack1);
        });        
    }

    private void OnIceAttack2()
    {
        StartCoroutine(IceShoot());
    }

    private IEnumerator WaitFinishShoot()
    {
        yield return new WaitForSeconds(0.3f);
        if (auto.bullets[gunnum]._Shooting)
            StartCoroutine(WaitFinishShoot());
        else
        {
            auto.boss.myAnim.SetTrigger(AnimConst.idle);
            auto.OnCallMakeDicision(Random.Range(0.8f, 1.5f));
        }
        
    }

    private IEnumerator IceShoot()
    {
        if (iceBullet._Shooting)
        {
            yield return new WaitForSeconds(0.3f);
            StartCoroutine(IceShoot());
        }
        else
        {
            auto.boss.myAnim.SetTrigger(AnimConst.attack2);
            iceBullet.Shot();
        }
    }
    #endregion;
}
