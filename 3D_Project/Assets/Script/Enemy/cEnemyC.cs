using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0108
#pragma warning disable 0114

public class cEnemyC : cEnemy
{
    public GameObject bullet;

    float shotPower = 20f;

    void Awake()
    {
        radius = 0.5f;
        range = 25f;
        base.Awake();
    }

    void Start()
    {
        base.Start();
    }

    void Update()
    {
        base.Update();
    }

    void FixedUpdate()
    {
        base.FixedUpdate();
    }

    protected override IEnumerator Attack()
    {
        isAttack = true;
        isChase = false;
        anim.SetBool("isAttack", isAttack);

        yield return new WaitForSeconds(0.5f);
        GameObject _bullet = Instantiate(bullet, this.transform.position, this.transform.rotation);
        Rigidbody rigid_bullet = _bullet.GetComponent<Rigidbody>();
        rigid_bullet.AddForce(transform.forward * shotPower, ForceMode.Impulse);

        yield return new WaitForSeconds(2f);

        isAttack = false;
        isChase = true;
        anim.SetBool("isAttack", isAttack);
    }

    protected override void DeadCount()
    {
        if (curHealth <= 0)
        {
            Debug.Log("C ¾Ë¸²");
            manager.enemyCountC--;
        }
    }
}
