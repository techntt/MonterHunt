using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Division : BasePath
{

    #region Inspector Variables
    public Vector2 startPoint;
    public Vector2 dividePoint;
    public int divisor = 1;
    public float divisorSize = 0.4f;
    #endregion;

    #region Member Variables
    private SpecialObject spObj;
    private bool divided;
    
    private float timer = 0f;
    private float distance;
    private float timeMove = -1f;
    
    #endregion;

    #region Unity Methods
    private void Awake()
    {
        spObj = gameObject.GetComponent(typeof(SpecialObject)) as SpecialObject;
        spObj.type = SpecialObject.SPOBJ_TYPE.DIVISION;
        spObj.OnChildInit += Init;
        divided = false;
    }

    private void FixedUpdate()
    {
        if (!divided)
        {

            if (timer <= timeMove)
            {
                float step = timer / timeMove;
                spObj.trans.position = Vector2.Lerp(startPoint, dividePoint, step);
                timer += 0.02f;
            }
            else
            {
                StartCoroutine(Divide());
                divided = true;
            }            
        }
    }
    #endregion;

    #region Private Methods
    private void Init()
    {        
        timer = 0f;
        transform.position = startPoint;
        distance = Vector2.Distance(startPoint, dividePoint);
        timeMove = distance / speed;
    }

    IEnumerator Divide()
    {
        int i = 0;
        float size = spObj.myCollider.radius * 2;
        float step = ((size - 0.2f) *divisor) / spObj.hp;
        while (spObj.hp > 0)
        {
            spObj.HP -= divisor;
            size -= step;
            spObj.myRender.size = new Vector2(size, size);
            spObj.myCollider.radius = size / 2;

            SpecialObject obj = CircleManager.Instance.PopSPObj();
            obj.Init(divisor, divisorSize);
            obj.Id = -2;
            obj.trans.position = spObj.trans.position;
            obj.myBody.velocity = new Vector2(Mathf.Cos(i * 5), Mathf.Sin(i * 5)).normalized * 2;         
            i++;
            yield return new WaitForSeconds(0.1f);
        }
        spObj.DummyObjInvisible();
    }
    #endregion;
}
