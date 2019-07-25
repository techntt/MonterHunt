using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextEffect : BaseEffect {

	public Text text;

	public void Init (string text, int fontSize, Color color) {
		base.Init();
		this.text.text = text;
		this.text.fontSize = fontSize;
		this.text.color = color;
	}
}