using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ShipData {	
    public int id;
	public string shipName;
    public Sprite[] bulletImg;
    public Sprite skillImg;
	public int campaignPassed;
    public float baseDamage;
    public int crystal;
	public float minSpeed, maxSpeed;
    public float baseSkillDamage;
    public float timeDelay;    
}