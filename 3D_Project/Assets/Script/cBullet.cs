using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cBullet : MonoBehaviour
{
    public int damage;
    public bool isMelee;
    public bool isRock;

    void OnCollisionEnter(Collision collision)
    {
        if (!isRock && collision.gameObject.CompareTag("Floor"))
            Destroy(gameObject, 3f);
    }

    void OnTriggerEnter(Collider other)
    {
        if (!isMelee && other.gameObject.CompareTag("Wall"))
            Destroy(gameObject);
    }
}
