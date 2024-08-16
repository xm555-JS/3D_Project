using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cMini_Weapon : MonoBehaviour
{
    public int damage;
    public float rate;
    public float speed;

    public GameObject bullet;
    public GameObject instateBullet;
    public Transform bulletPos;

    public void Use()
    {
        GameObject _bullet = Instantiate(bullet, bulletPos.position, bulletPos.rotation);
        instateBullet = _bullet;

        Rigidbody rigid = _bullet.GetComponent<Rigidbody>();
        rigid.velocity = _bullet.transform.forward * speed;
    }

    //IEnumerator InstantiateBullet()
    //{
    //    // ÃÑ¾Ë º¹»ç
    //    GameObject _bullet = Instantiate(bullet, bulletPos.position, bulletPos.rotation);
    //    instateBullet = _bullet;

    //    Rigidbody rigid = _bullet.GetComponent<Rigidbody>();
    //    rigid.velocity = _bullet.transform.forward * speed;
    //    yield return null;
    //}
}
