using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class cSpawnMonster : MonoBehaviour
{
    [SerializeField] GameObject[] enemys;

    GameObject boss;

    int enemyCount;

    float instateTime = 1f;
    
    bool isGameOver;
    bool isBossSpawn;
    bool isNormalSpawn = true;

    public GameObject GetBossObject() { return boss; }

    void OnEnable()
    {
        cMini_Enemy.OnEnemyDeath += HandleEnemyDeath;
        cMini_Enemy.OnEnemyDeath += HandleEnemyAwake;
    }

    void OnDisable()
    {
        cMini_Enemy.OnEnemyDeath -= HandleEnemyDeath;
        cMini_Enemy.OnEnemyDeath -= HandleEnemyAwake;
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
        }
            

        float time = 0;
        while (isNormalSpawn)
        {
            time += Time.deltaTime;
            if (time >= instateTime)
            {
                Instantiate(enemys[stageNum - 1], transform.position, transform.rotation);
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
        if (enemyCount >= 20)
        {
            isGameOver = true;
        }

        return isGameOver;
    }

    public void ResetSpawn()
    {
        isGameOver = false;
        enemyCount = 0;
    }

    void HandleEnemyDeath(cMini_Enemy enemy)
    {
        enemyCount++;
        Debug.Log(enemyCount);
    }

    void HandleEnemyAwake(cMini_Enemy enemy)
    {
        enemyCount--;
        //Debug.log(enemyCount);
    }
}
