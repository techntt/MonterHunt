using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class D3SwordController : MonoBehaviour {

    #region Inspector Variables
    public Animator myAnim;
    public Rigidbody2D myBody;
    public DummyObject[] path;
    public float speed = 50;
    #endregion;

    #region Member Variables
    [HideInInspector] public Transform trans;
    private Transform player;
    private int count;
    #endregion;

    #region Unity Methods
    private void Awake()
    {        
        player = GameManager.Instance.player1.transform;
        trans = gameObject.GetComponent(typeof(Transform)) as Transform;
        count = 0;
    }
    #endregion;

    #region Public Methods
    public void SwordActive()
    {
        // Calculate angle with player        
        foreach (DummyObject obj in path)
        {
            obj.maxHp = CircleSpawner.Instance.maxHP;
            obj.hp = Random.Range(1, CircleSpawner.Instance.h2 + 1);
            obj.NewInit();
        }      
        Vector3 direction = (player.position - trans.position).normalized ;
        myBody.velocity = new Vector3(0,-1,0);
        Invoke("SwordInActive", 1.5f);
    }

    public void SwordInActive()
    {
        myBody.velocity = Vector3.zero;
        foreach (DummyObject obj in path)
        {            
            obj.Reset();           
        }
        myAnim.SetTrigger("inactive");
    }
    #endregion;

    #region Private Methods   
    
    #endregion;
}
