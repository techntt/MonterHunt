using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyBullet : MonoBehaviour {

    #region Inspector Variables
    public DummyObject[] dumms;
    #endregion;

    #region Member Variables
    private DummyObject myDummy;
    #endregion;

    #region Unity Methods

    private void OnEnable()
    {
        int index = Random.Range(0, dumms.Length);
        index = Mathf.Clamp(index, 0, dumms.Length - 1);
        myDummy = dumms[index];
        myDummy.maxHp = CircleSpawner.Instance.maxHP;
        myDummy.hp = Random.Range(1, CircleSpawner.Instance.h2 + 1);
        myDummy.Init();
        myDummy.gameObject.SetActive(true);
        myDummy.myRender.enabled = true;
    }
    private void OnDisable()
    {
        foreach(DummyObject obj in dumms)
        {
            obj.gameObject.SetActive(false);
        }
    }
    #endregion;

    #region Public Methods

    #endregion;

    #region Private Methods
    #endregion;

}
