using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ObjectPooler : MonoBehaviour
{
    private Dictionary<GameObject, ObjectPool<GameObject>> pools = new();

    public static ObjectPooler Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null)
            Destroy(this);
        else
            Instance = this;
    }

    public void CreatePool(GameObject objectToPool, bool collectionCheck = true, int defaultCapacity = 10, int maxSize = 10000)
    {
        if (pools.ContainsKey(objectToPool))
        {
            Debug.LogWarning($"Object pooler has already catalogued {objectToPool.name}. Will skip.");
            return;
        }

        ObjectPool<GameObject> newPool = new(
            () =>
            {
                return TomadleUtils.InstantiateDisabled(objectToPool);
            },
            gottenObject =>
            {
                //normally we get a disabled object with InstantiateDisabled
            },
            releasedObject =>
            {
                releasedObject.SetActive(false);
            },
            objectToDestroy =>
            {
                Destroy(objectToDestroy);
            },
            collectionCheck, defaultCapacity, maxSize);

        pools.Add(objectToPool, newPool);
    }

    public GameObject GetPooledObject(GameObject sample)
    {
        GameObject pooledObject = pools[sample].Get();
        if(pooledObject.TryGetComponent(out IPoolReleasable releasable))
        {
            releasable.releaseFunction = () => ReleasePooledObject(sample, pooledObject);
        }

        return pooledObject;
    }

    public void ReleasePooledObject(GameObject objectKey, GameObject objectToRelease)
    {
        pools[objectKey].Release(objectToRelease);
    }
}
