using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using ABIPlugins;
public class QuestionPopup : SingletonPopup<QuestionPopup> {
    
    public Text title;
    public Text message;
    private Action<bool> OnButtonClickCallback;

    public void Show(string title, string message, bool overlay = true, Action<bool> OnButtonClickCallback = null)
    {
        this.OnButtonClickCallback = OnButtonClickCallback;
        this.title.text = title;
        this.message.text = message;
        base.Show(overlay);
    }

    public void OnYesClick()
    {
        if (OnButtonClickCallback != null) OnButtonClickCallback(true);
        Hide();
    }

    public void OnNoClick()
    {
        if (OnButtonClickCallback != null) OnButtonClickCallback(false);
        Hide();
    }
}
