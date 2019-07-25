using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FollowPath : MonoBehaviour {

    #region Inspector Variables
    public PathManager path;
    #endregion;

    #region  Member Variables
    private Transform trans;
    #endregion;

    #region Unity Methods
    private void Start()
    {
        trans = gameObject.GetComponent(typeof(Transform)) as Transform;
        if (path != null)
        {
            MoveTo(path.nextPoint());
        }
    }
    #endregion;


    #region Private Methods
    private void MoveTo(Vector3 point)
    {
        trans.DOMove(point, 1).OnComplete(() =>
        {
            MoveTo(path.nextPoint());
        });
    }
    #endregion;


}
