using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : SingletonMonoBehaviour<TutorialManager> {

	public List<PointerData> pData;

	/// <summary>
	/// guide player press start button
	/// </summary>
	public void PointToStartButton () {
		TutorialPopup.Instance.Init(pData[0], true);
		HomeManager.Instance.PlayBtn.SetAsLastSibling();
	}

	/// <summary>
	/// guide player control the ship
	/// </summary>
	public void SwipeToPlay () {
		TutorialPopup.Instance.Init(pData[1], true);
		TutorialPopup.Instance.background.enabled = false;
		TutorialPopup.Instance.myAnim.Play("swipe");
	}

	/// <summary>
	/// guide player get quest reward
	/// </summary>
	public void CheckQuest () {
		TutorialPopup.Instance.Init(pData[2], true);
		HomeManager.Instance.questBtn.SetAsLastSibling();
	}

	/// <summary>
	/// Guide player upgrade the ship
	/// </summary>
	public void CheckUpgrade () {
		TutorialPopup.Instance.Init(pData[3], true);
		HomeManager.Instance.upgradeBtn.SetAsLastSibling();
	}

	/// <summary>
	/// Point to upgrade panel
	/// </summary>
	public void UpgradeShip () {
		TutorialPopup.Instance.Init(pData[4], true);
	}
}

[System.Serializable]
public class PointerData {
	public TUT_DIRECTION direction;
	public Vector3 position;
}