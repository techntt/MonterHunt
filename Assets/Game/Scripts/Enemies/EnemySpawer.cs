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
    #endregion;

    #region Unity Methods
    private void Awake()
    {
        ReadLevelData();
    }
    #endregion;

    #region Public Methods
    #endregion;

    #region Private Methods
    private void ReadLevelData()
    {
        TextAsset lvAsset = Resources.Load<TextAsset>(Const.LEVEL_DATA+ CampaignManager.campaign.id);
        List<Dictionary<string, string>> listTurn = CSVReader.ReadDataToList(lvAsset.text);
        waves = new List<Wave>();
        foreach(Dictionary<string,string> dict in listTurn)
        {
            //Debug.Log("wave : " + dict["wave"]);
            //Debug.Log("minHP : " + dict["minHP"]);
            //Debug.Log("maxHP : " + dict["maxHP"]);
            //Debug.Log("enemy : " + dict["enemy"]);
            Wave wave = new Wave();
            wave.id = dict["wave"];
            wave.minHP = float.Parse(dict["minHP"]);
            wave.maxHP = float.Parse(dict["maxHP"]);
            string enemyStr = dict["enemy"];
            if (enemyStr.Contains(":"))
            {
                string[] lstEnemy = enemyStr.Split(new char[] { ':' });
                for(int i = 0; i < lstEnemy.Length; i++)
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

        Debug.Log("Wave 0 test read enemy id: " + waves[0].GetEID());
        Debug.Log("Wave 1 test read enemy id: " + waves[1].GetEID());
        Debug.Log("Wave 2 test read enemy id: " + waves[2].GetEID());
        Debug.Log("Wave 3 test read enemy id: " + waves[3].GetEID());
        Debug.Log("Wave 4 test read enemy id: " + waves[4].GetEID());
    }
    #endregion;

    #region Private Class
    private class Wave
    {
        public string id;
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
        }
    }
    #endregion;
}
