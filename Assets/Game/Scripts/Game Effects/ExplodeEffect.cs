using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class ExplodeEffect : BaseEffect {

	public CircleCollider2D myCollider;
	float damage;
	public AudioClip sfx_explode;

	/// <summary>
	/// Init an explosion with size, damage and layer
	/// </summary>
	/// <param name="size">Size of the explosion</param>
	/// <param name="damage">Damage of the explosion.</param>
	/// <param name="layer">Layer of the explosion, decide it would damage the enemy or player. Default is "bullet"</param>
	public void Init (float size, float damage, int layer = 10) {
		base.Init();
		this.damage = damage;
		gameObject.layer = layer;
		ParticleSystem.MainModule main = myParticle.main;
		main.startSize = size;
		myCollider.radius = size * 0.4f;
		myCollider.enabled = true;
		Invoke("StopCollide", 0.2f);
		CameraShake.Instance.Vibrate(1, 0.05f);
		SoundManager.Instance.PlaySfxRewind(sfx_explode);
	}

	void OnTriggerEnter2D (Collider2D col) {
		Damageable d = ColliderRef.Instance.GetDamageable (col.GetInstanceID ());
		if (d != null) {
			d.TakeDamage(damage);
		}
	}

	void StopCollide () {
		myCollider.enabled = false;
	}
}