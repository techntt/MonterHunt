using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zigzag : BasePath {

    #region Inspector Variables
    public Vector2 startPoint;
    public float amplitude = 1f;
    #endregion;

    #region Member Variables
    private SpecialObject spObj;
    private float halfApm;
    #endregion;

    #region Unity Methods
    private void Start()
    {
        spObj = gameObject.GetComponent(typeof(SpecialObject)) as SpecialObject;
        spObj.trans.position = startPoint;
        spObj.myBody.velocity = Vector2.down * speed;
        halfApm = amplitude / 2;
        pause = false;
    }

    private void Update()
    {
        if(!pause)
            spObj.trans.position = new Vector3(Mathf.PingPong(Time.time, amplitude)- halfApm, spObj.trans.position.y, spObj.trans.position.z);       
    }
    #endregion;    

    #region Public Methods
    public override void OnPause()
    {
        base.OnPause();
        spObj.myBody.velocity = Vector2.zero;
    }

    public void OnContinue()
    {
        base.OnResume();
        spObj.myBody.velocity = Vector2.down * speed;
    }
    #endregion;

}
