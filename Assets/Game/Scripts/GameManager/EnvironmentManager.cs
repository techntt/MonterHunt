using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EnvironmentManager : MonoBehaviour {

	public SpriteRenderer background;
	public ParticleSystem cloud, star;

	public EnvironmentData[] environments;

	void Start () {
		int id = CampaignManager.campaign.bossID;
		EnvironmentData e = environments[id];
		background.color = e.bgColor;
		ParticleSystem.MainModule m = cloud.main;
		m.startColor = e.cloudColor;
		ParticleSystem.MainModule n = star.main;
		n.startColor = e.starColor;
		ParticleSystem.TextureSheetAnimationModule t = cloud.textureSheetAnimation;
		t.rowIndex = e.spriteRow;
	}
}

[System.Serializable]
public class EnvironmentData {
	public Color bgColor;
	public Color cloudColor;
	public Color starColor;
	public int spriteRow;
}