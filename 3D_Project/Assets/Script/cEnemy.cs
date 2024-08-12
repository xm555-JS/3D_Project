using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class cEnemy : MonoBehaviour
{
    public GameManager manager;

    public int score;
    public GameObject[] coins;

    public int maxHealth;
    public int curHealth;
    protected float radius = 1.5f;
    protected float range = 2f;
    protected Transform target;
    protected bool isChase;
    protected bool isAttack;
    protected bool isDead;

    protected Rigidbody rigid;
    protected BoxCollider boxCollider;
    protected MeshRenderer[] materials;
    protected List<MeshRenderer> materialList = new List<MeshRenderer>();
    protected NavMeshAgent nav;
    protected Animator anim;

    protected void Awake()
    {
        Initialize();

        Invoke("Chase", 2f);
    }

    protected void Initialize()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();

        materials = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer material in materials)
        {
            if (material.gameObject.CompareTag("MiniMonster"))
                continue;

            materialList.Add(material);
        }

        nav = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
    }

    protected void Start()
    {
        GameObject player = GameObject.FindWithTag("Player");
        target = player.GetComponent<Transform>();
    }

    protected virtual void Update()
    {
        if (isDead)
        {
            StopAllCoroutines();
            return;
        }

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
        if (isDead)
            return;

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
        foreach (MeshRenderer mesh in materialList)
            mesh.material.color = Color.red;

        yield return new WaitForSeconds(0.1f);

        if (!isGrenade)
            rigid.AddForce(dir.normalized * 5f, ForceMode.Impulse);
        else
            rigid.AddForce(dir.normalized * 10f, ForceMode.Impulse);

        if (curHealth > 0f)
        {
            foreach (MeshRenderer mesh in materialList)
                mesh.material.color = Color.white;
        }
        else
        {
            foreach (MeshRenderer mesh in materialList)
                mesh.material.color = Color.gray;

            gameObject.layer = 12;
            isDead = true;
            DeadCount();

            anim.SetTrigger("doDie");
            isChase = false;
            nav.enabled = false;

            Player player = target.GetComponent<Player>();
            player.score += score;
            int randCoin = Random.Range(0, 3);
            Instantiate(coins[randCoin], transform.position, Quaternion.identity);

            Destroy(gameObject, 3f);
        }
    }

    protected abstract void DeadCount();

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
