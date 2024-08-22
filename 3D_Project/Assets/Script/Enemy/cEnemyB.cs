using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0108
#pragma warning disable 0114

public class cEnemyB : cEnemy
{
    public BoxCollider meleeArea;

    float rushPower = 20f;

    void Awake()
    {
        radius = 1f;
        range = 15f;
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

        yield return new WaitForSeconds(0.1f);
        rigid.AddForce(transform.forward * rushPower, ForceMode.Impulse);
        meleeArea.enabled = true;

        yield return new WaitForSeconds(0.5f);
        rigid.velocity = Vector3.zero;
        meleeArea.enabled = false;

        yield return new WaitForSeconds(2f);

        isAttack = false;
        isChase = true;
        anim.SetBool("isAttack", isAttack);
    }

    protected override void DeadCount()
    {
        if (curHealth <= 0)
        {
            Debug.Log("B ¾Ë¸²");
            manager.enemyCountB--;
        }
    }
}
