using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class D5Boss : MonoBehaviour {

    #region Inspector Variables
    public AutoBoss auto;
    public float moveSpeed = 2f;
    #endregion;

    #region Member Variables
    private int gunnum;
    private bool moveToRight;
    private Vector3 leftBound, rightBound;
    #endregion;

    #region Unity Methods
    private void Awake()
    {
        gunnum = 0;
        moveToRight = true;
        leftBound = new Vector3(GameManager.Instance.wallLeft.position.x + 1, auto.posAppear.y, 0);
        rightBound = new Vector3(GameManager.Instance.wallRight.position.x - 1, auto.posAppear.y, 0);

        auto.OnBossAction += OnD5Action;
        auto.OnBossAction1 += OnD5Action1;
        auto.OnBossAttack += OnD5Attack;
        auto.OnBossAttack1 += OnD5Attack1;
        auto.OnBossAttack2 += OnD5Attack2;
    }

    private void Update()
    {
        if (auto.boss.myAnim.GetCurrentAnimatorStateInfo(0).IsName("ido"))
        {
            if (auto.currState == AutoBoss.AI_STATE.ACTION || auto.currState == AutoBoss.AI_STATE.ACTION1)
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
    public void D5Attack()
    {
        auto.bullets[gunnum].Shot();
        StartCoroutine(WaitFinishShoot());
    }
    #endregion;

    #region Private Methods
    private void OnD5Action()
    {
        auto.OnCallMakeDicision(Random.Range(auto.minActTimeDelay, auto.maxActTimeDelay));
    }

    private void OnD5Action1()
    {
        auto.OnCallMakeDicision(Random.Range(auto.minActTimeDelay, auto.maxActTimeDelay));
    }

    private void OnD5Attack()
    {
        gunnum = 0;
        auto.boss.myAnim.SetTrigger(AnimConst.attack);
    }

    private void OnD5Attack1()
    {
        gunnum = 1;
        auto.boss.myAnim.SetTrigger(AnimConst.attack);
    }

    private void OnD5Attack2() {
        gunnum = 2;
        auto.boss.myAnim.SetTrigger(AnimConst.attack);
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
