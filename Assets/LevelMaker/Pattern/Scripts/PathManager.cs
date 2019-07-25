using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathManager : MonoBehaviour {

    #region Inspector Variables
    public Transform[] paths;
    #endregion;

    #region Member Variables    
    private int index;
    private Transform trans;
    #endregion;

    #region Unity Methods
    private void Awake()
    {
        index = 0;
        trans = gameObject.GetComponent(typeof(Transform)) as Transform;
    }
    private void OnDrawGizmos()
    {
        if (paths != null && paths.Length >= 1)
        {
            for (int i = 0; i < paths.Length; i++)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(i == 0 ? gameObject.transform.position : paths[i - 1].position, paths[i].position);
            }
        }           
    }
    #endregion

    #region Public Methods
    public Vector3 nextPoint()
    {        
        Vector3 next = new Vector3();
        if(paths!=null && paths.Length > 1)
        {
            if(index ==0)
            {
                next = trans.position;
            } else
            {
                next = paths[index - 1].position;
            }
            index++;
            if (index == paths.Length)
                index = 0;
        }
        else
        {
            next = trans.position;
        }
        return next;
    }
    #endregion;

}
