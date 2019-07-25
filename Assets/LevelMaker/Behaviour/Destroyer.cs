using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyer : MonoBehaviour {

    #region Inspector Variables
    public GameObject destroyPref;
    public Rigidbody2D myBody;
    #endregion;

    #region Member Variables
    private int numberObj;
    private int screenW, screenH;
    Vector3 centerScreen;
    private float speed;
    private int invisionCount;
    public enum Destroy_Style
    {
        Easy,
        Normal,
        Hard
    }
    #endregion;

    #region Unity Methods
    
    #endregion;

    #region Public Methods
    public void Init(Destroy_Style style,float speed =1)
    {
        invisionCount = 0;
       screenW = Screen.width;
       screenH = Screen.height;
       centerScreen = new Vector3(screenW / 2, 0);       
       switch (style)
        {
            case Destroy_Style.Easy:
                this.speed = 1;
                createEasyDestroyer();
                break;
            case Destroy_Style.Normal:
                this.speed = 2;
                createNormalDestroyer();
                break;
            case Destroy_Style.Hard:
                this.speed = 3;
                createHardDestroyer();
                break;
        }
    }

    public void ChildOnBecameInvisible()
    {
        invisionCount++;
        if(invisionCount == numberObj)
        {
            this.gameObject.SetActive(false);
        }
    }
    #endregion;

    #region Private Methods
    private void createEasyDestroyer()
    {        
        numberObj = 1;             
        Vector3 pos = Camera.main.ScreenToWorldPoint(centerScreen);
        pos.x += Random.Range(-0.5f, 0.5f);
        pos.y = gameObject.transform.position.y;
        pos.z = 0;

        GameObject obj = Instantiate(destroyPref) as GameObject;
        obj.transform.parent = gameObject.transform;
        obj.transform.position = pos;
        SpriteDestroyer sp = obj.GetComponent(typeof(SpriteDestroyer)) as SpriteDestroyer;
        sp.Init(this);
        myBody.velocity = Vector2.down*speed;

    }

    private void createNormalDestroyer()
    {
        numberObj = 2;
        Vector3 pos = Camera.main.ScreenToWorldPoint(centerScreen);
        pos.x += Random.Range(-0.5f, 0.5f);
        if (pos.x > 0)
            pos.x += 0.5f;
        else
            pos.x -= 0.5f;
        pos.y = gameObject.transform.position.y;
        pos.z = 0;
        for(int i = 0; i < numberObj; i++)
        {
            GameObject obj = Instantiate(destroyPref) as GameObject;
            obj.transform.parent = gameObject.transform;
            Vector3 cPos = pos;
            if (pos.x > 0)
                cPos.x = pos.x - (2f *i);
            else
                cPos.x = pos.x + (2f*i);
            cPos.y += Random.Range(0f, 1f) * i;
            obj.transform.position = cPos ;
            SpriteDestroyer sp = obj.GetComponent(typeof(SpriteDestroyer)) as SpriteDestroyer;
            sp.Init(this);
        }

        myBody.velocity = Vector2.down * speed;
    }

    private void createHardDestroyer()
    {
        numberObj = 3;
        Vector3 pos = Camera.main.ScreenToWorldPoint(centerScreen);
        pos.x += Random.Range(-0.5f, 0.5f);
        pos.y = gameObject.transform.position.y;
        pos.z = 0;
        for (int i = 0; i < numberObj; i++)
        {
            GameObject obj = Instantiate(destroyPref) as GameObject;
            obj.transform.parent = gameObject.transform;
            Vector3 cPos = pos;
            int side = 1;
            if (i % 2 == 0)
                side = 1;
            else
                side = -1;
            if (pos.x > 0)
                cPos.x = pos.x - (2f * Mathf.Clamp(i,0,1) * side);
            else
                cPos.x = pos.x + (2f * Mathf.Clamp(i, 0, 1) * side);
            cPos.y += Random.Range(0f, 1f) * i;
            obj.transform.position = cPos;
            SpriteDestroyer sp = obj.GetComponent(typeof(SpriteDestroyer)) as SpriteDestroyer;
            sp.Init(this);
        }

        myBody.velocity = Vector2.down * speed;
    }
    #endregion;
}
