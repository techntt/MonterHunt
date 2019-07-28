using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleSpawner : SingletonMonoBehaviour<CircleSpawner> {

	public Transform wallLeft, wallRight;

	// fixed parameters
	public int maxHP;
	public int h1, h2, h3, h4;
	bool isEarlyGame = true;
	public float h1Chance, h2Chance, h5Chance, h4Chance;
	SortedList<int, HpChance> hpChances;
	public float minSpeed = 1, maxSpeed = 4;
	public float minDelayTime = 1, maxDelayTime = 0.3f;

	public const float verySmallSize = 0.5f;
	public const float veryLargeSize = 2;
	public const int specialSizeChance = 5;
	// test
	public const float minSize = 0.5f, maxSize = 1.6f;
	public float bonusDropChance = 15;
	public int specialCircleChance = 30;
	public int minHardenChance, maxHardenChance;

	public float estimatedTime = 180;
	public float maxBonusSpeed;

	// parameters changed by time
	public float bonusSpeed = 0;
	public float spawnDelayTime;
	public int hardenChance;

	public int[] phase1;
	public int[] phase2;
	public int[] phase3;
	int[] orbitChance;

	public int activeCircles;
	int minCircles = 3;
	public ParticleSystem cloudParticle, starParticle;

	Vector3[] spawnPos;

	BaseFormation bf1, bf2;
	BaseBoss boss;
    

	void Awake () {
		ReadDifficultyData();
		hardenChance = minHardenChance;
		phase1 = CampaignManager.campaign.phase1;
		phase2 = CampaignManager.campaign.phase2;
		phase3 = CampaignManager.campaign.phase3;
		spawnPos = new Vector3[] {
			new Vector3(wallLeft.position.x - 1, 0), //0
			new Vector3(wallLeft.position.x - 1, 1), //1
			new Vector3(wallLeft.position.x - 1, 2), //2
			new Vector3(wallLeft.position.x - 1, 3), //3
			new Vector3(wallLeft.position.x - 1, 4), //4
			new Vector3(wallLeft.position.x - 1, 5), //5
			new Vector3(wallLeft.position.x - 1, 6), //6
			new Vector3(wallLeft.position.x + 1, 6), //7
			new Vector3(wallLeft.position.x + 2, 6), //8
			new Vector3(0, 6), 					  //9
			new Vector3(wallRight.position.x - 2, 6), //10
			new Vector3(wallRight.position.x - 1, 6), //11
			new Vector3(wallRight.position.x + 1, 6), //12
			new Vector3(wallRight.position.x + 1, 5), //13
			new Vector3(wallRight.position.x + 1, 4), //14
			new Vector3(wallRight.position.x + 1, 3), //15
			new Vector3(wallRight.position.x + 1, 2), //16
			new Vector3(wallRight.position.x + 1, 1), //17
			new Vector3(wallRight.position.x + 1, 0), //18
		};
		GameEventManager.Instance.GameStart += HandleOnGameStart;
		GameEventManager.Instance.GameEnd += OnGameEnd;
		GameEventManager.Instance.GamePhaseChanged += HandleGamePhaseChanged;
		GameEventManager.Instance.CircleExit += HandleCircleExit;
		GameEventManager.Instance.CircleExplode += HandleCircleExplode;
		GameManager.Instance.OnTimeChange += IncreaseDifficultyByTime;
		// spawn formations and boss
		GameObject go = Instantiate(Resources.Load(Const.FORMATION + CampaignManager.campaign.form1ID)) as GameObject;
		bf1 = (BaseFormation)go.GetComponent(typeof(BaseFormation));
		go = Instantiate(Resources.Load(Const.FORMATION + CampaignManager.campaign.form2ID)) as GameObject;
		bf2 = (BaseFormation)go.GetComponent(typeof(BaseFormation));
		go = Instantiate(Resources.Load(Const.BOSS + CampaignManager.campaign.bossID)) as GameObject;
		boss = (BaseBoss)go.GetComponent(typeof(BaseBoss));
		boss.maxHp = CampaignManager.campaign.bossHp;
        
	}

	void HandleOnGameStart () {
		if (CampaignManager.campaign.id == 0)
			Invoke("EndEarlyGame", 20);
		else
			Invoke("EndEarlyGame", 10);
	}

	void EndEarlyGame () {
		isEarlyGame = false;
	}

	void HandleCircleExplode (Circle c) {
		activeCircles--;
		if (GameManager.Instance.phase == GAME_PHASE.PHASE1 || GameManager.Instance.phase == GAME_PHASE.PHASE2 ||
		    GameManager.Instance.phase == GAME_PHASE.PHASE3) {
			if (activeCircles < minCircles)
				Invoke("SpawnAdditionalCircle", 0.1f);
		}
	}

	void HandleCircleExit (Circle c) {
		activeCircles--;
		if (GameManager.Instance.phase == GAME_PHASE.PHASE1 || GameManager.Instance.phase == GAME_PHASE.PHASE2 ||
		    GameManager.Instance.phase == GAME_PHASE.PHASE3) {
			if (activeCircles < minCircles)
				Invoke("SpawnAdditionalCircle", 0.1f);
		}
	}

	void ReadDifficultyData () {
		hpChances = new SortedList<int, HpChance>();
		List<Dictionary<string, string>> data = CSVReader.ReadDataToList(DataManager.Instance.difficulty[CampaignManager.campaign.id]);
		maxHP = int.Parse(data[0]["maxHp"]);
		h1 = maxHP / 5;
		h4 = maxHP - h1;
		h3 = h4 - h1;
		h2 = h3 - h1;
		minSpeed = float.Parse(data[0]["minSpeed"]);
		maxSpeed = float.Parse(data[0]["maxSpeed"]);
		minDelayTime = float.Parse(data[0]["minDelayTime"]);
		maxDelayTime = float.Parse(data[0]["maxDelayTime"]);
		bonusDropChance = float.Parse(data[0]["bonusDropChance"]);
		specialCircleChance = int.Parse(data[0]["specialCircleChance"]);
		estimatedTime = float.Parse(data[0]["estimatedTime"]);
		maxBonusSpeed = float.Parse(data[0]["bonusSpeed"]);
		minHardenChance = int.Parse(data[0]["minHardenChance"]);
		maxHardenChance = int.Parse(data[0]["maxHardenChance"]);
		for (int i = 0; i < data.Count; i++) {
			HpChance h = new HpChance(float.Parse(data[i]["h1"]), float.Parse(data[i]["h2"]), float.Parse(data[i]["h4"]), float.Parse(data[i]["h5"]));
			hpChances.Add(int.Parse(data[i]["time"]), h);
		}
		h1Chance = hpChances[0].h1;
		h2Chance = hpChances[0].h2;
		h4Chance = hpChances[0].h4;
		h5Chance = hpChances[0].h5;
	}
    

	void HandleGamePhaseChanged (GAME_PHASE phase) {
		switch (phase) {
			case GAME_PHASE.PHASE1:
				orbitChance = phase1;
				StartCoroutine("SpawnCircles");
				break;
			case GAME_PHASE.PHASE2:
				orbitChance = phase2;
				StartCoroutine("SpawnCircles");
				break;
			case GAME_PHASE.PHASE3:
				orbitChance = phase3;
				StartCoroutine("SpawnCircles");
				break;
			case GAME_PHASE.FORM1:
				StopCoroutine("SpawnCircles");
				bf1.gameObject.SetActive(true);
				bf1.Init();
				break;
			case GAME_PHASE.FORM2:
				StopCoroutine("SpawnCircles");
				bf2.gameObject.SetActive(true);
				bf2.Init();
				break;
			case GAME_PHASE.BOSS:
				StopCoroutine("SpawnCircles");
				StartCoroutine("BossAppear");
				break;
		}
	}

	void OnGameEnd () {
		StopCoroutine("SpawnCircles");
	}

	IEnumerator SpawnCircles () {
		SpawnCircle();
		yield return new WaitForSeconds(spawnDelayTime);
		StartCoroutine("SpawnCircles");
	}

	public void SpawnCircle () {
		// calculate orbit
		CircleOrbit orbit = GetRandomOrbit();
		// calculate hp
		int hp = GetRandomHP();
		// calculate size
		float size = GetRandomSize();
		// calculate speed
		float speed = GetRandomSpeed(orbit);
		// calculate start pos
		Vector3 pos = GetRandomPosition(orbit);
		CircleType type = GetRandomCircleType();
		Circle cir = CircleManager.Instance.PopCircle(type, size, pos);
		cir.Init(hp, orbit, speed, true);
		activeCircles++;
	}

	public void SpawnAdditionalCircle () {
		// calculate orbit
		CircleOrbit orbit = GetRandomOrbit();
		// calculate hp
		int hp = GetRandomHP();
		// calculate size
		float size = GetRandomSize();
		// calculate speed
		float speed = GetRandomSpeed(orbit);
		// calculate start pos
		Vector3 pos = GetRandomPosition(orbit);
		CircleType type = CircleType.NORMAL;
		Circle cir = CircleManager.Instance.PopCircle(type, size, pos);
		cir.Init(hp, orbit, speed, true, true, true);
		activeCircles++;
	}

	IEnumerator BossAppear () {
		yield return new WaitForSeconds(3);
		boss.gameObject.SetActive(true);
	}

	public CircleType GetRandomCircleType () {
		int c = Random.Range(0, 100);
		if (c < hardenChance) {
			return CircleType.HARDEN;
		} else {
			if (Random.Range(0, 100) < specialCircleChance) {
				return (CircleType)Random.Range(0, 2);
			} else
				return CircleType.NORMAL;
		}
	}

	public CircleOrbit GetRandomOrbit () {
		CircleOrbit orbit;
		int c = Random.Range(0, 100);
		if (c < orbitChance[0])
			orbit = CircleOrbit.D;
		else if (c < orbitChance[0] + orbitChance[1]) {
			if (Random.Range(0, 100) < 50)
				orbit = CircleOrbit.L;
			else
				orbit = CircleOrbit.R;
		} else {
			if (Random.Range(0, 100) < 50)
				orbit = CircleOrbit.ZL;
			else
				orbit = CircleOrbit.ZR;
		}
		return orbit;
	}

	public float GetRandomSize () {
		float size = 0;
		if (Random.Range(0, 100) < specialSizeChance) {
			size = veryLargeSize;
		} else {
			size = Random.Range(minSize, maxSize);
		}
		return size;
	}

	public int GetRandomHP () {
		if (isEarlyGame) {
			return Random.Range(1, h1 / 2 + 1);
		} else {
			int c = Random.Range(0, 100);
			if (c < h1Chance) {
				return Random.Range(1, h1 + 1);
			} else if (c < h1Chance + h5Chance) {
				return Random.Range(h4, maxHP + 1);
			} else if (c < h1Chance + h5Chance + h2Chance) {
				return Random.Range(h1, h2 + 1);
			} else if (c < h1Chance + h5Chance + h2Chance + h4Chance) {
				return Random.Range(h3, h4 + 1);
			} else {
				return Random.Range(h2, h3 + 1);
			}
		}
	}

	public float GetRandomSpeed (CircleOrbit orbit) {
		float speed = Random.Range(minSpeed, maxSpeed) + bonusSpeed;
		if (orbit == CircleOrbit.L || orbit == CircleOrbit.R)
			speed /= 1.25f;
		else if (orbit == CircleOrbit.ZL || orbit == CircleOrbit.ZR)
			speed /= 1.5f;
		return speed;
	}

	public Vector3 GetRandomPosition (CircleOrbit orbit) {
		Vector3 pos = Vector3.zero;
		switch (orbit) {
			case CircleOrbit.D:
			case CircleOrbit.ZL:
			case CircleOrbit.ZR:
				pos = spawnPos[Random.Range(7, 12)];
				break;
			case CircleOrbit.L:
				pos = spawnPos[Random.Range(0, 9)];
				break;
			case CircleOrbit.R:
				pos = spawnPos[Random.Range(10, 19)];
				break;
		}
		return pos;
	}

	void IncreaseDifficultyByTime (int time) {
		if (hpChances.Count > 0 && time == hpChances.Keys[0]) {
			HpChance h = hpChances[time];
			h1Chance = h.h1;
			h2Chance = h.h2;
			h4Chance = h.h4;
			h5Chance = h.h5;
			hpChances.RemoveAt(0);
		}

        if (GameManager.Instance.phase == GAME_PHASE.PHASE1 ||
            GameManager.Instance.phase == GAME_PHASE.PHASE2 ||
            GameManager.Instance.phase == GAME_PHASE.PHASE3)
        {
            // change speed
            bonusSpeed = GameManager.GetLinearValueSimilarTo(0, estimatedTime, 0, 8, time);
            // change spawn time
            spawnDelayTime = GameManager.GetLinearValueSimilarTo(0, estimatedTime, minDelayTime, maxDelayTime, time);
            // change particle speed
            ParticleSystem.MainModule m = cloudParticle.main;
            m.simulationSpeed = GameManager.GetLinearValueSimilarTo(0, estimatedTime, 1, 5, time);
            m = starParticle.main;
            m.simulationSpeed = GameManager.GetLinearValueSimilarTo(0, estimatedTime, 1, 5, time);
            // change spawned rate of harden type
            hardenChance = (int)GameManager.GetLinearValueSimilarTo(0, estimatedTime, minHardenChance, maxHardenChance, time);
        }  
	}

	public static int GetNumberOfCoinBySize (float size) {
		size = Mathf.Clamp(size, minSize, maxSize);
		float c0 = 0, c1 = 0, c2 = 0, c3 = 0, c4 = 0;
		c0 = GameManager.GetLinearValueSimilarTo(minSize, maxSize, 70, 5, size);
		c1 = c0 + GameManager.GetLinearValueSimilarTo(minSize, maxSize, 30, 10, size);
		c2 = c1 + GameManager.GetLinearValueSimilarTo(minSize, maxSize, 0, 15, size);
		c3 = c2 + GameManager.GetLinearValueSimilarTo(minSize, maxSize, 0, 20, size);
		c4 = c3 + GameManager.GetLinearValueSimilarTo(minSize, maxSize, 0, 25, size);
		float c = Random.Range(0, 100);
		if (c < c0)
			return 0;
		else if (c < c1)
			return 1;
		else if (c < c2)
			return 2;
		else if (c < c3)
			return 3;
		else if (c < c4)
			return 4;
		else
			return 5;
	}
}

public class HpChance {
	public float h1, h2, h4, h5;

	public HpChance () {
		h1 = 100;
	}

	public HpChance (float h1, float h2, float h4, float h5) {
		this.h1 = h1;
		this.h2 = h2;
		this.h4 = h4;
		this.h5 = h5;
	}
}