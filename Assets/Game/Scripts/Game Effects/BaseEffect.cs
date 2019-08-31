using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEffect : MonoBehaviour {
	public EFFECT_TYPE type;
	public float duration;
	public Animator myAnim;
	public ParticleSystem myParticle;


    public virtual void Init(float damage)
    {

    }

	public virtual void Init () {
		Invoke("Expire", duration);
		if (myAnim != null) {
			myAnim.SetTrigger(AnimConst.start);
		}
		if (myParticle != null) {
			myParticle.Clear(true);
			myParticle.Play(true);
		}
	}

	public virtual void Expire () {
		EffectManager.Instance.PushEffect(this);
	}
}

public enum EFFECT_TYPE {
	FLOAT_TEXT,
	EXPLODE,
	PLAYER_DEATH,
    DAMAGE_TEXT,
    NONE
}