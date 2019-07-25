using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OriginalSprite : MonoBehaviour {

    [SerializeField] private SpecialObject spObj;

    private Vector2 visblePoint;

    void OnBecameVisible()
    {
        spObj.wasInScene = true;
        visblePoint = spObj.trans.position;
    }

    void OnBecameInvisible()
    {
        if (spObj.wasInScene && spObj.gameObject.activeSelf && spObj.Id != -1)
        {
            float distance = Vector2.Distance(visblePoint, spObj.trans.position);
            if(distance>=0.5f)
                spObj.DummyObjInvisible();
        }
    }

}
