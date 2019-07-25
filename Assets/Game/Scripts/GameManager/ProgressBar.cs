using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour {

	public RectTransform bar;
	public RectTransform ship;
	public Image shipIcon;
	public Image progImage;
	public Text progText;

	string checkScore = "CheckScore";

	void Start () {
		InvokeRepeating(checkScore, 3, 1);
	}

	void OnDisable () {
		CancelInvoke(checkScore);
	}

	void CheckScore () {
		int score = GameManager.Instance.score;
		int objective = CampaignManager.campaign.objective;
		float percent = (float)score / objective;
		progImage.fillAmount = percent;
		float des = GameManager.GetLinearValueSimilarTo(0, objective, 0, bar.rect.width, score);
		ship.localPosition = new Vector3(des, 0);
		progText.text = string.Format("{0}%", (int)(percent*100));
	}
}
