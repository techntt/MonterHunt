using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseMenuPopup : MonoBehaviour
{
    public RectTransform mTrans;
    public virtual void Show() { }
    public virtual void Hide() { }
}
