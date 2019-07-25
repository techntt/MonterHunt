using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Satellite : MonoBehaviour
{

    #region Inspector Variables
    public float numberChild = 0f;
    //public float rotateSpeed = 5f; // time(s) to rotate 360 deg
    #endregion;

    #region Member Variables
    private List<Transform> childs;
    private float Radius;
    private int childHp;
    private SpecialObject spObj;
    #endregion;

    #region Unity Methods
    private void Awake()
    {
        spObj = gameObject.GetComponent(typeof(SpecialObject)) as SpecialObject;
        spObj.type = SpecialObject.SPOBJ_TYPE.SATELLITE;
        spObj.OnChildInit += Init;
        childHp = (int)spObj.HP / 3;
        childHp = Mathf.Clamp(childHp, 1, (int)spObj.hp);
        childs = new List<Transform>();
        for (int i = 0; i < numberChild; i++)
        {
            GameObject nChild = Instantiate(Resources.Load(Const.ORIGINAL_SP)) as GameObject;
            nChild.transform.parent = gameObject.transform;
            nChild.transform.localPosition = new Vector3(0f, Radius, 0f);
            SpecialObject obj = nChild.GetComponent(typeof(SpecialObject)) as SpecialObject;
            ColliderRef.Instance.DamageableRef.Add(obj.myCollider.GetInstanceID(), obj);
            childs.Add(nChild.transform);
            nChild.SetActive(false);
        }
    }
    #endregion;


    #region Public Methods
    #endregion;

    #region Private Methods   
    private void Init()
    {        
        
        /**
         * Caculate properties for child
         * **/ 
        float parentRadius = spObj.myCollider.radius;
        float childRadius = parentRadius / 2;
        Radius = parentRadius + childRadius;   

        for(int i = 0; i < childs.Count; i++)
        {
            GameObject nChild = childs[i].gameObject;
            nChild.SetActive(true);
            SpecialObject sp = nChild.GetComponent(typeof(SpecialObject)) as SpecialObject;
            sp.Init(childHp, parentRadius);            

            // Set position for childs
            var _deltaAngle = 360 / numberChild * i * Mathf.Deg2Rad;
            var offset = new Vector2(Mathf.Sin(_deltaAngle), Mathf.Cos(_deltaAngle)) * Radius;
            childs[i].localPosition = offset;
        }
        spObj.myAmim.SetTrigger("isRotate");
    }
   
    #endregion;
}
