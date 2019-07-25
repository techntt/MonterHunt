using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ABIPlugins;
public class InfoPopup : SingletonPopup<InfoPopup>
{
    public Text title;
    public Text message;

    public void Show(string title, string message, bool overlay = true)
    {
        this.title.text = title;
        this.message.text = message;
        base.Show(overlay);
    }
}
