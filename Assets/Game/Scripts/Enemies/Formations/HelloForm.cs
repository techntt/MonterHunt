using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelloForm : BaseFormation {

	public float speed;

	public override void Init () {
		base.Init();
		for (int i = 0; i < members.Count; i++) {
			members[i].maxHp = CircleSpawner.Instance.maxHP;
			members[i].hp = Random.Range(1, CircleSpawner.Instance.h2 + 1);
			members[i].Init();
		}
	}

	void Update () {
		transform.localPosition += Vector3.down * Time.deltaTime * speed;
	}
}