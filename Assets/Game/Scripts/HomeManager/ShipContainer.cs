using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipContainer : SingletonMonoBehaviour<ShipContainer> {
	public Player[] ship;
	Player currentShip;

	public void ShowShip (int type) {
		if (currentShip != null)
			currentShip.gameObject.SetActive(false);
		currentShip = ship[(int)type];
		currentShip.gameObject.SetActive(true);
		currentShip.SetColor(GameManager.GetColorByHP(1 + PlayerData.Instance.shipData[type].hpLv, 60));
		currentShip.healthText.text = "" + (1 + PlayerData.Instance.shipData[type].hpLv);
	}
}