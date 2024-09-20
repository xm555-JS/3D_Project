using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class cSpawnMonster : MonoBehaviour
{
    [SerializeField] GameObject[] enemys;

    cObjectPool _pool;

    GameObject boss;
    int bossMaxHealth;
    int bossCurHealth;

    int enemyCount;

    float instateTime = 1f;
    
    bool isGameOver;
    bool isBossSpawn;
    bool isNormalSpawn = true;

    public GameObject GetBossObject() { return boss; }
    public int GetBossMaxHealth() { return bossMaxHealth; }
    public int GetBossCurHealth() { return bossCurHealth; }

    public event Action<cSpawnMonster> OnGetCoin;

    void Start()
    {
        _pool = gameObject.AddComponent<cObjectPool>();
    }

    void OnEnable()
    {
        cMini_Enemy.OnEnemyDeath += HandleEnemyDeath;
    }

    void OnDisable()
    {
        cMini_Enemy.OnEnemyDeath -= HandleEnemyDeath;
    }

    void Update()
    {
        if (boss)
            bossCurHealth = boss.GetComponent<cMini_Enemy>().GetCurHealth();
    }

    void LateUpdate()
    {
        if (isBossSpawn)
        {
            if (bossCurHealth <= 0)
            {
                boss = null;
                isBossSpawn = false;
            }
        }
    }

    public void SpawnEnemy(int stageNum)
    {
        CheckSpawn(stageNum);
        StartCoroutine(instateEnemy(stageNum));
    }

    public void DontSpawnEnemy()
    {
        StopAllCoroutines();
    }

    IEnumerator instateEnemy(int stageNum)
    {
        if (isBossSpawn)
        {
            boss = Instantiate(enemys[stageNum - 1], transform.position, transform.rotation);
            bossMaxHealth = boss.GetComponent<cMini_Enemy>().GetMaxHealth();
            bossCurHealth = bossMaxHealth;
        }

        float time = 0;
        while (isNormalSpawn)
        {
            time += Time.deltaTime;
            if (time >= instateTime)
            {
                //Instantiate(enemys[stageNum - 1], transform.position, transform.rotation);
                GameObject enemy = _pool.Spawn();
                //enemy.transform.SetParent(this.transform);
                enemy.transform.position = this.transform.position;
                enemy.transform.rotation = this.transform.rotation;
                enemyCount++;
                Debug.Log(enemyCount);
                time = 0f;
            }
            yield return null;
        }
        isNormalSpawn = true;
    }

    void CheckSpawn(int stageNum)
    {
        const int normalStage = 3;
        if (stageNum > normalStage)
        {
            isNormalSpawn = false;
            isBossSpawn = true;
            return;
        }
        else
            isNormalSpawn = true;
    }

    public bool CheckGameOver()
    {
        if (enemyCount >= 60)
            isGameOver = true;

        return isGameOver;
    }

    public void ResetSpawn()
    {
        isGameOver = false;
        enemyCount = 0;
    }

    void HandleEnemyDeath(cMini_Enemy enemy)
    {
        enemyCount--;
        OnGetCoin?.Invoke(this);
        Debug.Log(enemyCount);
    }
}
