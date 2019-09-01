using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour {

    public Slider slider;

	string checkScore = "CheckScore";

	void Start () {
		InvokeRepeating(checkScore, 3, 1);
	}

	void OnDisable () {
		CancelInvoke(checkScore);
	}

	void CheckScore () {
		
	}
}
