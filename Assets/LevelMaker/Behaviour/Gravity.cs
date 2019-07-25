using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gravity : MonoBehaviour {

	#region Inspector Variables;

	public int numberChild = 1;
	public float radius = 1f;

	#endregion;

	#region Member Variables

	private SpecialObject spObj;
	private List<Circle> childs;
	[HideInInspector] public int currChilds;
	private bool isActive = false;

	#endregion;

	#region Unity Methods

	private void Awake () {
		spObj = gameObject.GetComponent(typeof(SpecialObject)) as SpecialObject;
		spObj.type = SpecialObject.SPOBJ_TYPE.GRAVITY;
		spObj.OnChildInit += Init;
		spObj.OnDetachChild += DetachParrent;
		childs = new List<Circle>();
		currChilds = 0;
	}

	private void Update () {
		if (!isActive && spObj.trans.position.y < 2.5f)
			ActiveGravity();
	}

	#endregion;

	#region Public Methods

	public bool canAdd () {
		if (currChilds >= numberChild) {
			spObj.gravityCollider.gameObject.SetActive(false);
			return false;
		}
		return true;
	}

	public void AddChild (Circle circle) {
		float radius = spObj.myCollider.radius + circle.myCollider.radius;
		var _deltaAngle = (360 / numberChild) * childs.Count * Mathf.Deg2Rad;
		var offset = new Vector2(Mathf.Sin(_deltaAngle), Mathf.Cos(_deltaAngle)) * radius;
		circle.trans.parent = spObj.trans;
		circle.trans.localPosition = offset;
		childs.Add(circle);
	}

	#endregion;

	#region Private Methods

	private void Init () {
        
	}

	private void ActiveGravity () {
		spObj.gravityCollider.gameObject.SetActive(true);
		spObj.gravityCollider.radius = radius;
		spObj.myAmim.SetTrigger("isRotate");
		isActive = true;
	}

	private void DetachParrent () {
		foreach (Circle circle in childs) {
			circle.trans.parent = CircleManager.Instance.transform;
			if (circle.wasInScene)
				circle.myBody.velocity = Vector2.down * circle.speed;
//            circle.isSucked = false;
		}
		childs.Clear();
	}

	#endregion;
}
