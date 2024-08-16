using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cMini_Enemy : MonoBehaviour
{
    protected MeshRenderer[] materials;
    protected List<MeshRenderer> materialList = new List<MeshRenderer>();

    public int maxHealth;
    public int curHealth;
    public float speed;
    bool isDead;

    void Awake()
    {
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

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Bullet"))
        {
            cBullet bullet = other.GetComponent<cBullet>();
            curHealth -= bullet.damage;

            StartCoroutine(Hit());

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
            Destroy(gameObject, 3f);
        }
    }
}
