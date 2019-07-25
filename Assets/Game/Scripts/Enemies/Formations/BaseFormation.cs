using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseFormation : MonoBehaviour {

	public int id;
	public List<DummyObject> members;
	int inactiveCount;

	public virtual void Init () {
		inactiveCount = 0;
		for (int i = 0; i < members.Count; i++) {
			members[i].ObjectInactive += OnObjectInactive;
		}
	}

	protected virtual void OnObjectInactive (DummyObject d) {
		d.ObjectInactive -= OnObjectInactive;
		members.Remove(d);
		ColliderRef.Instance.DamageableRef.Remove(d.myCollider.GetInstanceID());
		Destroy(d.gameObject);
		if (members.Count == 0) {
			GameManager.Instance.NextPhase();
			Destroy(gameObject);
		}
//		inactiveCount++;
//		if (inactiveCount == members.Count) {
//			GameManager.Instance.NextPhase();
//			for (int i = 0; i < members.Count; i++) {
//				members[i].ObjectInactive -= OnObjectInactive;
//				ColliderRef.Instance.DamageableRef.Remove(members[i].myCollider.GetInstanceID());
//			}
//			Destroy(gameObject);
//		}
	}
}