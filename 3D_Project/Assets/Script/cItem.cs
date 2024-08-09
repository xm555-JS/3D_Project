using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cItem : MonoBehaviour
{
    // component
    Rigidbody rigid;
    SphereCollider sphereCollider;

    // type
    public enum Type { AMMO, COIN, GRENADE, HEART, WEAPON };
    public Type type;
    public int value;

    // info
    float rotationSpeed = 20f;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        sphereCollider = GetComponent<SphereCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Floor"))
        {
            rigid.isKinematic = true;
            sphereCollider.enabled = false;
        }
    }
}
