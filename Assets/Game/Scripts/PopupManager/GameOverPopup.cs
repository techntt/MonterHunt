using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;
using ABIPlugins;

public class GameOverPopup : SingletonPopup<GameOverPopup> {

	public Text title;
	public Text score;
	public Text gold;
	public Text time;
	public Text collectedGold;
	public Text bonusGold;
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
	int goldCollect;
	int goldBonus;
	Tweener temp;
	bool watchVideo;

	public void Show () {
		watchVideo = false;
		SoundManager.Instance.PlaySfx(vitorySfx);
		title.text = GameManager.Instance.gameResult.ToString();
		score.text = "0";
		gold.text = "0";
		collectedGold.text = "0";
		bonusGold.text = "0";
		okButton.interactable = false;
		rewardBtn.interactable = false;
		shipImg.sprite = GameManager.Instance.player1.myRender.sprite;
		base.Show(true, () => {
			int min = GameManager.Instance.timePlay / 60;
			int sec = GameManager.Instance.timePlay % 60;
			time.text = string.Format("{0}:{1}", min, sec.ToString("D2"));
			scoreValue = 0;
			goldValue = 0;
			goldCollect = 0;
			goldBonus = 0;
			int obj = CampaignManager.campaign.objective;
			// tween the score and the progress
			DOTween.To(() => scoreValue, x => scoreValue = x, GameManager.Instance.score, 1).OnUpdate(() => {
				score.text = scoreValue.ToString();
				proText.text = string.Format("{0}", Mathf.Min((float)scoreValue / obj, 1).ToString("P0"));
				SoundManager.Instance.PlaySfxNoRewind (scoreSfx);
			}).OnComplete(() => {
				// tween the collected gold value
				DOTween.To(() => goldCollect, x => goldCollect = x, GameManager.Instance.coin, 0.5f).OnUpdate(() => {
					collectedGold.text = goldCollect.ToString();
				}).OnComplete(() => {
					// tween the bonus gold value
					DOTween.To(() => goldBonus, x => goldBonus = x, GameManager.Instance.bonusCoin, 0.5f).OnUpdate(() => {
						bonusGold.text = goldBonus.ToString();
					}).OnComplete ( () => {
						// tween the total gold value
						DOTween.To(() => goldValue, x => goldValue = x, goldCollect + goldBonus, 0.5f).OnUpdate(() => {
							gold.text = goldValue.ToString();
						}).OnComplete(() => {
							okButton.interactable = true;
							if (AdsManager.Instance.rewardBasedVideo.IsLoaded()) {
								rewardBtn.interactable = true;
								temp = rewardBtnRect.DOScale(new Vector3(1.3f, 1.3f), 0.5f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear).SetUpdate(true);
							}
						});
					});
				});
			});
			// tween the ship's position and the fill amount value
			float des = GameManager.GetLinearValueSimilarTo(0, CampaignManager.campaign.objective, 0, bar.rect.width, GameManager.Instance.score);
			ship.DOLocalMoveX(des, 1);
			progress.DOFillAmount((float)GameManager.Instance.score / CampaignManager.campaign.objective, 1);
		});
	}

	public override void Hide () {
		if (!watchVideo)
			AdsManager.Instance.ShowInterAd();
		SoundManager.Instance.PlayUIButtonClick();
		base.Hide(() => {
			AdsManager.Instance.rewardBasedVideo.OnAdRewarded -= HandleOnAdRewarded;
			SceneManager.LoadScene(Const.SCENE_HOME);
		});
	}

	public void DoubleReward () {
		rewardBtn.interactable = false;
		temp.Kill();
		rewardBtnRect.localScale = new Vector3(1, 1, 1);
		#if UNITY_EDITOR || UNITY_STANDALONE_WIN
		PlayerData.Instance.gold += goldCollect + goldBonus;
		DOTween.To(() => goldValue, x => goldValue = x, (goldCollect + goldBonus)*2, 1).OnUpdate(() => {
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
		PlayerData.Instance.gold += goldCollect + goldBonus;
		DOTween.To(() => goldValue, x => goldValue = x, (goldCollect + goldBonus)*2, 1).OnUpdate(() => {
			gold.text = goldValue.ToString();
		});
	}
}