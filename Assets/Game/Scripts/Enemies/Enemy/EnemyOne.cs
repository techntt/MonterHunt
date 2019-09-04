using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyOne : BaseEnemy
{


    private void OnTriggerEnter2D(Collider2D col)
    {
        Debug.Log("Enemy Collied with : " + col.gameObject.name);
    }
}
