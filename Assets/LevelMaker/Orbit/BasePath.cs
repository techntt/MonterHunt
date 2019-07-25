using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePath : MonoBehaviour {

	[HideInInspector] public float speed ;

    [HideInInspector] public bool pause;

    public virtual void OnPause()
    {
        Debug.Log("Pause");
        pause = true;
    }

    public virtual void OnResume()
    {
        Debug.Log("Resume");
        pause = false;
    }
}