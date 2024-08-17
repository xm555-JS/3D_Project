using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cMini_Enemy : MonoBehaviour
{
    [Header("[Info]")]
    [SerializeField] int maxHealth;
    [SerializeField] int curHealth;
    [SerializeField] float speed;
    bool isDead;
    public static int enemyCount;

    MeshRenderer[] materials;
    List<MeshRenderer> materialList = new List<MeshRenderer>();
    Animator anim;

    static MiniGameManager miniGameManager;

    void Awake()
    {
        enemyCount++;
        CheckGameOver();

        Debug.Log(enemyCount);
        GameObject manager = GameObject.FindGameObjectWithTag("MiniGameManager");
        miniGameManager = manager.GetComponent<MiniGameManager>();

        anim = GetComponentInChildren<Animator>();

        materials = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer material in materials)
        {
            if (material.gameObject.CompareTag("MiniMonster"))
                continue;

            materialList.Add(material);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead)
            return;

        transform.position += transform.forward * Time.deltaTime * speed;
    }

    void CheckGameOver()
    {
        if (enemyCount >= 60)
            miniGameManager.MiniGameOver();
    }

    void OnTriggerEnter(Collider other)
    {
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
            foreach (MeshRenderer mesh in materialList)
                mesh.material.color = Color.gray;

            gameObject.layer = 12;
            isDead = true;
            enemyCount--;
            anim.SetTrigger("doDie");
            Destroy(gameObject, 3f);

            miniGameManager.GetCoin(100);
        }
    }
}
