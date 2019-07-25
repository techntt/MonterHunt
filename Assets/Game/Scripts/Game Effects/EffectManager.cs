using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : SingletonMonoBehaviour<EffectManager> {

	public BaseEffect[] samples;
	SortedList<EFFECT_TYPE, BaseEffect> _samples = new SortedList<EFFECT_TYPE, BaseEffect>();
	SortedList<EFFECT_TYPE, Stack<BaseEffect>> effects = new SortedList<EFFECT_TYPE, Stack<BaseEffect>>();

	void Start () {
		for (int i = 0; i < samples.Length; i++) {
			_samples.Add(samples[i].type, samples[i]);
			effects.Add(samples[i].type, new Stack<BaseEffect>());
		}
		for (int i = 0; i < (int)EFFECT_TYPE.NONE; i++) {
			BaseEffect e = SpawnEffect((EFFECT_TYPE)i, new Vector3 (100, 100));
			PushEffect(e);
		}
	}

	public BaseEffect SpawnEffect (EFFECT_TYPE type, Vector3 pos) {
		// if the required effect does not exist
		if (!effects.ContainsKey(type))
			return null;
		// if there is no stored effect left in the pool
		if (effects[type].Count == 0) {
			BaseEffect e = Instantiate(_samples[type]) as BaseEffect;
			e.transform.position = pos;
			e.transform.parent = transform;
			return e;
		} else {
			BaseEffect e = effects[type].Pop();
			e.transform.position = pos;
			e.gameObject.SetActive(true);
			return e;
		}
	}

	public void PushEffect (BaseEffect e) {
		e.gameObject.SetActive(false);
		effects[e.type].Push(e);
	}
}