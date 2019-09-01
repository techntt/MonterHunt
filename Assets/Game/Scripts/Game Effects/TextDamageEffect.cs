using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class TextDamageEffect : BaseEffect
{
    public TextMeshPro txtDMG;
    public Transform trans;

    public override void Init(float damage)
    {
        //float size = 25f;
        //if (damage >= (CircleSpawner.Instance.maxHP / 3))
        //    size = 40;
        //else if (damage >= ((CircleSpawner.Instance.maxHP / 4)))
        //    size = 30f;
        //else
        //    size = 25f;
        //txtDMG.fontSize = size;
        txtDMG.text =""+ (int)(damage*10);
        trans.DOMoveY(trans.position.y + 0.3f, 0.3f).OnComplete(()=> {
            EffectManager.Instance.PushEffect(this);
        });
    }
}
