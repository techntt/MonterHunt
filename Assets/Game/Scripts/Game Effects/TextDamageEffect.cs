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
        txtDMG.text =""+ (int)(damage*10);
        trans.DOMoveY(trans.position.y + 0.3f, 0.3f).OnComplete(()=> {
            EffectManager.Instance.PushEffect(this);
        });
    }
}
