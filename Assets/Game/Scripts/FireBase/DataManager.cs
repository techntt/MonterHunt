using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : SingletonMonoBehaviour<DataManager>
{
    [HideInInspector]
    public string campaign;
    [HideInInspector]
    public string ship;
    [HideInInspector]
    public string quest;
    [HideInInspector]
    public string[] difficulty;
}
