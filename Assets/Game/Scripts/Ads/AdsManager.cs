using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using System;

public class AdsManager : SingletonMonoBehaviour<AdsManager> {

	public RewardBasedVideoAd rewardBasedVideo;
	InterstitialAd ad;

	string appId = "ca-app-pub-9819920607806935~6350142352";
	// real ad id
	//	[HideInInspector]public string videoAdId = "ca-app-pub-9819920607806935/8073862537";
	//	[HideInInspector]public string interAdId = "ca-app-pub-9819920607806935/8265434224";
	// test ad id
//	public string videoAdId = "ca-app-pub-3940256099942544/5224354917";
//	public string interAdId = "ca-app-pub-3940256099942544/1033173712";

	void Start () {
		MobileAds.Initialize(appId);
		this.rewardBasedVideo = RewardBasedVideoAd.Instance;
		rewardBasedVideo.OnAdFailedToLoad += RewardBasedVideo_OnAdFailedToLoad;
	}

	void RewardBasedVideo_OnAdFailedToLoad (object sender, AdFailedToLoadEventArgs e) {
		Debug.Log("HandleFailedToReceiveAd event received with message: " + e.Message);
	}

	public void LoadVideoAd () {
		string videoAdId = "ca-app-pub-9819920607806935/8073862537";
		AdRequest request = new AdRequest.Builder().
//			AddTestDevice("2440f383ab45852764fa76b0dabc17aa").
			Build();
		rewardBasedVideo.LoadAd(request, videoAdId);
	}

	public void ShowVideoAd () {
		if (rewardBasedVideo.IsLoaded()) {
			rewardBasedVideo.Show();
		}
	}

	public void LoadInterAd () {
		string interAdId = "ca-app-pub-9819920607806935/8265434224";
		ad = new InterstitialAd(interAdId);
		AdRequest r = new AdRequest.Builder().
//			AddTestDevice("2440f383ab45852764fa76b0dabc17aa").
			Build();
		ad.LoadAd(r);
	}

	public void ShowInterAd () {
		if (ad != null && ad.IsLoaded()) {
			ad.Show();
		}
	}
}