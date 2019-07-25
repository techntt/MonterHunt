using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;

public class D3Boss : MonoBehaviour {

    #region Inspector Variables
    public AutoBoss auto;
    public GameObject warningSign;
    public Transform weaponPos;
    public D3SwordController sword;
    public AudioClip warning;
    #endregion;

    #region Member Variables
    private int gunnum;
    private int shootCount;
    private int numShoot = 3;
    private bool isAttack2;
    private int direction;
    #endregion

    #region Unity Methods
    private void Awake()
    {
        auto.OnBossAction += OnD3Action;
        auto.OnBossAction1 += OnD3Action1;
        auto.OnBossAttack += OnD3Attack;
        auto.OnBossAttack1 += OnD3Attack1;
        auto.OnBossAttack2 += OnD3Attack2;
        // detach warning sign
        warningSign.transform.parent = null;
        Vector3 centerBottom = new Vector3(0, 0,0);
        Vector3 signPos = Camera.main.ScreenToWorldPoint(centerBottom);
        signPos.x = 0;
        signPos.y += 0.4f;
        signPos.z = 0;
        warningSign.transform.position = signPos;
        gunnum = 0;
        shootCount = 0;
        isAttack2 = false;
        direction = 1;
        // detach sword
        sword.transform.parent = null;
    }
    #endregion;

    #region Public Methods
    public void D3Attack()
    {
        sword.transform.position = weaponPos.position;       
        sword.myAnim.SetTrigger("appear");
        auto.OnCallMakeDicision(Random.Range(auto.minAtkTimeDelay, auto.maxAtkTimeDelay));
    }

    public void D3Attack1()
    {
        auto.bullets[gunnum = 2].Shot();
        auto.OnCallMakeDicision(Random.Range(auto.minAtkTimeDelay, auto.maxAtkTimeDelay));
    }

    public void D3Attack2()
    {
        if (isAttack2)
        {
            isAttack2 = false;
            AttackWarningSign();
        }
    }

    #endregion;

    #region Private Methods
    private void OnD3Action()
    {
        Vector3 next = auto.posAppear;
        next.x = Random.Range(-0.9f, 0.9f);
        next.y = Random.Range(1.5f, 2.5f);
        auto.trans.DOJump(next, 0.5f, 2, 1).OnComplete(() =>
        {
            auto.OnCallMakeDicision(Random.Range(auto.minActTimeDelay,auto.maxActTimeDelay));
        });
    }

    private void OnD3Action1()
    {
        Vector3 next = auto.trans.position;
        next.x = Random.Range(-0.9f, 0.9f);
        auto.trans.DOMove(next,0.5f).OnComplete(() =>
        {
            auto.OnCallMakeDicision(Random.Range(auto.minActTimeDelay, auto.maxActTimeDelay));
        });
    }

    private void OnD3Attack()
    {
        Vector3 next = GameManager.Instance.player1.transform.position ;
        next.y = Random.Range(2.7f, 3f);
        auto.trans.DOMove(next, 0.5f).OnComplete(() =>
        {
            auto.boss.myAnim.SetTrigger(AnimConst.attack);
        });
        
    }

    private void OnD3Attack1()
    {
        Vector3 next = auto.trans.position;
        next.x = Random.Range(-0.2f, 0.2f);
        next.y = Random.Range(2.7f, 3f);
        auto.trans.DOMove(next, 0.5f).OnComplete(() =>
        {
            auto.boss.myAnim.SetTrigger(AnimConst.attack1);
        });        
    }

    private void OnD3Attack2()
    {
        Vector3 next = auto.posAppear;
        next.x = 0;
        auto.trans.DOMove(next, 1).OnComplete(() =>
        {
            isAttack2 = true;
            auto.boss.myAnim.SetTrigger(AnimConst.attack2);
        });        
    }



    private void AttackWarningSign()
    {
        Vector3 nextPos = warningSign.transform.position;
        if(shootCount == 0)
        {
            int percent = Random.Range(0, 10);
            if (percent > 5)
                direction = 1;
            else
                direction = -1;
        }
        SoundManager.Instance.PlaySfx(warning);
        nextPos.x = (-1.5f + shootCount * 1.5f) * direction;
        warningSign.transform.position = nextPos;
        StartCoroutine(WarningBlink(4, 0.2f,true));        
    }

    private IEnumerator WarningBlink(int times, float delay,bool state)
    {
        warningSign.SetActive(state);
        yield return new WaitForSeconds(delay);
        times--;
        if (times > 0)
        {
            StartCoroutine(WarningBlink(times, delay,!state));
        }
        else
        {
            warningSign.SetActive(false);
            ShootWarningSign();
        }
    }

    private void ShootWarningSign()
    {
        shootCount++;
        auto.bullets[gunnum=0].Shot();
        auto.bullets[gunnum=1].Shot();
        StartCoroutine(WaitFinishAttack2());
    }

    private IEnumerator WaitFinishAttack2()
    {
        yield return new WaitForSeconds(0.2f);
        if (auto.bullets[gunnum]._Shooting)
            StartCoroutine(WaitFinishAttack2());
        else
        {
            if (shootCount < numShoot)
            {
                Invoke("AttackWarningSign", 0.5f);
            }
            else
            {
                shootCount = 0;
                auto.boss.myAnim.SetTrigger(AnimConst.idle);
                auto.OnCallMakeDicision(Random.Range(auto.minAtkTimeDelay,auto.maxAtkTimeDelay));
            }
        }
    }
    #endregion;

}
