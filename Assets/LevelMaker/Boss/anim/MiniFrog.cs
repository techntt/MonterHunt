using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class MiniFrog : Damageable {

    #region Inspector Variables
    public Animator myAnim;
    public Collider2D myColl;
    public Image healthbar;
    public Transform firePos;
    #endregion;

    #region Member variables
    private bool isActive;
    private Transform trans;
    private bool leftFirst;
    private float startYPos;
    private float minX, maxX;
    private enum FrogState
    {
        IDLE,
        MOVE,
        ATTACK,
        NONE
    }

    private FrogState state;
    #endregion;

    #region Unity Methods
    private void Awake()
    {
        trans = gameObject.GetComponent(typeof(Transform)) as Transform;
        minX = GameManager.Instance.wallLeft.position.x + 0.5f;
        maxX = GameManager.Instance.wallRight.position.x - 0.5f;
    }
    #endregion;

    #region Public Methods    
    public void EnableHealthBar()
    {
        isDead = false;
        hp = maxHp;
        healthbar.enabled = true;
        healthbar.fillAmount = 0;
        ColliderRef.Instance.DamageableRef.Add(myColl.GetInstanceID(), this);
        StartCoroutine(IncreaseHP());
        
    }

    public void Init(int index, float startYPos)
    {
        if (index % 2 == 0)
        {
            leftFirst = true;
        }
        else
        {
            leftFirst = false;
        }
        this.startYPos = startYPos;
    }
    public void OnKillFrog()
    {
        if(!isDead)
         EggDie();
    }
      

    public override void TakeDamage(float damage)
    {
        if (!isDead)
        {
            hp = Mathf.Clamp(hp - damage, 0, maxHp);
            if (hp > 0)
            {
                healthbar.fillAmount = hp / maxHp;
            }
            else
            {    
                StartCoroutine(EggDie());
            }
        }        
    }
    #endregion;

    #region Private Methods
    private IEnumerator IncreaseHP()
    {
        yield return new WaitForSeconds(0.1f);
        if(healthbar.fillAmount < 1)
        {
            healthbar.fillAmount += 0.1f;
            StartCoroutine(IncreaseHP());
        }
        else
        {            
            myAnim.SetTrigger("action");
            isDead = false;
            Vector3 next = trans.position;
            next.y = startYPos;
            trans.DOJump(next, 0.3f, 2, 1).OnComplete(() =>
            {
                state = FrogState.IDLE;
                ChangeState();
            });            
        }
    }

    private void Move()
    {
        Vector3 next = trans.position;
        if (leftFirst)
        {
            next.x -= 0.5f;
            if (next.x < minX)
            {
                next.x = minX;
                leftFirst = false;
            }
        }
        else
        {
            next.x += 0.5f;
            if (next.x > maxX)
            {
                next.x = maxX;
                leftFirst = true;
            }
        }

        trans.DOJump(next, 0.3f, 2, 0.5f).OnComplete(() =>
        {
            StartCoroutine(MakeDecision(Random.Range(0.5f, 1.5f)));
        });
                
    }
    private void Attack()
    {
        CircleType t = CircleSpawner.Instance.GetRandomCircleType();
        Circle c = CircleManager.Instance.PopCircle(t, 0.7f, firePos.position);
        int hp = CircleSpawner.Instance.GetRandomHP();
        c.Init(hp, CircleOrbit.D, 4, false, false, true);
        c.myBody.velocity = Vector2.down;
        StartCoroutine(MakeDecision(Random.Range(1.5f, 2f)));
    }

    private IEnumerator MakeDecision(float time)
    {
        yield return new WaitForSeconds(time);
        int x = Random.Range(0, 99);
        if (x < 20)
        {
            state = FrogState.IDLE;
        }else if (x < 60)
        {
            state = FrogState.MOVE;
        }
        else
        {
            state = FrogState.ATTACK;
        }
        if(!isDead)
            ChangeState();
    }

    private void ChangeState()
    {
        switch (state)
        {
            case FrogState.IDLE:
                StartCoroutine(MakeDecision(Random.Range(0.5f, 1.5f)));
                break;
            case FrogState.MOVE:
                Move();
                break;
            case FrogState.ATTACK:
                Attack();
                break;
        }
    }
    private IEnumerator EggDie()
    {
        isDead = true;
        CircleManager.Instance.SpawnExplodeEffect(trans.position, Color.green);
        yield return new WaitForSeconds(0.3f);
        gameObject.SetActive(false);
    }
    #endregion;

}
