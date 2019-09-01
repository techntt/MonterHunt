using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class RevivePopup : BasePopup {

	public RectTransform bar;
	public RectTransform shipPos;
	public Image ship;
	public Image progress;
	public Text percent;
	public Text scoreLeft;
	public RectTransform reviveNow;
	public Button reviveBtn;
	bool isReward;

	public override void Show () {
		isReward = false;
		Time.timeScale = 0;
		ship.sprite = GameManager.Instance.player.myRender.sprite;
        InitUI();
        base.Show();
    }

    private void InitUI()
    {
        if (GameManager.Instance.phase == GAME_PHASE.BOSS)
            scoreLeft.text = string.Format("Keep fighting!");
        reviveNow.DOScale(new Vector3(1.4f, 1.4f), 1).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear).SetUpdate(true);        
    }
    
	public void Close () {
		SoundManager.Instance.PlayUIButtonClick();
		Time.timeScale = 1;
        GameManager.Instance.gameResult = GAME_RESULT.GREAT;
        GameManager.Instance.GameOver();
        base.Hide();
    }

	public void Revive () {
		#if UNITY_EDITOR || UNITY_STANDALONE_WIN
		isReward = true;
		HandleOnAdClosed(null, null);
		#else
		SoundManager.Instance.PlayUIButtonClick();
		reviveBtn.interactable = false;
		AdsManager.Instance.rewardBasedVideo.OnAdClosed += HandleOnAdClosed;
		AdsManager.Instance.rewardBasedVideo.OnAdRewarded += HandleOnAdRewarded;
		AdsManager.Instance.ShowVideoAd();
		StopCoroutine("OnShow");
		#endif
	}

	// player cancels video and doesn't get revived
	void HandleOnAdClosed (object sender, System.EventArgs e) {
		AdsManager.Instance.rewardBasedVideo.OnAdClosed -= HandleOnAdClosed;
		AdsManager.Instance.rewardBasedVideo.OnAdRewarded -= HandleOnAdRewarded;
		if (isReward) {            
            GameEventManager.Instance.OnPlayerRevive();
            GlobalEventManager.Instance.OnWatchAds("revive", PopupManager.Instance.scene.ToString(), "finish");
        }
        else {
            Time.timeScale = 1;
            GameManager.Instance.gameResult = GAME_RESULT.GREAT;
            GameManager.Instance.GameOverDelay(0.5f);
            GlobalEventManager.Instance.OnWatchAds("revive", PopupManager.Instance.scene.ToString(), "cancel");
        }

        // hide popup
        base.Hide();
    }
    // player get rewarded
    void HandleOnAdRewarded (object sender, GoogleMobileAds.Api.Reward e) {
		GameEventManager.Instance.OnPlayerRevive();
		isReward = true;
	}
    
}