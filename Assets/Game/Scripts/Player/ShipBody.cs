using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Handle collisions between ship and enemies
/// </summary>
public class ShipBody : MonoBehaviour {

	public Player myShip;

	void OnTriggerEnter2D (Collider2D col) {
		myShip.HandleCollision(col);
	}
}