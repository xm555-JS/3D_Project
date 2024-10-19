using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class cObjectPool : MonoBehaviour
{
    public int maxPoolSize = 10;
    public int stackDefaultCapacity = 10;

    GameObject poolPrefab;

    public enum poolingType { DEFAULT, ENEMYA, ENEMYB, ENEMYC };
    public poolingType poolType;

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
        GameObject instantEnemy = Instantiate(poolPrefab);
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

    public GameObject Spawn(int type)
    {
        switch (type)
        {
            case 0:
                poolPrefab = Resources.Load<GameObject>("Prefabs/Enemy A");
                break;
            case 1:
                poolPrefab = Resources.Load<GameObject>("Prefabs/Enemy A");
                break;
            case 2:
                poolPrefab = Resources.Load<GameObject>("Prefabs/Enemy B");
                break;
            case 3:
                poolPrefab = Resources.Load<GameObject>("Prefabs/Enemy C");
                break;
        }

        var poolObject = Pool.Get();
        return poolObject;
    }
}
