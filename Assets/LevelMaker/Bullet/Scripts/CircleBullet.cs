using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleBullet : BaseBullet {

    public override void Shot()
    {
        if (_BulletNum <= 0 || _BulletSpeed <= 0f)
        {
            Debug.LogWarning("Cannot shot because BulletNum or BulletSpeed is not set.");
            return;
        }
        _Shooting = true;
        float shiftAngle = 360f / (float)_BulletNum;

        for (int i = 0; i < _BulletNum; i++)
        {

            var bullet = GetBullet(transform.position, transform.rotation);
            if (bullet == null || !_Shooting)
            {
                break;
            }

            float angle = shiftAngle * i;

            ShotBullet(bullet, _BulletSpeed, angle);
        }

        FinishedShot();
    }
}
