using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FrogBoss : MonoBehaviour {

    #region Inspector Variables
    public AutoBoss auto;
    public Transform[] arm;
    public GameObject[] fists;
    #endregion;

    #region Member Variables
    private int gunnum;
    private Transform[] fistTrans;
    private Animator[] fistAnim;
    #endregion;

    #region Unity Methods
    private void Awake()
    {
        auto.OnBossAction += OnFrogAction;
        auto.OnBossAction1 += OnFrogAction1;
        auto.OnBossAttack += OnFrogAttack;
        auto.OnBossAttack1 += OnFrogAttack1;
        auto.OnBossAttack2 += OnFrogAttack2;
        gunnum = 0;
        int i = 0;
        fistTrans = new Transform[fists.Length];
        fistAnim = new Animator[fists.Length];
        foreach(GameObject obj in fists)
        {
            fistTrans[i] = obj.GetComponent(typeof(Transform)) as Transform;
            fistAnim[i++] = obj.GetComponent(typeof(Animator)) as Animator;
        }
    }
    #endregion;

    #region Public Methods
    public void FrogAttack()
    {
        auto.bullets[gunnum].Shot();
        auto.OnCallMakeDicision(Random.Range(auto.minAtkTimeDelay,auto.maxAtkTimeDelay));
    }

    public void FrogAttack1()
    {
        auto.bullets[gunnum].Shot();
        auto.OnCallMakeDicision(Random.Range(auto.minAtkTimeDelay, auto.maxAtkTimeDelay));
    }
    
    #endregion;

    #region Private Methods
    private void OnFrogAction()
    {
        // Move to middle
        auto.trans.DOLocalMove(auto.posAppear, 1).OnComplete(() =>
        {
            auto.boss.myAnim.SetTrigger(AnimConst.attack2);
            StartCoroutine(Puch(0));
        });
    }

    private IEnumerator Puch(int i)
    {
        if (i < fists.Length)
        {            
            Vector3 startPoint = arm[i].position;           
            
            Vector3 angle = arm[i].localEulerAngles;
            fistTrans[i].position = startPoint;
            fists[i].SetActive(true);
            arm[i].gameObject.SetActive(false);
            fistAnim[i].SetTrigger("dobig");
            Vector3 target = GameManager.Instance.player1.transform.position;
            yield return new WaitForSeconds(0.5f);            
            fistTrans[i].DOMove(target, 0.3f).OnComplete(() =>
            {
                StartCoroutine(PuchCallback(0.2f, startPoint, i));
            });
        }
        else
        {
            auto.boss.myAnim.SetTrigger(AnimConst.idle);
            auto.OnCallMakeDicision(Random.Range(auto.minAtkTimeDelay, auto.maxAtkTimeDelay));
        }

        yield return null;
    }

    private IEnumerator PuchStop(float stoptime,int i)
    {
        yield return new WaitForSeconds(stoptime);
        fists[i].SetActive(false);
        arm[i].gameObject.SetActive(true);
        StartCoroutine(Puch(++i));
    }

    private IEnumerator PuchCallback(float stoptime,Vector3 endPoint,int i)
    {
        yield return new WaitForSeconds(stoptime);
        fistTrans[i].DOMove(endPoint, 0.2f).OnComplete(() =>
        {
            fistTrans[i].position = arm[i].position;
            fistAnim[i].SetTrigger("dosmall");
            StartCoroutine(PuchStop(0.2f, i));
        });
    }

    private void OnFrogAction1()
    {
        Vector3 nextPos = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        Mathf.Clamp(nextPos.x, -1.65f, 1.65f);
        Mathf.Clamp(nextPos.y, -1.5f, 0.76f);
        transform.DOLocalJump(nextPos, 0.5f, 3, 2).OnComplete(() =>
        {
            auto.OnCallMakeDicision(Random.Range(auto.minActTimeDelay,auto.maxActTimeDelay));
        });
    }

    private void OnFrogAttack()
    {
        gunnum = 0;
        auto.boss.myAnim.SetTrigger(AnimConst.attack);
    }

    private void OnFrogAttack1()
    {
        gunnum = 1;
        auto.boss.myAnim.SetTrigger(AnimConst.attack);
    }

    private void OnFrogAttack2()
    {
        gunnum = 2;
        auto.boss.myAnim.SetTrigger(AnimConst.attack1);
    }
    #endregion;

}
