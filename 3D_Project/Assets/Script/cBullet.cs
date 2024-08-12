using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cBullet : MonoBehaviour
{
    public int damage;
    public bool isMelee;
    public bool isRock;

    //float destroyTime = 5f;
    //float timer;

    //void Update()
    //{
    //    timer += Time.deltaTime;
    //    if (timer >= destroyTime)
    //    {
    //        Destroy(gameObject);
    //        timer = 0;
    //    }
    //}

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
