using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cItem : MonoBehaviour
{
    // type
    public enum Type { AMMO, COIN, GRENADE, HEART, WEAPON };
    public Type type;
    public int value;

    // info
    float rotationSpeed = 20f;

    // Update is called once per frame
    void Update()
    {
        this.transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }
}
