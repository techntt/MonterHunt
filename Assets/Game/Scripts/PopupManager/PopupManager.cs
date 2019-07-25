using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using Facebook.Unity;

public class PopupManager : MonoBehaviour {

	public static PopupManager Instance;

	public SCENE scene;

	void Awake () {
		if (Instance == null) {
			Instance = this;
			DontDestroyOnLoad(gameObject);
		} else
			Destroy(gameObject);
        // facebook initialize
        //if (FB.IsInitialized)
        //    FB.ActivateApp();
        //else
        //    FB.Init(() =>
        //    {
        //        FB.ActivateApp();
        //    });
    }

	void OnApplicationPause (bool pauseStatus) {
		if (!pauseStatus) {
			//if (FB.IsInitialized) {
			//	FB.ActivateApp();
			//} else {
			//	FB.Init( () => {
			//		FB.ActivateApp();
			//	});
			//}
		}
	}
}

public enum SCENE {
	HOME,
	UPGRADE,
	GAME,
	SHOP,
	NONE
}