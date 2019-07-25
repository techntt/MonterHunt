using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NSinWayBullet : BaseBullet
{
    // "Set a number of shot way."
    public int _WayNum = 1;
    // "Set a center angle of shot. (0 to 360)"
    [Range(0f, 360f)]
    public float _CenterAngle = 180f;
    // "Set a size of wave range. (0 to 360)"
    [Range(0f, 360f)]
    public float _WaveRangeSize = 40f;
    // "Set a speed of wave. (0 to 30)"
    [Range(0f, 30f)]
    public float _WaveSpeed = 10f;
    // "Set a angle between bullet and next bullet. (0 to 360)"
    [Range(0f, 360f)]
    public float _BetweenAngle = 10f;
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

        for (int i = 0; i < _BulletNum; i++)
        {
            if (_WayNum <= wayIndex && _Shooting)
            {
                wayIndex = 0;

                if (0f < _NextLineDelay)
                {
                    yield return StartCoroutine(UbhUtil.WaitForSeconds(_NextLineDelay));
                }
            }

            var bullet = GetBullet(transform.position, transform.rotation);
            if (bullet == null)
            {
                break;
            }

            float baseAngle = _WayNum % 2 == 0 ? _CenterAngle - (_BetweenAngle / 2f) : _CenterAngle;

            float angle = UbhUtil.GetShiftedAngle(wayIndex, baseAngle, _BetweenAngle);

            ShotBullet(bullet, _BulletSpeed, angle, false, null, 0f, true, _WaveSpeed, _WaveRangeSize);
            
            wayIndex++;
        }

        FinishedShot();
    }
}
