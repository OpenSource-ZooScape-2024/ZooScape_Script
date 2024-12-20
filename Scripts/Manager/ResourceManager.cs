using UnityEngine;
using Object = UnityEngine.Object;
using System.Collections;

public class ResourceManager 
{
    public T Load<T>(string key) where T : Object
    {
        return Resources.Load<T>(key);
    }

    public GameObject Instantiate(string key, Vector3 pos)
    {
        GameObject prefab = Load<GameObject>($"Prefabs/{key}");

        if (prefab == null)
        {
            Debug.LogError($"Failed to load prefab : {key}");
            return null;
        }

        GameObject go = Managers.Pool.Pop(prefab, pos);
        return go;
    }

    public void Destroy(GameObject go)
    {
        if (go == null)
            return;

        if (Managers.Pool.Push(go))
            return;
    }
}
