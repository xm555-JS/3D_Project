using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#pragma warning disable 0108
#pragma warning disable 0649

public class cBoss : cEnemy
{
    public int maxHealthBoss;
    public int curHealthBoss;

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
        maxHealth = maxHealthBoss;
        curHealth = curHealthBoss;
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

        curHealthBoss = curHealth;
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
                Debug.Log("미사일");
                StartCoroutine("ShotMissile");
                break;
            case 2:
            case 3:
                Debug.Log("돌");
                // 돌 굴리기
                StartCoroutine("ShotRock");
                break;
            case 4:
                Debug.Log("점프");
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
        // 인라인으로 처리하면 뭐가 안좋나?
        cBossMissile missilACom = missileA.GetComponent<cBossMissile>();
        missilACom.target = target;

        yield return new WaitForSeconds(0.3f);

        GameObject missileB = Instantiate(missile, missilePortB.transform.position, missilePortB.transform.rotation);
        // 인라인으로 처리하면 뭐가 안좋나?
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
        Debug.Log(transform.rotation);
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
        //isAttack = true;
        //isChase = false;
        //anim.SetBool("isAttack", isAttack);

        //yield return new WaitForSeconds(0.5f);
        //GameObject _bullet = Instantiate(bullet, this.transform.position, this.transform.rotation);
        //Rigidbody rigid_bullet = _bullet.GetComponent<Rigidbody>();
        //rigid_bullet.AddForce(transform.forward * shotPower, ForceMode.Impulse);

        //yield return new WaitForSeconds(2f);

        //isAttack = false;
        //isChase = true;
        //anim.SetBool("isAttack", isAttack);
    }
}
