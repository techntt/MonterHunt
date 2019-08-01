using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipContainer : SingletonMonoBehaviour<ShipContainer> {
	public Player[] ship;
	Player currentShip;

	public void ShowShip (int id) {
		if (currentShip != null)
			currentShip.gameObject.SetActive(false);
		currentShip = ship[id];
		currentShip.gameObject.SetActive(true);
	}
}