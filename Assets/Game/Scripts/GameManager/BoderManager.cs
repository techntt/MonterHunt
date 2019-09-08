using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoderManager : MonoBehaviour
{
    public Transform top, bot, left, right;
    // Start is called before the first frame update
    void Start()
    {
        top.position = new Vector2(0, Camera189.gameView.yMax);
        bot.position = new Vector2(0, Camera189.gameView.yMin);
        left.position = new Vector2(Camera189.gameView.xMin-0.1f, 0);
        right.position = new Vector2(Camera189.gameView.xMax, 0);
    }
    
}
