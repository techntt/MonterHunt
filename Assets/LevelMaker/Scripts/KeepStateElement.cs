using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeepStateElement : MonoBehaviour {

    private Quaternion initQt;
    private Transform trans;
    // Use this for initialization
    private void Awake()
    {
        trans = gameObject.GetComponent(typeof(Transform)) as Transform;
        initQt = trans.rotation;
    }
    void Start () {
       
	}

    private void LateUpdate()
    {
        trans.rotation = initQt;
    }
}
