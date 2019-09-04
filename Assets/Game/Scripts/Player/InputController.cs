using UnityEngine;

public class InputController : MonoBehaviour {

	public float followSpeed = 4;
	bool isConnected;
	public Rect screenBound;
	Vector2 dis;

	void Start () {
		screenBound.x = Camera189.gameView.xMin;
		screenBound.width = screenBound.x * -2;
	}

	void Update () {
		#if UNITY_EDITOR || UNITY_STANDALONE_WIN
		if (PlayerSettingData.Instance.controlStyle == CONTROL_STYLE.FIXED) {
			if (Input.GetMouseButtonDown(0)) {
				dis = transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
			} else if (Input.GetMouseButton(0)) {
				Vector2 temp = dis + (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
				temp.x = Mathf.Clamp(temp.x, screenBound.xMin, screenBound.xMax);
				temp.y = Mathf.Clamp(temp.y, screenBound.yMin, screenBound.yMax);
				transform.position = temp;
			}
		} else if (PlayerSettingData.Instance.controlStyle == CONTROL_STYLE.FOLLOW) {
			if (Input.GetMouseButton(0)) {
				if (!isConnected) {
					Vector2 temp = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) + Vector2.up * 0.5f;
					float delta = followSpeed * Time.deltaTime;
					temp.x = Mathf.Clamp(temp.x, screenBound.xMin, screenBound.xMax);
					temp.y = Mathf.Clamp(temp.y, screenBound.yMin, screenBound.yMax);
					transform.position = Vector2.MoveTowards(transform.position, temp, delta);
					if (Vector2.SqrMagnitude((Vector2)transform.position - temp) < delta * delta) {
						isConnected = true;
					}
				} else {
					Vector2 temp = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition) + Vector2.up * 0.5f;
					temp.x = Mathf.Clamp(temp.x, screenBound.xMin, screenBound.xMax);
					temp.y = Mathf.Clamp(temp.y, screenBound.yMin, screenBound.yMax);
					transform.position = temp;
				}
			} else if (Input.GetMouseButtonUp(0)) {
				isConnected = false;
			}
		}
		#elif UNITY_ANDROID
		if (PlayerSettingData.Instance.controlStyle == CONTROL_STYLE.FIXED) {
			if (Input.touchCount > 0) {
				if (Input.touches[0].phase == TouchPhase.Began) {
					dis = transform.position - Camera.main.ScreenToWorldPoint(Input.touches[0].position);
				} else {
					Vector2 temp = dis + (Vector2)Camera.main.ScreenToWorldPoint(Input.touches[0].position);
					temp.x = Mathf.Clamp(temp.x, screenBound.xMin, screenBound.xMax);
					temp.y = Mathf.Clamp(temp.y, screenBound.yMin, screenBound.yMax);
					transform.position = temp;
				}
			}
		} else if (PlayerSettingData.Instance.controlStyle == CONTROL_STYLE.FOLLOW) {
			if (Input.touchCount > 0) {
				if (!isConnected) {
					Vector2 temp = (Vector2)Camera.main.ScreenToWorldPoint(Input.touches[0].position) + Vector2.up * 0.5f;
					float delta = followSpeed * Time.deltaTime;
					temp.x = Mathf.Clamp(temp.x, screenBound.xMin, screenBound.xMax);
					temp.y = Mathf.Clamp(temp.y, screenBound.yMin, screenBound.yMax);
					transform.position = Vector2.MoveTowards(transform.position, temp, delta);
					if (Vector2.SqrMagnitude((Vector2)transform.position - temp) < delta * delta) {
						isConnected = true;
					}
				} else {
					Vector2 temp = (Vector2)Camera.main.ScreenToWorldPoint(Input.touches[0].position) + Vector2.up * 0.5f;
					temp.x = Mathf.Clamp(temp.x, screenBound.xMin, screenBound.xMax);
					temp.y = Mathf.Clamp(temp.y, screenBound.yMin, screenBound.yMax);
					transform.position = temp;
				}
			} else {
				isConnected = false;
			}
		}
		#endif
	}
}