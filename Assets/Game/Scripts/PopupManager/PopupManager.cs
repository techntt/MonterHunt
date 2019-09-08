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

    public static Color32[] colors = new Color32[] {
        new Color32(0, 255, 255, 255), // cyan
		new Color32(0, 255, 0, 255), // green
		new Color32(255, 255, 0, 255), // yellow
		new Color32(255, 0, 255, 255), // violet
		new Color32(255, 128, 0, 255), // orange
		new Color32(255, 0, 0, 255) //red
	};

    public static Color GetColorByHP(int hp, int maxHP)
    {
        if (hp == 0)
            return Color.white;
        hp = Mathf.Clamp(hp, 1, maxHP);
        float p = maxHP / 5f;
        p = p == 0 ? 1 : p;
        int part = (int)(hp / p);
        float left = hp - p * part;
        if (left == 0 || hp > (p * 5))
            return colors[part];
        float pi = p * part;
        int r = (int)GetLinearValueSimilarTo(pi, pi + p, colors[part].r, colors[part + 1].r, pi + left);
        int g = (int)GetLinearValueSimilarTo(pi, pi + p, colors[part].g, colors[part + 1].g, pi + left);
        int b = (int)GetLinearValueSimilarTo(pi, pi + p, colors[part].b, colors[part + 1].b, pi + left);
        return Color32ToColor(r, g, b);
    }

    public static Color Color32ToColor(int r, int g, int b)
    {
        return new Color(r / 255f, g / 255f, b / 255f);
    }

    public static string BigIntToString(long v)
    {
        if (v < 1000)
            return v.ToString();
        else if (v < 10000)
        {
            float n = v / 1000f;
            return string.Format("{0:N1}K", n);
        }
        else if (v < 1000000)
        {
            float n = v / 1000;
            return string.Format("{0}K", n);
        }
        else if (v < 10000000)
        {
            float n = v / 1000000f;
            return string.Format("{0:N1}M", n);
        }
        else if (v < 1000000000)
        {
            float n = v / 1000000;
            return string.Format("{0}M", n);
        }
        else
            return "";
    }

    /// <summary>
	/// return x2 in [a2, b2] which is similar to x1 in [a1, b1]
	/// </summary>
	public static float GetLinearValueSimilarTo(float a1, float b1, float a2, float b2, float x1)
    {
        float t = Mathf.InverseLerp(a1, b1, x1);
        float x2 = Mathf.Lerp(a2, b2, t);
        return x2;
    }
}

public enum SCENE {
	HOME,
	UPGRADE,
	GAME,
	SHOP,
	NONE
}

