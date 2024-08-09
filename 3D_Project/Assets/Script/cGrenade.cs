using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cGrenade : MonoBehaviour
{
    public GameObject meshObj;
    public GameObject EffectObj;
    public Rigidbody rigid;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("Explosion");
    }

    IEnumerator Explosion()
    {
        yield return new WaitForSeconds(3f);
        rigid.velocity = Vector3.zero;
        rigid.angularVelocity = Vector3.zero;
        meshObj.SetActive(false);
        EffectObj.SetActive(true);

        RaycastHit[] hits = Physics.SphereCastAll(this.transform.position, 15f, Vector3.up, 0f, LayerMask.GetMask("Enemy"));

        foreach (RaycastHit hitObj in hits)
        {
            hitObj.transform.GetComponent<cEnemy>().HitByGrenade(this.transform.position);
        }

        Destroy(gameObject, 5f);
    }
}
