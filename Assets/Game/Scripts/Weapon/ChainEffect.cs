using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainEffect : MonoBehaviour {
	
	public SpriteRenderer myRender;
	public LineRenderer myLine;
	public BaseEnemy target;
	Transform player;
	Transform circle;

	public void Init (BaseEnemy target) {
		this.target = target;
		player = GameManager.Instance.player.firePos;
		circle = target.transform;
		myRender.size =  new Vector2(1.4f,1.4f);
		myRender.color = Color.magenta;
		myRender.transform.position = circle.position;
		myLine.sortingOrder = -1;
		myLine.startColor = GameManager.Instance.player.myRender.color;
		myLine.endColor = Color.red;
		myLine.SetPosition(0, Vector3.zero);
		myLine.SetPosition(1, Vector3.zero);
		target.mColl.enabled = false;
		target.mRigi.velocity = Vector2.zero;
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
			target.mColl.enabled = true;
			ExplodeEffect e = (ExplodeEffect)EffectManager.Instance.SpawnEffect(EFFECT_TYPE.EXPLODE, circle.position);
			e.Init(5, WeaponDataCalculator.LIGHTNING_DAMAGE * GameManager.Instance.player.baseDamage);
			target = null;
		}
	}
}