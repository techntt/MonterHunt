using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class GameOverPopup : BasePopup {

	public Text title;
	public Text score;
	public Text gold;
	public Text time;
	public Button okButton;
	public Button rewardBtn;
	public RectTransform ship;
	public RectTransform bar;
	public Image shipImg;
	public Image progress;
	public Text proText;
	public RectTransform rewardBtnRect;

	public AudioClip vitorySfx;
	public AudioClip scoreSfx;

	int scoreValue;
	int goldValue;
	Tweener temp;
	bool watchVideo;

	public override void Show () {
		watchVideo = false;
		SoundManager.Instance.PlaySfx(vitorySfx);
		title.text = GameManager.Instance.gameResult.ToString();
		score.text = "0";
		gold.text = "0";
		okButton.interactable = false;
		rewardBtn.interactable = false;
		shipImg.sprite = GameManager.Instance.player.myRender.sprite;

        //
        int min = GameManager.Instance.timePlay / 60;
        int sec = GameManager.Instance.timePlay % 60;
        time.text = string.Format("{0}:{1}", min, sec.ToString("D2"));
        scoreValue = 0;
        goldValue = 0;
        // tween the total gold value
        DOTween.To(() => goldValue, x => goldValue = x, GameManager.Instance.coin, 0.5f).OnUpdate(() => {
            gold.text = goldValue.ToString();
        }).OnComplete(() => {
            okButton.interactable = true;
            if (AdsManager.Instance.rewardBasedVideo.IsLoaded())
            {
                rewardBtn.interactable = true;
                temp = rewardBtnRect.DOScale(new Vector3(1.3f, 1.3f), 0.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear).SetUpdate(true);
            }
        });
        base.Show();
    }

    public override void Hide () {
		if (!watchVideo)
			AdsManager.Instance.ShowInterAd();
		SoundManager.Instance.PlayUIButtonClick();
        AdsManager.Instance.rewardBasedVideo.OnAdRewarded -= HandleOnAdRewarded;
        SceneManager.LoadScene(Const.SCENE_HOME);
        base.Hide();
    }

	public void DoubleReward () {
		rewardBtn.interactable = false;
		temp.Kill();
		rewardBtnRect.localScale = new Vector3(1, 1, 1);
		#if UNITY_EDITOR || UNITY_STANDALONE_WIN
		PlayerData.Instance.gold += GameManager.Instance.coin;
		DOTween.To(() => goldValue, x => goldValue = x, (GameManager.Instance.coin) *2, 1).OnUpdate(() => {
			gold.text = goldValue.ToString();
			SoundManager.Instance.PlaySfxNoRewind (scoreSfx);
		});
		#else
		rewardBtn.interactable = false;
		AdsManager.Instance.rewardBasedVideo.OnAdRewarded += HandleOnAdRewarded;
		AdsManager.Instance.ShowVideoAd();
		#endif
	}

	void HandleOnAdRewarded (object sender, GoogleMobileAds.Api.Reward e) {
		watchVideo = true;
		PlayerData.Instance.gold += GameManager.Instance.coin ;
		DOTween.To(() => goldValue, x => goldValue = x, (GameManager.Instance.coin) *2, 1).OnUpdate(() => {
			gold.text = goldValue.ToString();
		});
        base.Hide();
	}
}