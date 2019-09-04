using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawer : SingletonMonoBehaviour<EnemySpawer>
{
    #region Inspector Variables
    #endregion;

    #region Member Variables
    private List<Wave> waves;
    private int cWave;
    private int minHP, maxHP;
    private float minDelay = 0.3f, maxDelay = 1;
    private int enemyExist;
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
        waves = new List<Wave>();
        foreach (Dictionary<string, string> dict in listTurn)
        {
            //Debug.Log("wave : " + dict["wave"]);
            //Debug.Log("minHP : " + dict["minHP"]);
            //Debug.Log("maxHP : " + dict["maxHP"]);
            //Debug.Log("enemy : " + dict["enemy"]);
            Wave wave = new Wave();
            wave.id = dict["wave"];
            wave.minHP = float.Parse(dict["minHP"]);
            wave.maxHP = float.Parse(dict["maxHP"]);
            wave.number = int.Parse(dict["number"]);
            string enemyStr = dict["enemy"];
            if (enemyStr.Contains(":"))
            {
                string[] lstEnemy = enemyStr.Split(new char[] { ':' });
                for (int i = 0; i < lstEnemy.Length; i++)
                {
                    string[] data = lstEnemy[i].Split(new char[] { '-' });
                    EnemyPercent ep = new EnemyPercent(data[0], data[1]);
                    wave.enemies.Add(ep);
                }
            }
            else
            {
                string[] data = enemyStr.Split(new char[] { '-' });
                EnemyPercent ep = new EnemyPercent(data[0], data[1]);
                wave.enemies.Add(ep);
            }
            waves.Add(wave);
        }

    }



    private IEnumerator SpawnWave(Wave wave)
    {
        for(int i = 0; i < wave.number; i++)
        {
            SpawnEnemy(wave.GetEID());
            if (i == wave.number - 1)
                yield return null;
            else
                yield return new WaitForSeconds(Random.Range(minDelay, maxDelay));
        }
    }


    private void SpawnEnemy(string id)
    {

    }

    private void HandleGameStart()
    {
        cWave = 0;
        StartWave();
    }

    private void StartWave()
    {
        Wave wave = waves[cWave];
        enemyExist = wave.number;
        StartCoroutine(SpawnWave(wave));
    }


    private void HandleEnemyDie(BaseEnemy enemy)
    {
        enemyExist -= 1;
        if (enemyExist <= 0)
        {
            // All enemy of wave is killed
            if(cWave < waves.Count)
            {
                // Start next wave
                cWave++;
                Invoke("StartWave", 2);
            }
            else
            {
                // Create boss

            }
        }
    }

    
    #endregion;

    #region Private Class
    private class Wave
    {
        public string id;
        public int number;
        public float minHP, maxHP;
        public List<EnemyPercent> enemies;

        public Wave()
        {
            enemies = new List<EnemyPercent>();
        }

        public string GetEID()
        {
            int percent = Random.Range(0, 99);
            float total = 0;
            for(int i = 0; i < enemies.Count; i++)
            {
                total += enemies[i].percent;
                if (percent <= total)
                    return enemies[i].id;
            }
            return null;
        }
    }

    private class EnemyPercent
    {
        public string id;
        public float percent;
        public EnemyPercent(string id,string percent)
        {
            this.id = id.Trim();
            this.percent = float.Parse(percent.Trim());
            int num = 5;
            if (this.percent > 50)
                num = 7;
            else if (this.percent < 30)
                num = 5;
            else
                num = 2;
            EnemyManager.Instance.PrepareEnemy(id,num);
        }
    }
    #endregion;
}
