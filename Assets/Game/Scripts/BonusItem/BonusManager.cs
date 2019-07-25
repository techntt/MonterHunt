using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// manage creating and pooling Bonus
/// </summary>
public class BonusManager : SingletonMonoBehaviour<BonusManager> {

	public const int specialBonusChance = 50;
	public int bonusOnScreen;
	int maxBonusOnScreen = 3;

	public Bonus sample;
	public Stack<Bonus> pool = new Stack<Bonus>();
	public Sprite[] bonusSprite;

	void Start () {
		List<Bonus> b = new List<Bonus>();
		for (int i = 0; i < 3; i++) {
			b.Add(GetBonus(BonusType.Bloom));
		}
		for (int i = 0; i < 3; i++)
			PushBonus(b[i]);
		bonusOnScreen = 0;
	}

	public Bonus GetBonus (BonusType type) {
		if (bonusOnScreen < maxBonusOnScreen) {
			Bonus b = null;
			if (pool.Count == 0) {
				b = Instantiate(sample) as Bonus;
				b.transform.parent = transform;
				ColliderRef.Instance.bonusRef.Add(b.myCollider.GetInstanceID(), b);
			} else {
				b = pool.Pop();
				b.gameObject.SetActive(true);
			}
			b.type = type;
			b.myRender.sprite = bonusSprite[(int)type];
			bonusOnScreen++;
			return b;
		} else
			return null;
	}

	public void PushBonus (Bonus b) {
		b.gameObject.SetActive(false);
		bonusOnScreen--;
		pool.Push(b);
	}

	public BonusType GetRandomBonusType () {
		if (Random.Range(0, 100) < 40) {
			return (BonusType)Random.Range(0, (int)BonusType.None - 1);
		} else
			return BonusType.PowerUp;
	}
}

public enum BonusType {
	Bloom,
	Rocket,
	Laser,
	Lightning,
	Rotate,
	Saw,
	Seek,
	Slow,
	SuperWeapon,
	Divide30,
	Divide180,
	Divide360,
	Shield,
	XDam,
	XPoint,
	Bomb,
	Magnet,
	PowerUp,
	None
}