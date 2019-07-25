using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseBullet : UbhBaseShot
{    
    public float bulletSize = 0.5f;
    public bool bulletIsCircle = true;
    public CircleType circle_type = CircleType.NORMAL;
    public bool randomCircleType = false;
    public bool notBoom = true;
    public bool hasBonus = true;
    
    public override void Shot()
    {
        
    }

    protected override UbhBullet GetBullet(Vector3 position, Quaternion rotation, bool forceInstantiate = false)
    {
        if (_BulletPrefab == null)
        {
            Debug.LogWarning("Cannot generate a bullet because BulletPrefab is not set.");
            return null;
        }
        GameObject goBullet;
        if (bulletIsCircle)
        {
            if (randomCircleType)
            {
                circle_type = (CircleType)Random.Range(0, 3);
                if (circle_type == CircleType.BOMB)
                    circle_type = CircleType.NORMAL;
            }
            Circle cir = CircleManager.Instance.PopCircle(circle_type, bulletSize, position);
            int hp = CircleSpawner.Instance.GetRandomHP();
            cir.Init(hp, CircleOrbit.L, 0, false, false, hasBonus);

            goBullet  = cir.gameObject;
            if (goBullet == null)
            {
                return null;
            }
        }
        else
        {
            goBullet = null;
            if (goBullet == null)
            {
                return null;
            }
        }
        

        // get or add UbhBullet component
        var bullet = goBullet.GetComponent<UbhBullet>();
        if (bullet == null)
        {
            bullet = goBullet.AddComponent<UbhBullet>();
        }

        return bullet;
    }

    public float LockPlayer()
    {
        Transform target = GameManager.Instance.player1.transform;        
        if (target != null)
        {
            var xDistance = target.position.x - transform.position.x;
            var yDistance = target.position.y - transform.position.y;
            var angle = Mathf.Atan2(xDistance, yDistance) * Mathf.Rad2Deg;
            return -Get360Angle(angle);
        }
        return 0f;
    }

    public float LockTarget(Transform target)
    {
        if (target != null)
        {
            var xDistance = target.position.x - transform.position.x;
            var yDistance = target.position.y - transform.position.y;
            var angle = Mathf.Atan2(xDistance, yDistance) * Mathf.Rad2Deg;
            return -Get360Angle(angle);
        }
        return 0f;
    }

    private static float Get360Angle(float angle)
    {       
        while (angle < 0f)
        {
            angle += 360f;
        }
        while (360f < angle)
        {
            angle -= 360f;
        }
        return angle;        
    }
    
    
}
