using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bonus : MonoBehaviour {

	public BonusType type;
	public int value;
	public SpriteRenderer myRender;
	public Collider2D myCollider;
	public Rigidbody2D myBody;
	public AudioClip sfx;
	public float speed;

	public void Init () {
		if (!Camera189.gameView.Contains(transform.localPosition)) {
			BonusManager.Instance.PushBonus(this);
			return;
		}
		myBody.velocity = Vector2.down * speed;
		value = Random.Range(0, 3);
	}

	void OnTriggerEnter2D (Collider2D col) {
		if (col.CompareTag(Const.TAG_BOUNDARY)) {
			TextEffect t = (TextEffect)EffectManager.Instance.SpawnEffect(EFFECT_TYPE.FLOAT_TEXT, transform.position);
			t.Init(GetBonusDescription(type, value), 25, Color.white);
			gameObject.SetActive(false);
			SoundManager.Instance.PlaySfx(sfx, SFX_PLAY_STYLE.OVERRIDE);
			GameEventManager.Instance.OnPlayerGetBonus(GameManager.Instance.player, type, value);
		}
	}

	void OnBecameInvisible () {
		BonusManager.Instance.PushBonus(this);
	}

	public string GetBonusDescription (BonusType t, int value) {
		switch (t) {
			case BonusType.Bloom:
				return "Force Wave";
			case BonusType.Bomb:
				return "Bomb";
			case BonusType.Divide180:
				return "Rain of Arrow";
			case BonusType.Divide30:
				return "Wind Cutter";
			case BonusType.Divide360:
				return "Halo of God";
			case BonusType.Laser:
				if (value == 0)
					return "Cyan Laser";
				else if (value == 1)
					return "Yellow Laser";
				else
					return "Red Laser";
			case BonusType.Lightning:
				return "Force Chain";
			case BonusType.Magnet:
				return "Magnet";
			case BonusType.PowerUp:
				return "Power Up";
			case BonusType.Rocket:
				return "Rocket";
			case BonusType.Rotate:
				if (value == 0)
					return "Cyan Rotor";
				else if (value == 1)
					return "Yellow Rotor";
				else
					return "Red Rotor";
			case BonusType.Saw:
				return "Buzzsaw";
			case BonusType.Seek:
				return "Homing Missles";
			case BonusType.Shield:
				return "Energy Shield";
			case BonusType.Slow:
				return "Ice Bullets";
			case BonusType.SuperWeapon:
				return "Max Weapon";
			case BonusType.XDam:
				if (value == 0)
					return "Damage + 50%";
				else if (value == 1)
					return "Damage + 100%";
				else
					return "Damage + 150%";
			case BonusType.XPoint:
				if (value == 0)
					return "Score x2";
				else if (value == 1)
					return "Score x3";
				else
					return "Score x4";
			default:
				return "";
		}
	}
}