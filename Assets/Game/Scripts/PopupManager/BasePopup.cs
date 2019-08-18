using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePopup : MonoBehaviour
{
    [HideInInspector]
    public bool isShow;

    public virtual void Show()
    {
        isShow = true;
    }

    public virtual void Hide()
    {
        isShow = false;
    }
}
