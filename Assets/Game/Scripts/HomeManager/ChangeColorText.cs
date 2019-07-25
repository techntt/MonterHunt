using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ChangeColorText : MonoBehaviour {

	public Text mainText;
	public ParticleSystem star;
	public ParticleSystem bg;
	ParticleSystem.MainModule m1, m2;

	public Color[] colors;
	bool isChanging;

	void ChangeColor () {
		isChanging = true;
		Color c = colors[Random.Range (0, colors.Length)];
		mainText.DOColor(c, 2).OnComplete (() => {
			isChanging = false;
		});
	}

	void Start () {
		m1 = star.main;
		m2 = bg.main;
		InvokeRepeating("ChangeColor", 0, 10);
	}

	void FixedUpdate () {
		if (isChanging) {
			m1.startColor = mainText.color;
			m2.startColor = mainText.color;
		}
	}
}
