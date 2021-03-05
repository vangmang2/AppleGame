using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class OutgameManager : MonoBehaviour
{

    public void OnClick_ToIngame()
    {
        SceneManager.LoadScene(1/*IngameScene*/);
    }
}
