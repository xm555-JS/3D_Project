using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class cObjectPool : MonoBehaviour
{
    public int maxPoolSize = 10;
    public int stackDefaultCapacity = 10;

    public IObjectPool<GameObject> Pool
    {
        get
        {
            if (_pool == null)
                _pool = new ObjectPool<GameObject>(CreatedPooledItem,
                                                    OnTakeFromPool,
                                                    OnReturnedToPool,
                                                    OnDestroyPoolObject,
                                                    true,
                                                    stackDefaultCapacity,
                                                    maxPoolSize);

            return _pool;
        }
    }

    IObjectPool<GameObject> _pool;

    GameObject CreatedPooledItem()
    {
        GameObject enemy = Resources.Load<GameObject>("Prefabs/Enemy A");
        GameObject instantEnemy = Instantiate(enemy);
        cMini_Enemy obj = instantEnemy.GetComponent<cMini_Enemy>();
        obj.Pool = Pool;

        return obj.gameObject;
    }

    void OnReturnedToPool(GameObject poolObject)
    {
        poolObject.SetActive(false);
    }

    void OnTakeFromPool(GameObject poolObject)
    {
        poolObject.SetActive(true);
    }

    void OnDestroyPoolObject(GameObject poolObject)
    {
        Destroy(poolObject);
    }

    public GameObject Spawn()
    {
        var poolObject = Pool.Get();
        return poolObject;
    }
}
