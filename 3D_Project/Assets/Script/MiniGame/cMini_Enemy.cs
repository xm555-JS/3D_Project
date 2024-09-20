using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Pool;

public class cMini_Enemy : MonoBehaviour
{
    [Header("[Info]")]
    [SerializeField] int maxHealth;
    [SerializeField] float speed;
    int curHealth;
    bool isDead;

    public IObjectPool<GameObject> Pool { get; set; }

    MeshRenderer[] materials;
    List<MeshRenderer> materialList = new List<MeshRenderer>();
    Animator anim;

    public static event Action<cMini_Enemy> OnEnemyDeath;

    public int GetMaxHealth() { return maxHealth; }
    public int GetCurHealth() { return curHealth; }

    void Awake()
    {
        curHealth = maxHealth;

        anim = GetComponentInChildren<Animator>();

        materials = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer material in materials)
        {
            if (material.gameObject.CompareTag("MiniMonster"))
                continue;

            materialList.Add(material);
        }
    }

    void OnDisable()
    {
        ResetEnemy();
    }

    void Update()
    {
        if (isDead)
            return;

        transform.position += transform.forward * Time.deltaTime * speed;
    }

    void OnTriggerEnter(Collider other)
    {
        if (isDead)
            return;

        if (other.gameObject.CompareTag("Bullet"))
        {
            cBullet bullet = other.GetComponent<cBullet>();
            curHealth -= bullet.damage;

            StartCoroutine(Hit());

            //if (!bullet.isMelee)
            //    ReturnToPool();
        }
    }

    IEnumerator Hit()
    {
        foreach (MeshRenderer mesh in materialList)
            mesh.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);

        if (curHealth > 0f)
        {
            foreach (MeshRenderer mesh in materialList)
                mesh.material.color = Color.white;
        }
        else
        {
            if (isDead)
                yield break;

            foreach (MeshRenderer mesh in materialList)
                mesh.material.color = Color.gray;

            gameObject.layer = 12;
            isDead = true;
            anim.SetTrigger("doDie");
            OnEnemyDeath?.Invoke(this);
            //ReturnToPool();//Destroy(gameObject, 3f);
            Invoke("ReturnToPool", 3f);
        }
    }

    void ReturnToPool()
    {
        Pool.Release(this.gameObject);
    }

    void ResetEnemy()
    {
        // hp
        curHealth = maxHealth;
        // layer
        gameObject.layer = 11;
        // color
        foreach (MeshRenderer mesh in materialList)
            mesh.material.color = Color.white;
        // isDead
        isDead = false;
        // animation?
    }
}
