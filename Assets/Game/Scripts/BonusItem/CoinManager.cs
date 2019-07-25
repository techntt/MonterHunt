using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinManager : SingletonMonoBehaviour<CoinManager> {

	public Coin sample;
	public Stack<Coin> pool = new Stack<Coin>();

	void Start () {
		List<Coin> coins = new List<Coin>();
		for (int i = 0; i < 30; i++)
			coins.Add(PopCoin());
		for (int i = 0; i < 30; i++)
			PushCoin(coins[i]);
	}

	public Coin PopCoin () {
		if (pool.Count == 0) {
			Coin c = Instantiate(sample) as Coin;
			c.transform.parent = transform;
			return c;
		} else {
			Coin c = pool.Pop();
			c.gameObject.SetActive(true);
			return c;
		}
	}

	public void PushCoin (Coin c) {
		c.gameObject.SetActive(false);
		pool.Push(c);
	}
}