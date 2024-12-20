using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using Unity.Netcode;

class Pool
{
    GameObject _prefab;
    IObjectPool<GameObject> _pool;

    Transform _root;
    Vector3 _spawnPos;
    Transform Root
    {
        get
        {
            if (_root == null)
            {
                GameObject go = new GameObject() { name = $"@{_prefab.name}Pool" };
                _root = go.transform;
            }
            return _root;
        }
    }

    public Pool(GameObject prefab)
    {
        _prefab = prefab;
        _pool = new ObjectPool<GameObject>(OnCreate, OnGet, OnRelease, OnDestroy);
    }

    public void Push(GameObject go)
    {
        if (go.activeSelf)
            _pool.Release(go);
    }

    public GameObject Pop(Vector3 pos)
    {
        _spawnPos = pos;
        return _pool.Get();
    }

    #region Funcs
    GameObject OnCreate()
    {
        GameObject go = GameObject.Instantiate(_prefab, _spawnPos, Quaternion.identity);
        //go.transform.SetParent(Root);
        go.name = _prefab.name;
        return go;
    }

    void OnGet(GameObject go)
    {
        go.transform.position = _spawnPos;
        go.SetActive(true);
        NetworkObject networkObject = go.GetComponent<NetworkObject>();
        networkObject.Spawn();
    }

    void OnRelease(GameObject go)
    {
        NetworkObject networkObject = go.GetComponent<NetworkObject>();
        networkObject.Despawn(false);
        go.SetActive(false);
    }

    void OnDestroy(GameObject go)
    {
        NetworkObject networkObject = go.GetComponent<NetworkObject>();
        networkObject.Despawn(true);
        GameObject.Destroy(go);
    }
    #endregion
}

public class PoolManager
{
    Dictionary<string, Pool> _pools = new Dictionary<string, Pool>();

    public GameObject Pop(GameObject prefab, Vector3 pos)
    {
        if (_pools.ContainsKey(prefab.name) == false)
            CreatePool(prefab);

        return _pools[prefab.name].Pop(pos);
    }

    public bool Push(GameObject go)
    {
        if (_pools.ContainsKey(go.name) == false)
            return false;

        _pools[go.name].Push(go);
        return true;
    }

    public void Clear()
    {
        _pools.Clear();
    }

    void CreatePool(GameObject original)
    {
        Pool pool = new Pool(original);
        _pools.Add(original.name, pool);
    }
}

