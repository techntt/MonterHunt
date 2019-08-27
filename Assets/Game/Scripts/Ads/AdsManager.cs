using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using System;

public class AdsManager : SingletonMonoBehaviour<AdsManager> {

	public RewardBasedVideoAd rewardBasedVideo;
	InterstitialAd ad;

	string appId = "";	

	void Start () {
		MobileAds.Initialize(appId);
		this.rewardBasedVideo = RewardBasedVideoAd.Instance;
		rewardBasedVideo.OnAdFailedToLoad += RewardBasedVideo_OnAdFailedToLoad;
	}

	void RewardBasedVideo_OnAdFailedToLoad (object sender, AdFailedToLoadEventArgs e) {
		Debug.Log("HandleFailedToReceiveAd event received with message: " + e.Message);
	}

	public void LoadVideoAd () {
		string videoAdId = "";
		AdRequest request = new AdRequest.Builder().
			Build();
		rewardBasedVideo.LoadAd(request, videoAdId);
	}

	public void ShowVideoAd () {
		if (rewardBasedVideo.IsLoaded()) {
			rewardBasedVideo.Show();
		}
	}

	public void LoadInterAd () {
		string interAdId = "";
		ad = new InterstitialAd(interAdId);
		AdRequest r = new AdRequest.Builder().
			Build();
		ad.LoadAd(r);
	}

	public void ShowInterAd () {
		if (ad != null && ad.IsLoaded()) {
			ad.Show();
		}
	}
}