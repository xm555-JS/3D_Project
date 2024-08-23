using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class cMini_Enemy : MonoBehaviour
{
    [Header("[Info]")]
    [SerializeField] int maxHealth;
    [SerializeField] int curHealth;
    [SerializeField] float speed;
    bool isDead;

    MeshRenderer[] materials;
    List<MeshRenderer> materialList = new List<MeshRenderer>();
    Animator anim;

    public static event Action<cMini_Enemy> OnEnemyDeath;

    public int GetMaxHealth() { return maxHealth; }
    public int GetCurHealth() { return curHealth; }

    void Awake()
    {
        anim = GetComponentInChildren<Animator>();

        materials = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer material in materials)
        {
            if (material.gameObject.CompareTag("MiniMonster"))
                continue;

            materialList.Add(material);
        }
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

            if (!bullet.isMelee)
                Destroy(other.gameObject);
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
            Destroy(gameObject, 3f);
        }
    }
}
