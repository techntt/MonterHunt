using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ABIPlugins;

public class Loader : SingletonPopup<Loader> {

    public Text message;

    public void Show(string message, bool overlay = true)
    {
        this.message.text = message;
        base.Show(overlay);
    }
	
}
