using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialPopup : MonoBehaviour {

	static TutorialPopup instance;

	public static TutorialPopup Instance {
		get { 
			if (instance == null) {
				GameObject go = Instantiate(Resources.Load ("Tutorial/pointer")) as GameObject;
				instance = (TutorialPopup)go.GetComponent(typeof(TutorialPopup));
			}
			return instance;
		}
	}

	public Image background;
	public RectTransform pointer;
	public RectTransform myTransform;
	public Animator myAnim;

	bool destroyOnTap;

	public void Init (TUT_DIRECTION direction, Vector3 position) {
		myTransform.SetParent(GameObject.FindGameObjectWithTag("MainCanvas").transform, false);
		myTransform.localScale = new Vector3(1, 1, 1);
		myTransform.SetAsLastSibling();
		switch (direction) {
			case TUT_DIRECTION.DOWN:
				pointer.localRotation = Quaternion.Euler(0, 0, -90);
				break;
			case TUT_DIRECTION.LEFT:
				//pointer.localRotation = Quaternion.Euler(0, 0, 0);
				pointer.localScale = new Vector3 (-1, 1, 1);
				break;
//			case TUT_DIRECTION.RIGHT:
//				pointer.localRotation = Quaternion.Euler(0, 0, 0);
//				break;
			case TUT_DIRECTION.UP:
				pointer.localRotation = Quaternion.Euler(0, 0, 90);
				break;
		}
		pointer.localPosition = position;
	}

	public void Init (PointerData p, bool destroyOnTap = false) {
		Init(p.direction, p.position);
		this.destroyOnTap = destroyOnTap;
	}

	void Update () {
		if (destroyOnTap && Input.GetMouseButtonDown(0))
			Destroy();
	}

	public void Destroy () {
		instance = null;
		Destroy(gameObject);
	}
}

public enum TUT_DIRECTION {
	DOWN,
	UP,
	LEFT,
	RIGHT,
	NONE
}