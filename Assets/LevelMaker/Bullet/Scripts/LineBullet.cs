using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineBullet : BaseBullet {

    [Range(0f, 360f)]
    public float _Angle = 180f;
    public float _BetweenDelay = 0.1f;
    public bool lockPlayer;
    public bool isLook;
    public Transform target;

    public override void Shot()
    {
        if (lockPlayer)
            _Angle = LockPlayer();
        else if(target != null)
        {
            _Angle = LockTarget(target);
        }
       
        StartCoroutine(ShotCoroutine());
        if (isLook)
        {            
            StartCoroutine(AimingCoroutine());
        }
    }

    IEnumerator ShotCoroutine()
    {
        if (_BulletNum <= 0 || _BulletSpeed <= 0f)
        {
            Debug.LogWarning("Cannot shot because BulletNum or BulletSpeed is not set.");
            yield break;
        }
        if (_Shooting)
        {
            yield break;
        }
        _Shooting = true;

        for (int i = 0; i < _BulletNum; i++)
        {
            if (0 < i && 0f < _BetweenDelay)
            {
                yield return StartCoroutine(UbhUtil.WaitForSeconds(_BetweenDelay));
            }

            var bullet = GetBullet(transform.position, transform.rotation);
            if (bullet == null || !_Shooting)
            {
                break;
            }

            ShotBullet(bullet, _BulletSpeed, _Angle);
        }

        FinishedShot();
    }

    IEnumerator AimingCoroutine()
    {
        while (isLook)
        {
            if (_Shooting == false)
            {
                yield break;
            }
            _Angle = LockPlayer();

            yield return 0;
        }
    }
}
