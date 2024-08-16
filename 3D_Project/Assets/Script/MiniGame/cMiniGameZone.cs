using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cMiniGameZone : MonoBehaviour
{
    public GameManager manager;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Mini_Player"))
        {
            manager.MiniGameStart();
        }
    }
}
