using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cStartZone : MonoBehaviour
{
    public GameManager manager;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            manager.StageStart();
        }
    }
}
