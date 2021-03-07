using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Object = UnityEngine.Object;

public class UnityObjectPool : MonoBehaviour
{
    [SerializeField] GameObject prefab;

    readonly Stack<GameObject> itemStack = new Stack<GameObject>();
    [SerializeField] int spawnCount;

    private GameObject CreateItem()
    {
        var item = Instantiate(prefab, transform);
        return item;
    }

    public GameObject Spawn<T>(ref T instance, Transform parent) where T : Object, IPoolable<T>
    {
        GameObject item = null;
        if(itemStack.Count > 0)
            item = itemStack.Pop();
        else        
            item = CreateItem();

        item.transform.SetParent(parent);
        item.SetActive(true);
        T itemInstance = null;
        Action<T> callback = (_instance) => itemInstance = _instance;
        item.SendMessage("GetInstance", callback, SendMessageOptions.DontRequireReceiver);
        instance = itemInstance;
        spawnCount++;
        return item;
    }

    public void Despawn<T>(T item) where T : Object, IPoolable<T>
    {
        var _item = item.getGameObject;
        itemStack.Push(_item);
        _item.transform.SetParent(transform);
        spawnCount--;
        _item.SetActive(false);
    }
}

public interface IPoolable<T> where T : Object
{
    GameObject getGameObject { get; } 
    void GetInstance(Action<T> callback);
}
