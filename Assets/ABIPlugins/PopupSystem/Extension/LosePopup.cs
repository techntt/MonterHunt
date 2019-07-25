using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using ABIPlugins;
public class LosePopup : SingletonPopup<LosePopup>
{
    public Text yourScore;

    public void Show(int yourScore, bool overlay = true)
    {
        this.yourScore.text = yourScore.ToString();
        base.Show(overlay);
    }
}
