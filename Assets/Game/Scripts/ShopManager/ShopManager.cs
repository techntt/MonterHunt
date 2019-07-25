using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Purchasing;

public class ShopManager : SingletonMonoBehaviour<ShopManager> {

	public Text goldText;
	public Text rankText;

	void Start () {
		PopupManager.Instance.scene = SCENE.SHOP;
		PlayerData.Instance.OnGoldChange += HandleOnGoldChanged;
		HandleOnGoldChanged(0);
		rankText.text = PlayerData.Instance.rank.ToString();
	}

	void HandleOnGoldChanged (int gold) {
		goldText.text = PlayerData.Instance.gold.ToString();
	}

	public void Back () {
		SceneManager.LoadScene(Const.SCENE_HOME);
	}

	void Update () {
		if (Input.GetKeyDown(KeyCode.Escape))
			Back();
	}

	public void HandlePurchaseComplete (Product p) {
		int gold = 0;
		if (p.definition.id == "pack1")
			gold = 3000;
		else if (p.definition.id == "pack2")
			gold = 6500;
		else if (p.definition.id == "pack3")
			gold = 19000;
		else if (p.definition.id == "pack4")
			gold = 45000;
		else if (p.definition.id == "pack5")
			gold = 105000;
		else if (p.definition.id == "pack6")
			gold = 300000;
		PlayerData.Instance.gold += gold;
		GlobalEventManager.Instance.OnCurrencyChanged("gold", "earn", gold.ToString());
		PlayerData.Instance.SaveAllData();
	}
}