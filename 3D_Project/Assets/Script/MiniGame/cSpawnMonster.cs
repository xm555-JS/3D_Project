using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cSpawnMonster : MonoBehaviour
{
    public GameObject enemyA;
    public GameObject enemyB;
    public GameObject enemyC;
    public GameObject enemyD;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z))
        {
            Instantiate(enemyA, transform.position, transform.rotation);
        }
        if (Input.GetKeyDown(KeyCode.X))
        {
            Instantiate(enemyB, transform.position, transform.rotation);
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            Instantiate(enemyC, transform.position, transform.rotation);
        }
        if (Input.GetKeyDown(KeyCode.V))
        {
            Instantiate(enemyD, transform.position, transform.rotation);
        }
    }
}
