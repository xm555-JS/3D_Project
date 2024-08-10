using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cWeapon : MonoBehaviour
{
    public enum Type { MELEE, RANGE }
    public Type type;

    public int damage;
    public float rate;

    public int curAmmo;
    public int maxAmmo;

    public BoxCollider meleeArea;
    public TrailRenderer trailRenderer;

    public GameObject bullet;
    public Transform bulletPos;

    public GameObject bulletCase;
    public Transform bulletCasePos;

    public void Use()
    {
        if (type == Type.MELEE)
        {
            StopCoroutine("Swing");
            StartCoroutine("Swing");
        }
        if (type == Type.RANGE && curAmmo != 0)
        {
            StartCoroutine("InstantiateBullet");
            --curAmmo;
        }
    }

    IEnumerator Swing()
    {
        yield return null;
        meleeArea.enabled = true;
        trailRenderer.enabled = true;

        yield return new WaitForSeconds(0.3f);
        meleeArea.enabled = false;

        yield return new WaitForSeconds(0.3f);
        trailRenderer.enabled = false;
    }

    IEnumerator InstantiateBullet()
    {
        // ÃÑ¾Ë º¹»ç
        GameObject _bullet = Instantiate(bullet, bulletPos.position, bulletPos.rotation);

        Rigidbody rigid = _bullet.GetComponent<Rigidbody>();
        rigid.velocity = _bullet.transform.forward * 50f;
        yield return null;

        // ÅºÇÇ º¹»ç
        GameObject _bulletCase = Instantiate(bulletCase, bulletCasePos.position, bulletCasePos.rotation);
        Rigidbody rigidCase = _bulletCase.GetComponent<Rigidbody>();
        Vector3 caseVec = bulletCasePos.forward * Random.Range(-3f, -2f) + Vector3.up * Random.Range(2f, 3f);
        rigidCase.AddForce(caseVec, ForceMode.Impulse);
        rigidCase.AddTorque(Vector3.up * 50f, ForceMode.Impulse);
    }
}
