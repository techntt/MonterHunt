using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawer : SingletonMonoBehaviour<EnemySpawer>
{
    #region Inspector Variables
    #endregion;

    #region Member Variables
    public int minHP, maxHP;
    private float minDelay = 0.3f, maxDelay = 1;
    private float minSize = 0.8f, maxSize = 1.5f;
    #endregion;

    #region Unity Methods
    private void Awake()
    {
        GameEventManager.Instance.GameStart += HandleGameStart;
        GameEventManager.Instance.EnemyExplode += HandleEnemyDie;
    }

    private void Start()
    {
        ReadLevelData();
    }
    #endregion;

    #region Public Methods

    #endregion;

    #region Private Methods
    private void ReadLevelData()
    {
        TextAsset lvAsset = Resources.Load<TextAsset>(Const.LEVEL_DATA + CampaignManager.campaign.id);
        List<Dictionary<string, string>> listTurn = CSVReader.ReadDataToList(lvAsset.text);       

    }

    
    private void SpawnEnemy()
    {
        ENEMY_STYLE style = (ENEMY_STYLE)Random.Range(0, (int)ENEMY_STYLE.NORMAL);
        float size = Random.Range(minSize, maxSize);
        Vector2 startPos = Vector2.zero;
        startPos.y = Camera189.gameView.yMax + 1;
        startPos.x = Random.Range(Camera189.gameView.xMin - 2, Camera189.gameView.xMax + 2);
        BaseEnemy enemy = EnemyManager.Instance.PopEnemy(style,size,startPos);
        int hp = Random.Range(minHP, maxHP);
        Vector2 direct = Vector2.zero;
        if(startPos.x < Camera189.gameView.xMin*2/3)
        {
            direct.y = Random.Range(Camera189.gameView.yMin, Camera189.gameView.yMax * 1 / 3);
            direct.x = Random.Range(0, Camera189.gameView.xMax);
        } else if(startPos.x > Camera189.gameView.xMax*2/3)
        {
            direct.y = Random.Range(Camera189.gameView.yMin, Camera189.gameView.yMax * 1 / 3);
            direct.x = Random.Range(Camera189.gameView.xMin,0);
        }
        else
        {
            direct.y = Camera189.gameView.yMin - 1;
            direct.x = Random.Range(Camera189.gameView.xMin, Camera189.gameView.xMax);
        }
        direct = direct.normalized;
        enemy.Init(style, hp, direct, Random.Range(1f,3f));
    }

    private void HandleGameStart()
    {
        InvokeRepeating("SpawnEnemy", 0, 1);
    }
    

    private void HandleEnemyDie(BaseEnemy enemy)
    {
       
    }

    
    #endregion;

    #region Private Class
   
    #endregion;
}
