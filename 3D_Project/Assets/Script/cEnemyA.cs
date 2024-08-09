using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0108

public class cEnemyA : cEnemy
{
    public int maxHealthA;
    public int curHealthA;
    public BoxCollider meleeArea;

    void Awake()
    {
        maxHealth = maxHealthA;
        curHealth = curHealthA;
        radius = 1.5f;
        range = 2f;
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

        yield return new WaitForSeconds(0.2f);
        meleeArea.enabled = true;

        yield return new WaitForSeconds(1f);
        meleeArea.enabled = false;

        isAttack = false;
        isChase = true;
        anim.SetBool("isAttack", isAttack);
    }
}
