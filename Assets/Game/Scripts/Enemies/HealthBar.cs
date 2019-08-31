using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBar : MonoBehaviour
{
    #region Inspector Variables
    public Transform trans;
    public SpriteRenderer outline,bar;
    #endregion

    #region Member Variables
    Vector2 maxSize;
    float maxHp;
    #endregion;

    #region Public Methods
    public void Init(float ySize,float maxHp)
    {
        Vector2 pos = trans.localPosition;
        pos.y = (ySize / 2) + 0.15f;
        trans.localPosition = pos;
        maxSize = bar.size;
        this.maxHp = maxHp;
        SetHealthBar(maxHp);
    }

    public void SetHealthBar(float hp)
    {
        float percent = hp / maxHp;
        Vector2 newSize = maxSize;
        newSize.x *= percent;
        bar.size = newSize;
    }
    #endregion;

}
