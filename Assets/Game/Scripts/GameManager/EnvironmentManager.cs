using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnvironmentManager : MonoBehaviour {

	public SpriteRenderer background;

	public EnvironmentData[] environments;

	void Start () {
		
	}
}

[System.Serializable]
public class EnvironmentData {
	public Color bgColor;
	public Color cloudColor;
	public Color starColor;
	public int spriteRow;
}