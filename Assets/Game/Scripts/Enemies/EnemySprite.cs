using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySprite : MonoBehaviour
{
    public BaseEnemy enemy;

    private void OnBecameVisible()
    {
        if (!enemy.inScreen)
            enemy.inScreen = true;
    }

    private void OnBecameInvisible()
    {
        if (enemy.inScreen)
        {
            enemy.inScreen = false;
            enemy.Deactive();
        }            
    }
}
