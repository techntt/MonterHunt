using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScene : MonoBehaviour {

	public int mission;

	void Awake () {
		PlayerData.Instance.currentMission = mission;
		CampaignManager.ReadData();
		QuestManager.InitQuest();
	}

	void Start () {
		GameManager.Instance.player1.body.enabled = false;
		GameManager.Instance.player1.myBaseWeapon.enabled = false;
	}
}