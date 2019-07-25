using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverTakeNWayBullet : BaseBullet {

    // "Set a number of shot way."
    public int _WayNum = 8;
    // "Set a center angle of shot. (0 to 360)"
    [Range(0f, 360f)]
    public float _CenterAngle = 180f;
    // "Set a angle between bullet and next bullet. (0 to 360)"
    [Range(0f, 360f)]
    public float _BetweenAngle = 10f;
    // "Set a difference speed between shot and next line shot."
    public float _DiffSpeed = 0.5f;
    // "Set a shift angle between shot and next line shot. (-360 to 360)"
    [Range(-360f, 360f)]
    public float _ShiftAngle = 0f;
    // "Set a delay time between shot and next line shot. (sec)"
    public float _NextLineDelay = 0.1f;

    public bool lockPlayer;

    public override void Shot()
    {
        if (lockPlayer)
            _CenterAngle = LockPlayer();
        StartCoroutine(ShotCoroutine());
    }
    IEnumerator ShotCoroutine()
    {
        if (_BulletNum <= 0 || _BulletSpeed <= 0f || _WayNum <= 0)
        {
            Debug.LogWarning("Cannot shot because BulletNum or BulletSpeed or WayNum is not set.");
            yield break;
        }
        if (_Shooting || this.gameObject == null)
        {
            yield break;
        }
        _Shooting = true;

        int wayIndex = 0;

        float bulletSpeed = _BulletSpeed;
        float shiftAngle = 0f;

        for (int i = 0; i < _BulletNum; i++)
        {
            if (_WayNum <= wayIndex && _Shooting)
            {
                wayIndex = 0;

                if (0f < _NextLineDelay)
                {
                    yield return StartCoroutine(UbhUtil.WaitForSeconds(_NextLineDelay));
                }

                bulletSpeed += _DiffSpeed;
                shiftAngle += _ShiftAngle;
            }

            var bullet = GetBullet(transform.position, transform.rotation);
            if (bullet == null)
            {
                break;
            }

            float baseAngle = _WayNum % 2 == 0 ? _CenterAngle - (_BetweenAngle / 2f) : _CenterAngle;

            float angle = UbhUtil.GetShiftedAngle(wayIndex, baseAngle, _BetweenAngle) + shiftAngle;

            ShotBullet(bullet, bulletSpeed, angle);
            
            wayIndex++;
        }

        FinishedShot();
    }
}
