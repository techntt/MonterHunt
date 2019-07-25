using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TreeBoss : MonoBehaviour {
    #region Inspector Variables
    public AutoBoss auto;
    public Transform right_fire, left_fire;
    public GameObject dead_zone;
    public ParticleSystem sign_warning,wind_attack;
    #endregion;

    #region Member Variables
    private bool isAttack;
    private int gunnum;
    private bool isWindAttack;
    #endregion;

    #region Unity Methods
    private void Awake()
    {
        isAttack = false;
        gunnum = 0;
        auto.OnBossAttack += OnTreeAttack;
        auto.OnBossAttack1 += OnTreeAttack1;
        auto.OnBossAttack2 += OnTreeAttack2;
        isWindAttack = false;
    }
    #endregion;

    #region Public Methods
    public void TreeAttackRight()
    {
        for (int i = 0; i < 3; i++)
        {
            CircleType t = CircleSpawner.Instance.GetRandomCircleType();
            Circle c = CircleManager.Instance.PopCircle(t, 0.8f, right_fire.position);
            int hp = CircleSpawner.Instance.GetRandomHP();
            c.Init(hp, CircleOrbit.ZR, 4, false, false, true);
            c.myBody.velocity = Quaternion.Euler(0, 0, -75 + i*5) * -right_fire.up * 4;
        }
    }

    public void PlaySfx(AudioClip sfx)
    {
        SoundManager.Instance.PlaySfx(sfx);
    }
    public void TreeAttackLeft()
    {
        for (int i = 0; i < 3; i++)
        {
            CircleType t = CircleSpawner.Instance.GetRandomCircleType();
            Circle c = CircleManager.Instance.PopCircle(t, 0.8f, left_fire.position);
            int hp = CircleSpawner.Instance.GetRandomHP();
            c.Init(hp, CircleOrbit.ZL, 4, false, false, true);
            c.myBody.velocity = Quaternion.Euler(0, 0, 75 - i*5) * -left_fire.up * 4;
        }

        auto.OnCallMakeDicision(Random.Range(auto.minAtkTimeDelay, auto.maxAtkTimeDelay));
    }

    public void TreeAttackCenter()
    {
        sign_warning.Play();
        wind_attack.Play();
        isWindAttack = true;
        StartCoroutine(WindAttack());
    }
      

    private IEnumerator WindAttack()
    {
        yield return new WaitForSeconds(1f);
        dead_zone.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        wind_attack.Stop();
        yield return new WaitForSeconds(1f);
        dead_zone.SetActive(false);
        sign_warning.Stop();
        auto.boss.myAnim.SetTrigger(AnimConst.idle);
        isWindAttack = false;
        auto.OnCallMakeDicision(Random.Range(auto.minAtkTimeDelay, auto.maxAtkTimeDelay));
    }


    public void TreeAttack2()
    {
        if (isAttack)
        {
            isAttack = false;
            auto.bullets[gunnum].Shot();
            StartCoroutine(WaitFinishShoot());
        }
    }

    public void Die()
    {
        if (isWindAttack)
        {
            StopCoroutine("WindAttack");
            wind_attack.Stop();
            dead_zone.SetActive(false);
            sign_warning.Stop();
        }
        
        auto.Die();
    }
    #endregion;

    #region Private Methods
    private void OnTreeAction()
    {

    }

    private void OnTreeAction1()
    {

    }

    private void OnTreeAttack()
    {
        if (auto.trans.position.y < 0)
        {
            Vector3 next = auto.trans.position;
            next.y = Random.Range(0, 1);
            auto.trans.DOJump(next, 0.5f,2, 1).OnComplete(() =>
            {
                auto.boss.myAnim.SetTrigger(AnimConst.attack);
            });
        }
        else
        {
            auto.boss.myAnim.SetTrigger(AnimConst.attack);
        }
        
    }

    private void OnTreeAttack1()
    {
        Vector3 next = GameManager.Instance.player1.transform.position;
        next.y = Random.Range(0, 1);
        next.x = Mathf.Clamp(next.x, -2f, 2f);
        auto.trans.DOJump(next, 0.5f, 2, 1).OnComplete(() =>
        {
            auto.boss.myAnim.SetTrigger(AnimConst.attack1);
        });        
    }

    private void OnTreeAttack2()
    {
        isAttack = true;
        gunnum = 0;
        auto.boss.myAnim.SetTrigger(AnimConst.attack2);

    }

  

    private IEnumerator WaitFinishShoot()
    {
        yield return new WaitForSeconds(0.5f);
        if (auto.bullets[gunnum]._Shooting)
        {
            StartCoroutine(WaitFinishShoot());
        }
        else
        {
            auto.boss.myAnim.SetTrigger(AnimConst.idle);
            auto.OnCallMakeDicision(Random.Range(auto.minAtkTimeDelay, auto.maxAtkTimeDelay));
        }
    }
    #endregion;
}
