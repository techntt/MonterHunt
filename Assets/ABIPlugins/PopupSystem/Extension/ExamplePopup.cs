using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ABIPlugins;
public class ExamplePopup : SingletonPopup<ExamplePopup> {
    public Text content;
	public void Show(string content) {
        this.content.text = content;
        base.Show();
    }
}
