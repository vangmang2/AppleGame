using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Object = UnityEngine.Object;

public class UnityObjectPool : MonoBehaviour
{
    [SerializeField] GameObject prefab;

    readonly Stack<GameObject> itemStack = new Stack<GameObject>();
    [SerializeField] int spawnedCount, despawnedCount;

    private GameObject CreateItem()
    {
        var item = Instantiate(prefab, transform);
        return item;
    }

    public GameObject Spawn<T>(ref T instance) where T : Object, IPoolable<T>
    {
        GameObject item = null;
        if(itemStack.Count > 0)
            item = itemStack.Pop();
        else        
            item = CreateItem();

        item.transform.SetParent(transform.root);
        item.SetActive(true);
        T itemInstance = null;
        Action<T> callback = (_instance) => itemInstance = _instance;
        item.SendMessage("GetInstance", callback, SendMessageOptions.DontRequireReceiver);
        instance = itemInstance;
        spawnedCount++;
        return item;
    }

    public void Despawn(GameObject item)
    {
        itemStack.Push(item);
        item.transform.SetParent(transform);
        despawnedCount++;
        item.SetActive(false);
    }

    //List<GameObject> appleList = new List<GameObject>();
    //void Update()
    //{
    //    InputHandler.HandleKeyboardInput(KeyCode.Q, KeyInput.keyDown, () =>
    //    {
    //        UIItemApple apple = null;
    //        appleList.Add(Spawn(ref apple));
    //    });
    //    InputHandler.HandleKeyboardInput(KeyCode.W, KeyInput.keyDown, () =>
    //    {
    //        if (appleList.Count > 0)
    //        {
    //            Despawn(appleList[despawnedCount]);
    //        }
    //    });
    //}
}

public interface IPoolable<T> where T : Object
{
    void GetInstance(Action<T> callback);
}
