using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using System;

public class NotifyPopup : SingletonMonoBehaviour<NotifyPopup> {

	public Text title;
	public Text content;
	public Text yesBtn;
	public Text noBtn;
	public Button yes;
	public Button no;

	UnityAction noCallback;

	public void Show (string title, string content, string yes, UnityAction YesCallback, string no = "No", UnityAction NoCallback = null) {
		this.title.text = title;
		this.content.text = content;
		this.yesBtn.text = yes;
		this.noBtn.text = no;
		this.yes.onClick.AddListener(YesCallback);
		this.yes.onClick.AddListener(() => {
			SoundManager.Instance.PlayUIButtonClick();
		});
		if (NoCallback != null) {
			this.no.gameObject.SetActive(true);
			noCallback = NoCallback;
//			this.no.onClick.AddListener(NoCallback);
//			this.no.onClick.AddListener(() => {
//				SoundManager.Instance.PlayUIButtonClick();
//			});
		} else {
			this.no.gameObject.SetActive(false);
		}
}

	public void OnNoClick () {
		// Call hide popup
	}
}