using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEventManager : SingletonMonoBehaviour<GameEventManager> {
	public delegate void SingleGameEvent();
	public event SingleGameEvent GameStart;
	public event SingleGameEvent GameEnd;
	public event SingleGameEvent PlayerDead;
	public event SingleGameEvent PlayerRevive;
	public event SingleGameEvent BossAppear;
	public event SingleGameEvent BossFinishAppear;
	public event SingleGameEvent BossDefeated;
	public event SingleGameEvent BossFinishDie;

	public delegate void CircleEvent (Circle c);
	public event CircleEvent CircleSpawned;
	public event CircleEvent CircleExplode;
	public event CircleEvent CircleExit;
	public event CircleEvent CircleCollide;

	public delegate void PlayerCoinEvent (Player p, Coin c);
	public event PlayerCoinEvent PlayerGetCoin;

	public delegate void PlayerBonusEvent (Player p, BonusType b, int variant = 0);
	public event PlayerBonusEvent PlayerGetBonus;

	public delegate void PlayerHealthEvent (Player p, float hpChange);
	public event PlayerHealthEvent PlayerGainHealth;
	public event PlayerHealthEvent PlayerLostHealth;
	public event PlayerHealthEvent PlayerHealthChanged;

	public delegate void PlayerIntEvent (Player p, int value);
	public event PlayerIntEvent PlayerDealDamage;
	public event PlayerIntEvent PlayerGetScore;
	public event PlayerIntEvent PlayerUseCoin;

	public delegate void GamePhaseEvent (GAME_PHASE phase);
	public event GamePhaseEvent GamePhaseChanged;

	public delegate void ButtonEvent (string btnName);
	public event ButtonEvent ButtonPressed;

	public void OnGameStart () {
		if (GameStart != null)
			GameStart();
	}

	public void OnGameEnd () {
		if (GameEnd != null)
			GameEnd();
	}

	public void OnPlayerDead () {
		if (PlayerDead != null)
			PlayerDead();
	}

	public void OnPlayerRevive () {
		StartCoroutine("IPlayerRevive");
	}

	IEnumerator IPlayerRevive () {
		yield return new WaitForSecondsRealtime(0.2f);
		if (PlayerRevive != null)
			PlayerRevive();
	}

	public void OnBossAppear () {
		if (BossAppear != null)
			BossAppear();
	}

	public void OnBossFinishAppear () {
		if (BossFinishAppear != null)
			BossFinishAppear();
	}

	public void OnBossDefeated () {
		if (BossDefeated != null)
			BossDefeated();
	}

	public void OnBossFinishDie () {
		if (BossFinishDie != null)
			BossFinishDie();
	}

	public void OnCircleSpawned (Circle c) {
		if (CircleSpawned != null)
			CircleSpawned(c);
	}

	public void OnCircleExplode (Circle c) {
		if (CircleExplode != null)
			CircleExplode(c);
	}

	public void OnCircleExit (Circle c) {
		if (CircleExit != null)
			CircleExit(c);
	}

	public void OnCircleCollide (Circle c) {
		if (CircleCollide != null)
			CircleCollide(c);
	}

	public void OnPlayerGetCoin (Player p, Coin c) {
		if (PlayerGetCoin != null)
			PlayerGetCoin(p, c);
	}

	public void OnPlayerUseCoin (Player p, int coin) {
		if (PlayerUseCoin != null)
			PlayerUseCoin(p, coin);
	}

	public void OnPlayerGetBonus (Player p, BonusType b, int variant = 0) {
		if (PlayerGetBonus != null)
			PlayerGetBonus(p,b, variant);
	}

	public void OnPlayerGainHealth (Player p, float hpAdded) {
		if (PlayerGainHealth != null)
			PlayerGainHealth(p, hpAdded);
	}

	public void OnPlayerLostHealth (Player p, float hpLost) {
		if (PlayerLostHealth != null)
			PlayerLostHealth(p, hpLost);
	}

	public void OnPlayerHealthChanged (Player p, float newHp) {
		if (PlayerHealthChanged != null)
			PlayerHealthChanged(p, newHp);
	}

	public void OnPlayerDealDamage (Player p, int damage) {
		if (PlayerDealDamage != null)
			PlayerDealDamage(p, damage);
	}

	public void OnPlayerGetScore (Player p, int score) {
		if (PlayerGetScore != null)
			PlayerGetScore(p, score);
	}

	public void OnGamePhaseChanged (GAME_PHASE newPhase) {
		if (GamePhaseChanged != null)
			GamePhaseChanged(newPhase);
	}

	public void OnButtonPressed (string s) {
		if (ButtonPressed != null)
			ButtonPressed(s);
	}

	void OnDestroy () {
		GameStart = null;
		GameEnd = null;
		PlayerDead = null;
		PlayerRevive = null;
		BossAppear = null;
		BossFinishAppear = null;
		BossDefeated = null;
		BossFinishDie = null;
		CircleSpawned = null;
		CircleExplode = null;
		CircleExit = null;
		CircleCollide = null;
		PlayerGetCoin = null;
		PlayerUseCoin = null;
		PlayerGetBonus = null;
		PlayerGainHealth = null;
		PlayerLostHealth = null;
		PlayerHealthChanged = null;
		PlayerDealDamage = null;
		PlayerGetScore = null;
		GamePhaseChanged = null;
		ButtonPressed = null;
	}
}