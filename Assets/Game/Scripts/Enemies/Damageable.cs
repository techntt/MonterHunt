using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damageable : MonoBehaviour
{
    public float hp,maxHp,damage;
    public bool isDead;

    public virtual void TakeDamage(float damage)
    {

    }
}
