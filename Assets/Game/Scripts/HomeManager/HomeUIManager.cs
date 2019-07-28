using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HomeUIManager : SingletonMonoBehaviour<HomeUIManager>
{
    public Transform ship;
    public BaseMenuPopup[] popup;
    public RectTransform menubar;
    public Image[] buttons;
    public Sprite[] imgButton;
    public Vector2[] positions;


    private bool menuIsShowed;
    private int cMenu;

    private void Awake()
    {
        menuIsShowed = false;
        cMenu = -1;

    }

    public void ShowMenu(int index)
    {
        if (menuIsShowed)
        {
            if (cMenu != index)
            {
                buttons[index].sprite = imgButton[imgButton.Length - 1];
                buttons[cMenu].sprite = imgButton[cMenu];
                popup[cMenu].mTrans.DOAnchorPos(positions[2], 0.1f);
                popup[cMenu].Hide();
                cMenu = index;
                popup[cMenu].Show();
                popup[cMenu].mTrans.DOAnchorPos(positions[1], 0.3f);                
            }
            else
            {
                buttons[cMenu].sprite = imgButton[cMenu];
                popup[cMenu].mTrans.DOAnchorPos(positions[2], 0.1f);
                popup[cMenu].Hide();
                menubar.DOAnchorPos(positions[0], 0.1f);
                cMenu = -1;
                menuIsShowed = false;
                ship.DOMove(positions[5], 0.3f);
            }
        }
        else
        {
            menubar.DOAnchorPos(positions[3],0.3f);
            buttons[index].sprite = imgButton[imgButton.Length - 1];
            cMenu = index;
            popup[cMenu].Show();
            popup[cMenu].mTrans.DOAnchorPos(positions[1], 0.3f);
            menuIsShowed = true;
            ship.DOMove(positions[4], 0.3f);
        }
    }

}
