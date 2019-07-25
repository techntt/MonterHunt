using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponDataCalculator : Singleton<WeaponDataCalculator> {

	public const float LASER_CYAN = 1.5f;
	public const float LASER_YELLOW = 2;
	public const float LASER_RED = 2.5f;

	public const float POWER_X_2 = 0.5f;
	public const float POWER_X_3 = 1;
	public const float POWER_X_4 = 1.5f;

	public const float DIVIDE_30 = 2;
	public const float DIVIDE_180 = 4;
	public const float DIVIDE_360 = 8;

	public const float ROTATE_CYAN = 1;
	public const float ROTATE_YELLOW = 1.5f;
	public const float ROTATE_RED = 2;

	public const int POINT_X2 = 2;
	public const int POINT_X3 = 3;
	public const int POINT_X4 = 4;

	public const float SAW_DPS = 40;
	public const float BARRIER_DPS = 40;
	public const float BOMB_DAMAGE = 50;
	public const float ROCKET_DAMAGE = 50;
	public const float SEEK_DAMAGE = 50;
	public const float LIGHTNING_DAMAGE = 40;

	/// <summary>
	/// max level of basic weapons
	/// </summary>
	public const int MAX_WEAPON_LEVEL = 59;

	public const float superFireRate = 0.05f;
	public const float maxFireRate = 0.08f;
	public const float minFireRate = 0.25f;

	public const float superBulletSpeed = 15;
	public const float maxBulletSpeed = 10;
	public const float minBulletSpeed = 6;


	public static float maxCircleHP = 50;

	public const float minFireTime = 12.5f; // time to kill a 50-hp circle at min weapon level
	public const float maxFireTime = 1; // time to kill a 50-hp circle at max weapon level

	public const int maxNumOfBullet = 15;
    
	// Calculated parameter
	// fire speed of each weapon level
	private float[] fireRates;
	// speed of bullet with each weapon level
	private float[] bulletSpeeds;
	// DPS of each weapon level
	private float[] dps;
	private float[] rays;

	public WeaponDataCalculator () {
		fireRates = new float[MAX_WEAPON_LEVEL + 1];
		bulletSpeeds = new float[MAX_WEAPON_LEVEL + 1];
		dps = new float[MAX_WEAPON_LEVEL + 1];
		rays = new float[MAX_WEAPON_LEVEL + 1];
		float minDPS = maxCircleHP / minFireTime;
		float maxDPS = maxCircleHP / maxFireTime;
		float deltaDPS = (maxDPS - minDPS) / MAX_WEAPON_LEVEL;
		float deltaFireRate = (minFireRate - maxFireRate) / MAX_WEAPON_LEVEL;
		float deltaBulletSpeed = (maxBulletSpeed - minBulletSpeed) / MAX_WEAPON_LEVEL;
		for (int i = 0; i <= MAX_WEAPON_LEVEL; i++) {
			dps[i] = minDPS + i * deltaDPS;
			bulletSpeeds[i] = minBulletSpeed + i * deltaBulletSpeed;
			fireRates[i] = minFireRate - deltaFireRate * i;
			rays[i] = Mathf.Clamp(i * 15 / MAX_WEAPON_LEVEL + 1, 1, 15);
		}
	}

	// Get fire rate based on power level
	public float FireRate (int level) {
		return fireRates[Mathf.Clamp(level, 0, MAX_WEAPON_LEVEL)];
	}

	// Get bullet speed based on power level
	public float Speed (int level) {
		return bulletSpeeds[Mathf.Clamp(level, 0, MAX_WEAPON_LEVEL)];
	}

	public float DPS (int level) {
		return dps[Mathf.Clamp(level, 0, MAX_WEAPON_LEVEL)];
	}

	// Get number of bullets based on power level
	public int Rays (int level) {
		int ray = ((level + 1) * 15 / MAX_WEAPON_LEVEL) + 1;
		return Mathf.Clamp(ray, 1, 15);
	}

	public float CalculateBaseDamagePerBullet (int level) {
		level = Mathf.Clamp(level, 0, MAX_WEAPON_LEVEL);
		return dps[level] * fireRates[level] / rays[level];
	}
}