using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class cEnemy : MonoBehaviour
{
    protected int maxHealth;
    protected int curHealth;
    protected float radius = 1.5f;
    protected float range = 2f;
    protected Transform target;
    protected bool isChase;
    protected bool isAttack;

    //public BoxCollider meleeArea;
    protected Rigidbody rigid;
    protected BoxCollider boxCollider;
    protected Material material;
    protected NavMeshAgent nav;
    protected Animator anim;

    protected void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        material = GetComponentInChildren<MeshRenderer>().material;
        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();

        Invoke("Chase", 2f);
    }

    protected void Start()
    {
        GameObject player = GameObject.FindWithTag("Player");
        target = player.GetComponent<Transform>();
    }

    protected void Update()
    {
        if (nav.enabled)
        {
            nav.SetDestination(target.position);
            nav.isStopped = !isChase;
        }
    }

    protected void FixedUpdate()
    {
        FreezeVelocity();
        Targetting();
    }

    void Targetting()
    {
        //float radius = 1.5f;
        //float range = 2f;
        RaycastHit[] hits = Physics.SphereCastAll(this.transform.position, radius, transform.forward, range, LayerMask.GetMask("Player"));

        if (hits.Length > 0 && !isAttack)
        {
            StartCoroutine("Attack");
        }
    }

    protected abstract IEnumerator Attack();

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Melee"))
        {
            cWeapon weapon = other.GetComponent<cWeapon>();
            curHealth -= weapon.damage;

            Vector3 hitDir = KnockBack(other);

            StartCoroutine(Hit(hitDir));

            Debug.Log("HP(Melee) : " + curHealth);
        }
        else if (other.gameObject.CompareTag("Bullet"))
        {
            cBullet bullet = other.GetComponent<cBullet>();
            curHealth -= bullet.damage;

            Vector3 hitDir = KnockBack(other);

            StartCoroutine(Hit(hitDir));

            Destroy(other.gameObject);

            Debug.Log("HP(Bullet) : " + curHealth);
        }
    }

    IEnumerator Hit(Vector3 dir, bool isGrenade = false)
    {
        material.color = Color.red;
        yield return new WaitForSeconds(0.1f);

        if (!isGrenade)
            rigid.AddForce(dir.normalized * 5f, ForceMode.Impulse);
        else
            rigid.AddForce(dir.normalized * 10f, ForceMode.Impulse);

        if (curHealth > 0f)
            material.color = Color.white;
        else
        {
            material.color = Color.gray;
            gameObject.layer = 12;

            anim.SetTrigger("doDie");
            isChase = false;
            nav.enabled = false;

            Destroy(gameObject, 3f);
        }
    }

    Vector3 KnockBack(Collider other)
    {
        Vector3 dir = this.transform.position - other.transform.position;
        dir = dir.normalized;
        dir += Vector3.up;
        return dir;
    }

    public void HitByGrenade(Vector3 grenadeVec)
    {
        curHealth -= 100;
        Vector3 dir = this.transform.position - grenadeVec;
        StartCoroutine(Hit(dir, true));
    }

    void FreezeVelocity()
    {
        if (isChase)
        {
            rigid.velocity = Vector3.zero;
            rigid.angularVelocity = Vector3.zero;
        }
    }

    void Chase()
    {
        isChase = true;
        anim.SetBool("isWalk", isChase);
    }
}
