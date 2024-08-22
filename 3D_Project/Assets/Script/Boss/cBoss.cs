using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0108
#pragma warning disable 0649

public class cBoss : cEnemy
{
    public GameObject missilePortA;
    public GameObject missilePortB;
    public GameObject missile;

    public GameObject bossBullet;
    public BoxCollider tauntCollider;

    Vector3 lookVec;
    Vector3 tauntVec;
    bool isLook;

    void Awake()
    {
        radius = 0.5f;
        range = 25f;
        Initialize();
        nav.isStopped = true;
        StartCoroutine("Think");
    }

    void Start()
    {
        base.Start();
        isLook = true;
    }

    protected override void Update()
    {
        if (isDead)
            return;

        if (isLook)
        {
            lookVec = target.GetComponent<Player>().GetPlayerDir();
            lookVec = lookVec * 5;
            transform.LookAt(target.position + lookVec);
        }
        else
        {
            nav.SetDestination(tauntVec);
        }
    }

    IEnumerator Think()
    {
        yield return new WaitForSeconds(0.3f);

        int randAction = Random.Range(0, 5);
        switch (randAction)
        {
            case 0:
            case 1:
                // 미사일 발사
                StartCoroutine("ShotMissile");
                break;
            case 2:
            case 3:
                // 돌 굴리기
                StartCoroutine("ShotRock");
                break;
            case 4:
                // 점프 공격 패턴
                StartCoroutine("JumpAttack");
                break;
        }
    }

    IEnumerator ShotMissile()
    {
        anim.SetTrigger("doShot");
        
        yield return new WaitForSeconds(0.2f);
        
        GameObject missileA = Instantiate(missile, missilePortA.transform.position, missilePortA.transform.rotation);
        cBossMissile missilACom = missileA.GetComponent<cBossMissile>();
        missilACom.target = target;

        yield return new WaitForSeconds(0.3f);

        GameObject missileB = Instantiate(missile, missilePortB.transform.position, missilePortB.transform.rotation);
        cBossMissile missileBCom = missileB.GetComponent<cBossMissile>();
        missileBCom.target = target;

        yield return new WaitForSeconds(2f);

        StartCoroutine("Think");
    }

    IEnumerator ShotRock()
    {
        isLook = false;
        anim.SetTrigger("doBigShot");
        Instantiate(bossBullet, transform.position, transform.rotation);
        yield return new WaitForSeconds(3f);

        isLook = true;
        StartCoroutine("Think");
    }

    IEnumerator JumpAttack()
    {
        tauntVec = target.position + lookVec;

        isLook = false;
        nav.isStopped = false;
        boxCollider.enabled = false;

        anim.SetTrigger("doTaunt");

        yield return new WaitForSeconds(1.5f);
        tauntCollider.enabled = true;

        yield return new WaitForSeconds(0.5f);
        tauntCollider.enabled = false;

        yield return new WaitForSeconds(1f);
        boxCollider.enabled = true;
        nav.isStopped = true;
        isLook = true;
        StartCoroutine("Think");
    }

    protected override IEnumerator Attack()
    {
        yield return null;
    }

    protected override void DeadCount()
    {
        if (curHealth <= 0)
            manager.enemyCountD--;
    }
}
