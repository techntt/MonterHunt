using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteDestroyer : MonoBehaviour {

    private Destroyer parent;

    public void Init(Destroyer parent)
    {
        this.parent = parent;
    }
    private void OnBecameInvisible()
    {
        parent.ChildOnBecameInvisible();
    }
}
