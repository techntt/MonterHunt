using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Linear : BasePath {

    #region Inspector Variables
    public Vector2 startPoint;
	public Vector2 endPoint;
    #endregion;

    #region Member Variables
    private float timer = 0f;
    private float distance;
    private float timeMove = -1f;
    private SpecialObject spObj;
    #endregion;

    #region Unity Methods
    private void Awake()
    {
        spObj = gameObject.GetComponent(typeof(SpecialObject)) as SpecialObject;
        spObj.OnInitPos += Init;
        pause = false;
    }
  
	void FixedUpdate () {
        if (timer <= timeMove && !pause)
        {
            float step = timer / timeMove;
            spObj.trans.position = Vector2.Lerp(startPoint, endPoint, step);
            timer += 0.02f;
        }        
	}
    #endregion;

    #region Public Methods

    #endregion;

    #region Private Methods

    private void Init()
    {
        timer = 0f;
        transform.position = startPoint;
        distance = Vector2.Distance(startPoint, endPoint);
        timeMove = distance / speed;
    }
    #endregion;
}