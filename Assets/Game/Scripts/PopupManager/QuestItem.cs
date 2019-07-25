using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class QuestItem : MonoBehaviour {

	public int id;
	public bool isCompleted;
	public Text questName;
	public Image questProgress;
	public Text questProgText;
	public Text rankText;
	public Text goldText;
	public Animator myAnim;
	Vector3 originPos;
	public QuestPopupData myManager;

	public void Init () {
		originPos = transform.localPosition;
	}

	public void Init (Quest quest) {
		questName.text = quest.questDescription;
		questProgress.fillAmount = (float)quest.currentValue / quest.value;
		if (quest.currentValue < quest.value) {
			questProgText.text = string.Format("{0}/{1}", quest.currentValue, quest.value);
			isCompleted = false;
		} else {
			questProgText.text = "Completed";
			isCompleted = true;
		}
		//rankText.text = quest.reward["rank"].ToString();
		goldText.text = quest.reward["gold"].ToString();
	}

	public void HandleOtherQuestItemClose (QuestItem q) {
		if (q.id < id) {
			transform.DOLocalMoveY(transform.localPosition.y + 118, 0.5f);
		}
	}

	public void Reset (bool isHide) {
		transform.localPosition = originPos;
		if (!isHide) {
			if (myAnim)
				myAnim.SetTrigger(AnimConst.idle);
		} else {
			if (myAnim)
				myAnim.SetTrigger(AnimConst.die);
		}
	}

	void OnClose () {
		myManager.OnQuestClose(this);
	}

	public void Close () {
		if (myAnim)
			myAnim.SetTrigger(AnimConst.move);
	}

	public void Open () {
		if (myAnim)
			myAnim.SetTrigger(AnimConst.start);
	}

	public void Hide () {
		if (myAnim)
			myAnim.SetTrigger(AnimConst.die);
	}
}