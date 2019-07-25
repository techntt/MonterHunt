using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DailyItemManager : SingletonMonoBehaviour<DailyItemManager> {

	public DailyItem sample;
	public Stack<DailyItem> pool = new Stack<DailyItem>();
	Sprite todayIcon;

	void Start () {
		todayIcon = DailyQuestManager.Instance.todaySprite;
		DailyItem d = PopItem();
		PushItem(d);
	}

	public DailyItem PopItem () {
		if (pool.Count == 0) {
			DailyItem c = Instantiate(sample) as DailyItem;
			c.transform.parent = transform;
			c.myRender.sprite = todayIcon;
			return c;
		} else {
			DailyItem c = pool.Pop();
			c.gameObject.SetActive(true);
			return c;
		}
	}

	public void PushItem (DailyItem c) {
		c.gameObject.SetActive(false);
		pool.Push(c);
	}
}