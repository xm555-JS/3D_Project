using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cSpawnMonster : MonoBehaviour
{
    [SerializeField] GameObject[] enemys;

    float instateTime = 1f;
    bool isNormalSpawn = true;
    bool isBossSpawn;

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
            Instantiate(enemys[stageNum - 1], transform.position, transform.rotation);


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
}
