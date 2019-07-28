using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Loading : MonoBehaviour
{

    private void Start()
    {
        FireBaseManager.Instance.GetDataFromLocal();
        Invoke("EnterHome",3);
    }


    private void EnterHome()
    {
        SceneManager.LoadScene(Const.SCENE_HOME);

    }
}
