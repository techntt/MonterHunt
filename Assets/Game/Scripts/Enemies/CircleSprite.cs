using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleSprite : MonoBehaviour
{

    public Circle myCircle;
    public Animator myAnim;

    void OnBecameVisible()
    {
        myCircle.wasInScene = true;
        myAnim.enabled = true;
    }

    void OnBecameInvisible()
    {
        if (myCircle.wasInScene && myCircle.gameObject.activeSelf)
        {
            if (myCircle.hasScore)
                GameEventManager.Instance.OnCircleExit(myCircle);
            CircleManager.Instance.PushCircle(myCircle);
        }
        transform.localRotation = Quaternion.Euler(0, 0, 0);
        myAnim.enabled = false;
    }
}