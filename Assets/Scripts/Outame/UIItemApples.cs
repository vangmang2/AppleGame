using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIItemApples : MonoBehaviour
{
    [SerializeField] List<GameObject> appleList;

    public void EnableApples(int count)
    {
        for(int i = 0; i < appleList.Count; i++)
        {
            appleList[i].SetActive(i < count);
        }
    }
}
