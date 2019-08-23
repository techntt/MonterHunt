using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingPanel : BaseMenuPopup
{
    #region Inspector Variables
    public Toggle tgSound, tgMusic, tgVibrate;
    #endregion;

    #region Member Variables
    #endregion;

    #region Public Methods
    public override void Show()
    {
        tgMusic.isOn = PlayerSettingData.Instance.isMusic;
        tgSound.isOn = PlayerSettingData.Instance.isSound;
        tgVibrate.isOn = PlayerSettingData.Instance.isVibrate;
        base.Show();
    }

    public override void Hide()
    {
        PlayerSettingData.Instance.Save();
        base.Hide();
    }
    public void SoundChangeState(bool isOn)
    {
        PlayerSettingData.Instance.isSound = isOn;
        if (isOn)
            SoundManager.Instance.SoundOn();
        else
            SoundManager.Instance.SoundOff();
    }

    public void MusicChangeState(bool isOn)
    {
        PlayerSettingData.Instance.isMusic = isOn;
        if (isOn)
            SoundManager.Instance.MusicOn();
        else
            SoundManager.Instance.MusicOff();
    }

    public void VibrateChangeState(bool isOn)
    {
        PlayerSettingData.Instance.isVibrate = isOn;
    }
    #endregion;

}
