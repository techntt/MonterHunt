using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainEffect : MonoBehaviour {
	
	public SpriteRenderer myRender;
	public LineRenderer myLine;
	public Circle target;
	Transform player;
	Transform circle;

	public void Init (Circle target) {
		this.target = target;
		player = GameManager.Instance.player1.firePos;
		circle = target.transform;
		myRender.size = target.myRender.size * 1.4f;
		myRender.color = target.myRender.color;
		myRender.transform.position = circle.position;
		myLine.sortingOrder = -1;
		myLine.startColor = GameManager.Instance.player1.myRender.color;
		myLine.endColor = target.myRender.color;
		myLine.SetPosition(0, Vector3.zero);
		myLine.SetPosition(1, Vector3.zero);
		target.myCollider.enabled = false;
		target.myBody.velocity = Vector2.zero;
		gameObject.SetActive(true);
	}

	public void Deactivate () {
		gameObject.SetActive(false);
	}

	void Update () {
		myLine.SetPosition(0, player.position);
		myLine.SetPosition(1, circle.position);
		myRender.transform.position = circle.position;
	}

	void OnDisable () {
		if (target != null) {
			target.Die();
			target.myCollider.enabled = true;
			ExplodeEffect e = (ExplodeEffect)EffectManager.Instance.SpawnEffect(EFFECT_TYPE.EXPLODE, circle.position);
			e.Init(5, WeaponDataCalculator.LIGHTNING_DAMAGE * GameManager.Instance.player1.baseDamage);
			target = null;
		}
	}
}