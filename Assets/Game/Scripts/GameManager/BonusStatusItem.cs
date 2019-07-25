using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BonusStatusItem : MonoBehaviour {

	public Image icon;
	public Image cooldown;
	public Animator myAnim;
	public BonusStatusBar myBar;
	public BonusType currentBonus;

	public void Init (BonusType t, float time) {
		gameObject.SetActive(true);
		currentBonus = t;
		icon.sprite = BonusManager.Instance.bonusSprite[(int)t];
		myAnim.speed = 1 / time;
		myAnim.SetTrigger(AnimConst.idle);
		StopCoroutine("Push");
		StartCoroutine("Push", time);
	}

	IEnumerator Push(float time) {
		yield return new WaitForSeconds(time);
		gameObject.SetActive(false);
		myBar.PushItem(this);
	}

	public void ForcePush () {
		StopCoroutine("Push");
		myBar.pool.Push(this);
	}
}