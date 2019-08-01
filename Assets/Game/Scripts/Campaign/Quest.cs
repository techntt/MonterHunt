using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum QUEST_TYPE {
	PLAY,
	REVIVE,
	COLLIDE,
	COLLIDE_1_GAME,
	COLLIDE_RED,
	KILL,
	KILL_1_GAME,
	KILL_RED,
	KILL_ALL,
	SCORE,
	COIN,
	COIN_1_GAME,
	BONUS,
	BONUS_1_GAME,
	SURVIVE,
	MAX_HP,
	NO_COLLIDE,
	START_HP,
	GET_HP,
	USE_COIN,
	NONE,
}

public class Quest {

	public int id;
	public QUEST_TYPE questType;
	public string questDescription;
	public int value, value2;
	public int currentValue;
	public long duration = 0;
	public int times = 0;
	public Dictionary<string, int> reward;

	float referValue;

	public Quest () {
	}

	public Quest (int id, QUEST_TYPE type, int value, int value2, string reward) {
		this.id = id;
		questType = type;
		this.value = value;
		this.value2 = value2;
		questDescription = GetDescription();
		this.reward = new Dictionary<string, int>();
		ParseReward(reward);
	}

	/// <summary>
	/// Parses the reward.
	/// reward format should be like "100_gold 10_diamond ..."
	/// </summary>
	/// <param name="reward">Reward as string.</param>
	void ParseReward (string reward) {
		string[] s = reward.Split(new char[]{ ' ' });
		for (int i = 0; i < s.Length; i++) {
			string[] x = s[i].Split(new char[]{ '_' });
			this.reward.Add(x[1], int.Parse(x[0]));
		}
	}

	public string GetDescription () {
		switch (questType) {
			case QUEST_TYPE.BONUS:
				return string.Format("Get {0} Bonuses", value);
			case QUEST_TYPE.BONUS_1_GAME:
				return string.Format("Get {0} Bonuses in one game", value);
			case QUEST_TYPE.COIN:
				return string.Format("Collect {0} gold coins", value);
			case QUEST_TYPE.COIN_1_GAME:
				return string.Format("Collect {0} gold coins in one game", value);
			case QUEST_TYPE.COLLIDE:
				return string.Format("Collide with circles {0} times", value);
			case QUEST_TYPE.COLLIDE_1_GAME:
				return string.Format("Collide with circles {0} times in one game", value);
			case QUEST_TYPE.COLLIDE_RED:
				return string.Format("Collide with red circles {0} times", value);
			case QUEST_TYPE.GET_HP:
				return string.Format("Have {0} HP or more", value);
			case QUEST_TYPE.KILL:
				return string.Format("Destroy {0} circles", value);
			case QUEST_TYPE.KILL_1_GAME:
				return string.Format("Destroy {0} circles in one game", value);
			case QUEST_TYPE.KILL_RED:
				return string.Format("Destroy {0} red circles", value);
			case QUEST_TYPE.KILL_ALL:
				return string.Format("Destroy all spawned circles in first {0}s of a game", value);
			case QUEST_TYPE.MAX_HP:
				return string.Format("Reach your maximum HP {0} times in game", value);
			case QUEST_TYPE.NO_COLLIDE:
				return string.Format("Take no damage in first {0}s of a game", value);
			case QUEST_TYPE.PLAY:
				return string.Format("Play {0} games", value);
			case QUEST_TYPE.REVIVE:
				return string.Format("Revive {0} times", value);
			case QUEST_TYPE.SCORE:
				return string.Format("Get {0} scores", value);
			case QUEST_TYPE.START_HP:
				return string.Format("Start a game with {0} HP or more", value);
			case QUEST_TYPE.SURVIVE:
				return string.Format("Survive in first {0}s of a game", value);
			case QUEST_TYPE.USE_COIN:
				return string.Format("Spend {0} gold", value);
			default:
				return "Unidentified quest";
		}
	}

	public void TrackQuest () {
		switch (questType) {
			case QUEST_TYPE.BONUS:
				if (PopupManager.Instance.scene == SCENE.GAME)
					GameEventManager.Instance.PlayerGetBonus += HandlePlayerGetBonus;
				break;
			case QUEST_TYPE.BONUS_1_GAME:
				if (PopupManager.Instance.scene == SCENE.GAME)
					GameEventManager.Instance.PlayerGetBonus += HandlePlayerGetBonus;
				break;
			case QUEST_TYPE.COIN:
				if (PopupManager.Instance.scene == SCENE.GAME)
					GameEventManager.Instance.PlayerGetCoin += HandlePlayerGetCoin;
				break;
			case QUEST_TYPE.COIN_1_GAME:
				if (PopupManager.Instance.scene == SCENE.GAME)
					GameEventManager.Instance.PlayerGetCoin += HandlePlayerGetCoin;
				break;
			case QUEST_TYPE.COLLIDE:
				if (PopupManager.Instance.scene == SCENE.GAME)
					GameEventManager.Instance.CircleCollide += HandlePlayerCollideWithCircle;
				break;
			case QUEST_TYPE.COLLIDE_1_GAME:
				if (PopupManager.Instance.scene == SCENE.GAME)
					GameEventManager.Instance.CircleCollide += HandlePlayerCollideWithCircle;
				break;
			case QUEST_TYPE.COLLIDE_RED:
				if (PopupManager.Instance.scene == SCENE.GAME)
					referValue = CircleSpawner.Instance.h4;
				GameEventManager.Instance.CircleCollide += HandlePlayerCollideRedCircle;
				break;
			case QUEST_TYPE.GET_HP:
				if (PopupManager.Instance.scene == SCENE.GAME)
					GameEventManager.Instance.PlayerGainHealth += HandlePlayerGainHealth;
				break;
			case QUEST_TYPE.KILL:
				if (PopupManager.Instance.scene == SCENE.GAME)
					GameEventManager.Instance.CircleExplode += HandlePlayerKillCircle;
				break;
			case QUEST_TYPE.KILL_1_GAME:
				if (PopupManager.Instance.scene == SCENE.GAME)
					GameEventManager.Instance.CircleExplode += HandlePlayerKillCircle;
				break;
			case QUEST_TYPE.KILL_RED:
				if (PopupManager.Instance.scene == SCENE.GAME)
					referValue = CircleSpawner.Instance.h4;
				GameEventManager.Instance.CircleExplode += HandlePlayerKillRedCirle;
				break;
			case QUEST_TYPE.KILL_ALL:
				if (PopupManager.Instance.scene == SCENE.GAME) {
					// when game starts, invoke a countdown function.
					// when the time ends, quest is completed.
					// while countdowning, if a circle exits, the quest is failed.
					GameManager.Instance.OnTimeChange += KillAllHandleOnTimeChange;
					GameEventManager.Instance.CircleExit += KillAllHandleCircleExit;
				}
				break;
			case QUEST_TYPE.MAX_HP:
				if (PopupManager.Instance.scene == SCENE.GAME) {
					
					GameEventManager.Instance.PlayerGainHealth += CheckIfPlayerGetMaxHealth;
				}
				break;
			case QUEST_TYPE.NO_COLLIDE:
				if (PopupManager.Instance.scene == SCENE.GAME) {
					GameManager.Instance.OnTimeChange += NoCollideHandleTimeChange;
					GameEventManager.Instance.CircleCollide += PlayerFailNoCollideQuest;
				}
				break;
			case QUEST_TYPE.PLAY:
				if (PopupManager.Instance.scene == SCENE.GAME)
					GameEventManager.Instance.GameStart += HandleGameStart;
				break;
			case QUEST_TYPE.REVIVE:
				if (PopupManager.Instance.scene == SCENE.GAME) {
					GameEventManager.Instance.PlayerRevive += HandlePlayerRevive;
				}
				break;
			case QUEST_TYPE.SCORE:
				if (PopupManager.Instance.scene == SCENE.GAME)
					GameEventManager.Instance.GameEnd += ScoreHandleGameEnd;
				break;
			case QUEST_TYPE.START_HP:
				if (PopupManager.Instance.scene == SCENE.GAME)
					GameEventManager.Instance.GameStart += CheckPlayerHpOnGameStart;
				break;
			case QUEST_TYPE.SURVIVE:
				if (PopupManager.Instance.scene == SCENE.GAME) {
					GameEventManager.Instance.PlayerDead += PlayerCantSurvive;
					GameManager.Instance.OnTimeChange += SurviveHandleTimeChange;
				}
				break;
			case QUEST_TYPE.USE_COIN:
				if (PopupManager.Instance.scene == SCENE.UPGRADE) {
					GameEventManager.Instance.PlayerUseCoin += HandlePlayerSpendCoin;
				}
				break;
		}
		if (PopupManager.Instance.scene == SCENE.GAME)
			GameEventManager.Instance.GameEnd += HandleGameEnd;
	}


	public void HandleGameEnd () {
		this.duration += GameManager.Instance.timePlay;
		this.times++;
	}

	void HandlePlayerSpendCoin (Player p, int c) {
		currentValue += c;
	}

	void PlayerCantSurvive () {
		if (currentValue < value) {
			GameManager.Instance.OnTimeChange -= SurviveHandleTimeChange;
			GameEventManager.Instance.PlayerDead -= PlayerCantSurvive;
		}
	}

	void SurviveHandleTimeChange (int time) {
		currentValue = time;
		if (currentValue >= value) {
			GameManager.Instance.OnTimeChange -= SurviveHandleTimeChange;
			GameEventManager.Instance.PlayerDead -= PlayerCantSurvive;
		}
	}

	void CheckPlayerHpOnGameStart () {
		if (GameManager.Instance.player1.health >= value)
			currentValue = GameManager.Instance.player1.health;
	}

	void ScoreHandleGameEnd () {
		currentValue += GameManager.Instance.score;
	}

	void HandlePlayerRevive () {
		currentValue++;
	}

	void HandleGameStart () {
		currentValue++;
	}

	void PlayerFailNoCollideQuest (Circle c) {
		if (currentValue < value) {
			GameManager.Instance.OnTimeChange -= NoCollideHandleTimeChange;
			GameEventManager.Instance.CircleCollide -= PlayerFailNoCollideQuest;
		}
	}

	void NoCollideHandleTimeChange (int time) {
		currentValue = time;
		if (currentValue >= value) {
			GameManager.Instance.OnTimeChange -= NoCollideHandleTimeChange;
			GameEventManager.Instance.CircleCollide -= PlayerFailNoCollideQuest;
		}
	}

	void CheckIfPlayerGetMaxHealth (Player p, float hpChange) {
		if (p.health == referValue)
			currentValue++;
	}

	void KillAllHandleCircleExit (Circle c) {
		if (currentValue < value) {
			GameManager.Instance.OnTimeChange -= KillAllHandleOnTimeChange;
			GameEventManager.Instance.CircleExit -= KillAllHandleCircleExit;
		}
	}

	void KillAllHandleOnTimeChange (int time) {
		currentValue = time;
		if (currentValue >= value) {
			GameManager.Instance.OnTimeChange -= KillAllHandleOnTimeChange;
			GameEventManager.Instance.CircleExit -= KillAllHandleCircleExit;
		}
	}

	void HandlePlayerCollideRedCircle (Circle c) {
		if (c.maxHp > referValue)
			currentValue++;
	}

	void HandlePlayerKillRedCirle (Circle c) {
		if (c.maxHp > referValue)
			currentValue++;
	}

	void HandlePlayerKillCircle (Circle c) {
		currentValue++;
	}

	void HandlePlayerGainHealth (Player p, float hpChange) {
		if (p.health >= value) {
			currentValue = value;
		}
	}

	void HandlePlayerCollideWithCircle (Circle c) {
		currentValue++;
	}

	void HandlePlayerGetCoin (Player p, Coin c) {
		if (c.isGold)
			currentValue++;
		else
			currentValue += 10;
	}

	void HandlePlayerGetBonus (Player p, BonusType b, int variant) {
		currentValue++;
	}
}