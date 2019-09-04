using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//using System;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class GameManager : SingletonMonoBehaviour<GameManager> {

	public static Color32[] colors = new Color32[] {
		new Color32(0, 255, 255, 255), // cyan
		new Color32(0, 255, 0, 255), // green
		new Color32(255, 255, 0, 255), // yellow
		new Color32(255, 0, 255, 255), // violet
		new Color32(255, 128, 0, 255), // orange
		new Color32(255, 0, 0, 255) //red
	};

	public SpriteRenderer background;
	public GAME_RESULT gameResult;
	public GAME_STATE state;
	

	int _timePlay;

	public int timePlay {
		get { 
			return _timePlay;
		}
		set { 
			_timePlay = value;
			if (OnTimeChange != null)
				OnTimeChange(_timePlay);
		}
	}
    
	[HideInInspector]
	public int coin;
	public bool isRevived;
	public int goldPerCoin;    

	public delegate void TimeChange (int time);
	public event TimeChange OnTimeChange;

	public Player player;
    public GAME_PHASE phase;

	void Awake () {
		PopupManager.Instance.scene = SCENE.GAME;
		GameObject go = Instantiate(Resources.Load(Const.SHIP + (int)PlayerData.Instance.selectedShip)) as GameObject;
//		GameObject go = Instantiate(Resources.Load(Const.SHIP + 2)) as GameObject;
		player = (Player)go.GetComponent(typeof(Player));
		
	}

	void Start () {
		gameResult = GAME_RESULT.NONE;
		state = GAME_STATE.PREPARE;
		GameEventManager.Instance.PlayerDead += HandlePlayerDead;
		GameEventManager.Instance.PlayerRevive += HandlePlayerRevive;
		GameEventManager.Instance.BossDefeated += HandleBossDefeated;
		GameEventManager.Instance.BossFinishDie += HandleBossFinishDie;
		SoundManager.Instance.PlayGameMusic();
		AdsManager.Instance.LoadVideoAd();
        AdsManager.Instance.LoadInterAd();
        StartCoroutine(OnStart());
        goldPerCoin = 10 + CampaignManager.campaign.id + 5 * (int)PlayerData.Instance.selectedShip;
	}

	IEnumerator OnStart () {
		GameUIManager.Instance.RunCountDown();
		GameUIManager.Instance.HudAnim.enabled = true;
		yield return new WaitForSeconds(2);
		timePlay = 0;
		state = GAME_STATE.PLAY;
        phase = GAME_PHASE.ENEMY;
        GameEventManager.Instance.OnGameStart();
		InvokeRepeating("AddTime", 1, 1);
		GlobalEventManager.Instance.OnGameStart();
	}

	void Update () {		
		if (Input.GetKeyDown(KeyCode.Escape)) {
			if (state == GAME_STATE.PLAY)
				Pause();
			else if (state == GAME_STATE.PAUSE) {
				state = GAME_STATE.PLAY;
			} else if (state == GAME_STATE.REVIVE) {
				state = GAME_STATE.PREPARE;
			} else if (state == GAME_STATE.GAME_OVER) {
				state = GAME_STATE.PREPARE;
			}
		}
	}

	public void Pause () {
		SoundManager.Instance.PlayUIButtonClick();
		state = GAME_STATE.PAUSE;
		GameUIManager.Instance.ShowPausePopup ();
		GlobalEventManager.Instance.OnButtonPressed("Game", "pause");
	}

	public void GameOverDelay (float delayTime) {
		Invoke("GameOver", delayTime);
	}

	public void GameOver () {        
		PlayerData.Instance.retryTimes++;
		state = GAME_STATE.GAME_OVER;
		GameEventManager.Instance.OnGameEnd();
		GlobalEventManager.Instance.OnGameEnd();
		// save highscore and coin earned		
		PlayerData.Instance.gold += coin;
		GlobalEventManager.Instance.OnCurrencyChanged("gold", "earn", coin.ToString());
		
		// if player defeats the boss, move to next campaign
		if (gameResult == GAME_RESULT.VICTORY) {
			PlayerData.Instance.currentMission = Mathf.Min(PlayerData.Instance.currentMission + 1, CampaignManager.maxId);
			PlayerData.Instance.retryTimes = 0;
		}
		PlayerData.Instance.SaveAllData();
		GameUIManager.Instance.ShowGameOverPopup();
		CancelInvoke("AddTime");       
	}

	void Revive () {
		state = GAME_STATE.REVIVE;
		GameUIManager.Instance.ShowRevivePopup();
	}

	//	void OnApplicationPause () {
	//		if (state == GAME_STATE.PLAY)
	//			Pause();
	//	}

	public static Color GetColorByHP (int hp, int maxHP) {
		hp = Mathf.Clamp(hp, 1, maxHP);
		int p = maxHP / 5;
		int part = hp / p;
		int left = hp % p;
		if (left == 0)
			return colors[part];
		int pi = p * part;
		int r = (int)GetLinearValueSimilarTo(pi, pi + p, colors[part].r, colors[part + 1].r, pi + left);
		int g = (int)GetLinearValueSimilarTo(pi, pi + p, colors[part].g, colors[part + 1].g, pi + left);
		int b = (int)GetLinearValueSimilarTo(pi, pi + p, colors[part].b, colors[part + 1].b, pi + left);
		return Color32ToColor(r, g, b);
	}

	/// <summary>
	/// return x2 in [a2, b2] which is similar to x1 in [a1, b1]
	/// </summary>
	public static float GetLinearValueSimilarTo (float a1, float b1, float a2, float b2, float x1) {
		float t = Mathf.InverseLerp(a1, b1, x1);
		float x2 = Mathf.Lerp(a2, b2, t);
		return x2;
	}

	public static Color Color32ToColor (int r, int g, int b) {
		return new Color(r / 255f, g / 255f, b / 255f);
	}


	void HandleBossFinishDie () {
		player.GetComponent<InputController>().enabled = false;
		player.transform.DOMoveY(10, 6).SetSpeedBased().OnComplete(() => {
			GameOver();
		});
	}

	void HandleBossDefeated () {
		if (gameResult == GAME_RESULT.NONE) {
			gameResult = GAME_RESULT.VICTORY;
			WeaponManager.Instance.DeactivateAllWeapons();
			player.body.enabled = false;
			player.magnetField.enabled = false;
			player.bonusBound.enabled = false;
			SoundManager.Instance.PauseMusic();
		}
	}

	void HandlePlayerDead () {
		SoundManager.Instance.PauseMusic();
		if (isRevived || !AdsManager.Instance.rewardBasedVideo.IsLoaded()) {
			gameResult = GAME_RESULT.GREAT;
			Invoke("GameOver", 2);
		} else {
			Invoke("Revive", 1);
			isRevived = true;
		}
	}

	void HandlePlayerRevive () {
		StartCoroutine("OnPlayerRevive");
		if (phase != GAME_PHASE.BOSS)
			SoundManager.Instance.PlayGameMusic();
		else
			SoundManager.Instance.PlayBossMusic();
	}

	IEnumerator OnPlayerRevive () {
		yield return new WaitForSecondsRealtime(0.2f);
		gameResult = GAME_RESULT.NONE;
		state = GAME_STATE.PLAY;
		player.Revive();
		AdsManager.Instance.LoadVideoAd();
		GameUIManager.Instance.RunCountDown();
		yield return new WaitForSecondsRealtime(2);
		Time.timeScale = 1;
	}

	void AddTime () {
		timePlay++;
	}

	void OnDestroy () {
		OnTimeChange = null;
	}
}

public enum GAME_PHASE
{
    ENEMY,
    BOSS,
    NONE
}
public enum CONTROL_STYLE {
	FOLLOW,
	FIXED,
	NONE
}


public enum GAME_RESULT {
	NONE,
	GREAT,
	VICTORY,
}

public enum GAME_STATE {
	PREPARE,
	PLAY,
	PAUSE,
	REVIVE,
	GAME_OVER
}