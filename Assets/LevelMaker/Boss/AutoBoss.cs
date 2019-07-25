using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Linq;

public class AutoBoss : MonoBehaviour
{

    #region Inspector Variables
    public Vector2 posAppear;
    public BaseBoss boss;
    public BaseBullet[] bullets;
    public float minAtkTimeDelay = 1f, maxAtkTimeDelay=1.5f;
    public float minActTimeDelay = 0.5f, maxActTimeDelay = 1.5f;
    #endregion;

    #region Member Variables
    [HideInInspector] public Transform trans;
    public delegate void moveAppear(Vector2 pos);
    public event moveAppear OnMoveAppear;

    public enum AI_STATE
    {
        IDLE,
        ACTION,
        ACTION1,
        ATTACK,
        ATTACK1,
        ATTACK2,
        DIE
    }

    public AI_STATE preState;
    public AI_STATE currState;

    private float determineTime = 1f;
    private int countState = 0;
    private bool isLive;
    #endregion;


    #region Unity Methods
    private void Awake()
    {
        GameEventManager.Instance.BossAppear += HandleBossFinishAppear;
        GameEventManager.Instance.BossDefeated += HandleBossDefeated;
        trans = gameObject.GetComponent(typeof(Transform)) as Transform;
    }
    private void Start()
    {
        preState = AI_STATE.IDLE;
        currState = AI_STATE.IDLE;
        isLive = true;

        if (OnMoveAppear != null)
            OnMoveAppear(posAppear);
        else
            transform.DOLocalMove(posAppear, 2).OnComplete(() =>
            {
                boss.FinishAppear();
                OnChangeState();
            });
    }

    public delegate void NoParamEvent();
    public event NoParamEvent OnHandleUpdate;

    private void Update()
    {
        if (OnHandleUpdate != null)
            OnHandleUpdate();
        else
        {
            
        }
    }
    #endregion;

    #region Public Methods
    public delegate AI_STATE makeDicision(AI_STATE currentState);
    public event makeDicision OnMakeDision;        
    public event NoParamEvent ChangeState;

    public IEnumerator MakeDicision()
    {
        yield return new WaitForSeconds(determineTime);
        if (OnMakeDision != null)
            currState = OnMakeDision(currState);
        else
        {
            
            // default percent for each attack is 1/3
            int action = Random.Range(0, 5);
            switch (action)
            {
                case 0:
                    currState = AI_STATE.ACTION;
                    break;
                case 1:
                    currState = AI_STATE.ATTACK;
                    break;
                case 2:
                    currState = AI_STATE.ATTACK1;
                    break;
                case 3:
                    currState = AI_STATE.ACTION1;
                    break;
                case 4:
                    currState = AI_STATE.ATTACK2;
                    break;
            }   
        }
        if (currState == preState)
            countState++;
        else
            countState = 0;
        if (countState == 2) // avoid loop state
        {
            //Debug.Log("State: "+currState+ "[+"+countState+"]");
            var data = System.Enum
                    .GetValues(typeof(AI_STATE))
                    .Cast<AI_STATE>()
                    .Where(item => item != currState)
                    .Where(item => item != AI_STATE.IDLE)
                    .Where(item => item != AI_STATE.DIE)
                    .ToArray();
            currState = (AI_STATE)data.GetValue(Random.Range(0,data.Length-1));
            //Debug.Log("State Change: " + currState);
            countState = 0;
        }
        preState = currState;        
        OnChangeState();
    }

    public void OnChangeState()
    {
        if (ChangeState != null)
            ChangeState();
        else
            switch (currState)
            {
                case AI_STATE.IDLE:
                    preState = currState;
                    OnCallMakeDicision(Random.Range(minActTimeDelay, maxActTimeDelay));                    
                    break;
                case AI_STATE.ACTION:                    
                    BossAction();
                    break;
                case AI_STATE.ACTION1:
                    BossAction1();
                    break;
                case AI_STATE.ATTACK:
                    BossAttack();
                    break;
                case AI_STATE.ATTACK1:
                    BossAttack1();
                    break;
                case AI_STATE.ATTACK2:
                    BossAttack2();
                    break;
            }
    }

    public void Attack()
    {
        if (bullets[0] != null)
        {
            bullets[0].Shot();
        }
        currState = AI_STATE.IDLE;
        OnChangeState();
    }

    public void Attack1()
    {
        if (bullets[1] != null)
        {
            bullets[1].Shot();
        }
        currState = AI_STATE.IDLE;
        OnChangeState();
    }

    public void Attack2()
    {
        if (bullets[2] != null)
        {
            bullets[2].Shot();
        }
        currState = AI_STATE.IDLE;
        OnChangeState();
    }

    public void OnCallMakeDicision(float determineTime)
    {
        if (isLive)
        {
            this.determineTime = determineTime;
            StartCoroutine(MakeDicision());
        }        
    }

    public void Die()
    {
        if (!isLive)
        {
            StartCoroutine(Dead());
        }
    }
    #endregion;



    #region Private Methods

    public event NoParamEvent OnBossAction;
    private void BossAction() // Default boss 
    {
        if (OnBossAction != null)
            OnBossAction();
        else
        {
            Vector3 nextPos = GameManager.Instance.player1.transform.position;
            if (nextPos.x > 1.6f)
                nextPos.x = 1.6f;
            else if (nextPos.x < -1.6f)
                nextPos.x = -1.6f;

            if (nextPos.y > 3f)
                nextPos.y = 3f;
            else if (nextPos.y < -3f)
                nextPos.y = -3f;

            boss.myAnim.SetTrigger("shellAttack");
            transform.DOLocalMove(nextPos,1).OnComplete(()=>
            {
                OnCallMakeDicision(Random.Range(minActTimeDelay, maxActTimeDelay));
            });
        }
    }

    public event NoParamEvent OnBossAction1;
    private void BossAction1()
    {
        if (OnBossAction1 != null)
            OnBossAction1();
        else
        {
            Vector3 nextPos = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
            Mathf.Clamp(nextPos.x, -1.6f, 1.6f);
            Mathf.Clamp(nextPos.y, -3, 3);
            transform.DOLocalJump(nextPos, 0.5f, 3, 2).OnComplete(() =>
            {   
                OnCallMakeDicision(Random.Range(minActTimeDelay, maxActTimeDelay));
            });
        }
    }

    public event NoParamEvent OnBossAttack;
    private void BossAttack()
    {
        if (OnBossAttack != null)
            OnBossAttack();
        else
        {
            boss.myAnim.SetTrigger(AnimConst.attack);
        }

    }
      

    public event NoParamEvent OnBossAttack1;
    private void BossAttack1()
    {
        if (OnBossAttack1 != null)
            OnBossAttack1();
        else
        {
            boss.myAnim.SetTrigger(AnimConst.attack1);
        }
    }
    public event NoParamEvent OnBossAttack2;
    private void BossAttack2()
    {
        if (OnBossAttack2 != null)
            OnBossAttack2();
        else
        {
            if((Vector2)transform.position != posAppear)
            {
                transform.DOLocalJump(posAppear, 0.5f, 3, 2).OnComplete(() =>
                {
                    boss.myAnim.SetTrigger(AnimConst.attack2);
                });
            } else
                boss.myAnim.SetTrigger(AnimConst.attack2);
        }
    }  


    private IEnumerator Dead()
    {
        StopAllGun();
        for (int i = 0; i < boss.invulnerableParts.Length; i++)
        {
            boss.invulnerableParts[i].gameObject.SetActive(false);
            ExplodeEffect e = (ExplodeEffect)EffectManager.Instance.SpawnEffect(EFFECT_TYPE.EXPLODE, boss.invulnerableParts[i].transform.position);
            e.Init(5, 0);
            CircleManager.Instance.SpawnExplodeEffect(boss.invulnerableParts[i].transform.position, boss.invulnerableParts[i].GetComponent<SpriteRenderer>().color);
            yield return new WaitForSeconds(0.3f);
        }

        this.gameObject.SetActive(false);
        GameEventManager.Instance.OnBossFinishDie();
    }

    public event NoParamEvent OnBossFinishAppear;
    private void HandleBossFinishAppear()
    {
        if (OnBossFinishAppear != null)
            OnBossFinishAppear();
        else
        {

        }
    }
    public event NoParamEvent OnBossDefeated;
    private void HandleBossDefeated()
    {
        isLive = false;
        if (OnBossDefeated != null)
            OnBossDefeated();
        else
        {
            currState = AI_STATE.DIE;
            boss.myAnim.SetTrigger(AnimConst.die);           
        }
    }

    private void StopAllGun()
    {
        foreach(BaseBullet bullet in bullets)
        {
            if (bullet._Shooting)
            {
                bullet._Shooting = false;
            }
        }
    }
    #endregion;

}
