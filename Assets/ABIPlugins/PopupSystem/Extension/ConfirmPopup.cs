using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using ABIPlugins;

public class ConfirmPopup : SingletonPopup<ConfirmPopup>
{
    public Text title;
    public Text message;
    private Action OnOKClickCallback;

    public void Show(string title, string message, bool overlay = true, Action OnOKClickCallback = null)
    {
        this.OnOKClickCallback = OnOKClickCallback;
        this.title.text = title;
        this.message.text = message;
        base.Show(overlay);
    }

    public void OnOKClick()
    {
        if (OnOKClickCallback != null) OnOKClickCallback();
        Hide();
    }
}
