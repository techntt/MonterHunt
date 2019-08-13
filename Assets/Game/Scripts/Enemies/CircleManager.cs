using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleManager : SingletonMonoBehaviour<CircleManager> {

	public Circle sample;
	public Stack<Circle> pool = new Stack<Circle>();
    public Stack<SpecialObject> dummPool = new Stack<SpecialObject>();
	public Sprite[] CircleSprite;
    public float[] spriteRatio;
	public ParticleSystem explodeEffect;
	Stack<ParticleSystem> explodes = new Stack<ParticleSystem>();

	void Start () {
		List<Circle> c = new List<Circle>();
		for (int i = 0; i < 5; i++)
			c.Add(PopCircle(CircleType.BOMB, 1, new Vector3 (100, 100)));
		for (int i = 0; i < 5; i++)
			PushCircle(c[i]);
		if (PlayerSettingData.Instance.graphic == GRAPHIC_QUALITY.HIGH) {
			for (int i = 0; i < 5; i++) {
				SpawnExplodeEffect(new Vector3 (100, 100), Color.white);
			}
		}
	}

	public Circle PopCircle (CircleType type, float size, Vector3 pos) {
		Circle b;
		if (pool.Count == 0) {
			b = Instantiate(sample, pos, Quaternion.Euler(0,0,0)) as Circle;
			b.transform.parent = transform;
			ColliderRef.Instance.DamageableRef.Add(b.myCollider.GetInstanceID(), b);
		} else {
			b = pool.Pop();
			b.transform.position = pos;
			b.gameObject.SetActive(true);
		}
		b.type = type;
		b.size = size;
		b.myCollider.radius = size/2;
        switch (type)
        {
            case CircleType.GLOW:
                b.myRender.size = new Vector2(size * spriteRatio[0], size);
                break;
            case CircleType.BOMB:
                b.myRender.size = new Vector2(size * spriteRatio[1], size);
                break;
            case CircleType.HARDEN:
                b.myRender.size = new Vector2(size * spriteRatio[2], size);
                break;
            case CircleType.NORMAL:
                b.myRender.size = new Vector2(size * spriteRatio[3], size);
                break;
        }		
		b.myRender.sprite = CircleSprite[(int)type];
		return b;
	}

	public void PushCircle (Circle b) {
		b.gameObject.SetActive(false);
		pool.Push(b);
	}


    public SpecialObject PopSPObj()
    {
        SpecialObject spObj;
        if (dummPool.Count == 0)
        {
            GameObject obj = Instantiate(Resources.Load(Const.ORIGINAL_SP)) as GameObject;
            spObj = obj.GetComponent(typeof(SpecialObject)) as SpecialObject;
            spObj.transform.parent = transform;
            ColliderRef.Instance.DamageableRef.Add(spObj.myCollider.GetInstanceID(), spObj);
        }
        else
        {
            spObj = dummPool.Pop();
            spObj.gameObject.SetActive(true);
        }        
        return spObj;
    }

    public void PushSPObj(SpecialObject dumm)
    {
        dumm.gameObject.SetActive(false);
        dummPool.Push(dumm);
    }

	public ParticleSystem SpawnExplodeEffect (Vector3 pos, Color color) {
		ParticleSystem s;
		if (explodes.Count == 0) {
			s = Instantiate(explodeEffect) as ParticleSystem;
			s.transform.parent = transform;
		} else {
			s = explodes.Pop();
			s.gameObject.SetActive(true);
		}
		s.transform.position = pos;
		ParticleSystem.MainModule m = s.main;
		m.startColor = color;
		s.Clear();
		s.Play();
		StartCoroutine("PoolExplodeEffect", s);
		return s;
	}

	IEnumerator PoolExplodeEffect (ParticleSystem s) {
		yield return new WaitForSeconds(s.main.duration);
		s.gameObject.SetActive(false);
		explodes.Push(s);
	}

	void OnDestroy () {
		StopAllCoroutines();
	}
}

public enum CircleType {
	GLOW, // always give bonus
	BOMB, // deal damage to nearby circles
	HARDEN, // cannot be destroyed
	NORMAL
}

public enum CircleOrbit {
	D, // down
	L, // down left
	R, // down right
	ZL, // zigzag left
	ZR, // zigzag right
	NONE
}