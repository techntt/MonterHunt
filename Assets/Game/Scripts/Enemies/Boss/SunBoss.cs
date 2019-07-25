using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SunBoss : MonoBehaviour {

    #region Inspector Variables
    public AutoBoss auto;
    public float moveSpeed = 2f;    
    #endregion;

    #region Member Variables
    private int gunnum;
    private bool isAttack;
    private bool isAttack2;
    private bool moveToRight;
    private Vector3 leftBound, rightBound;
    #endregion;

    #region Unity Methods
    private void Awake()
    {
        gunnum = 0;
        auto.OnBossAction += OnSunAction;
        auto.OnBossAction1 += OnSunAction1;
        auto.OnBossAttack += OnSunAttack;
        auto.OnBossAttack1 += OnSunAttack1;
        auto.OnBossAttack2 += OnSunAttack2;
        isAttack2 = false;
        moveToRight = true;
        leftBound = new Vector3(GameManager.Instance.wallLeft.position.x + 1, auto.posAppear.y, 0);
        rightBound = new Vector3(GameManager.Instance.wallRight.position.x - 1, auto.posAppear.y, 0);

    }

    private void Update()
    {
        if (auto.boss.myAnim.GetCurrentAnimatorStateInfo(0).IsName("ido"))
        {
            if(auto.currState == AutoBoss.AI_STATE.ACTION || auto.currState == AutoBoss.AI_STATE.ACTION1)
            {
                if (moveToRight)
                {
                    transform.position = Vector3.MoveTowards(transform.position, rightBound, moveSpeed * Time.deltaTime);
                    if (transform.position == rightBound)
                    {
                        moveToRight = false;
                    }
                }
                else
                {
                    transform.position = Vector3.MoveTowards(transform.position, leftBound, moveSpeed * Time.deltaTime);
                    if (transform.position == leftBound)
                    {
                        moveToRight = true;
                    }
                }
            }
        }
    }
    #endregion;

    #region Public Methods
    public void SunAttack()
    {
        if (!isAttack)
        {
            isAttack = true;
            auto.bullets[gunnum].Shot();
            StartCoroutine(WaitFinishGun());
        }        
    }

    public void SunAttack1()
    {
        auto.bullets[gunnum].Shot();
        StartCoroutine(WaitFinishGun());
    }

    public void SunAttack2()
    {        
        if (!isAttack2)
        {
            isAttack2 = true;
            auto.bullets[gunnum].Shot();
            StartCoroutine(WaitFinishGun());
        }        
    }
    #endregion;

    #region Private Methods

    private void OnSunAction()
    {
        auto.OnCallMakeDicision(Random.Range(auto.minActTimeDelay,auto.maxActTimeDelay));
    }



    private void OnSunAction1()
    {
        auto.OnCallMakeDicision(Random.Range(auto.minActTimeDelay, auto.maxActTimeDelay));
    }

    private void OnSunAttack()
    {
        gunnum = 0;
        isAttack = false;
        auto.boss.myAnim.SetTrigger(AnimConst.attack);
    }

    private void OnSunAttack1()
    {
        gunnum = 1;
        auto.boss.myAnim.SetTrigger(AnimConst.attack1);
    }

    private void OnSunAttack2()
    {
        gunnum = 2;
        isAttack2 = false;
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
            auto.OnCallMakeDicision(Random.Range(auto.minAtkTimeDelay, auto.minAtkTimeDelay));
        }
    }
    #endregion;
}
