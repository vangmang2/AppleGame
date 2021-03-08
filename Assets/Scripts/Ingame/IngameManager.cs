using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// 게임의 전반적인 흐름, 유틸리티를 담당한다.
public class IngameManager : MonoBehaviour
{
    public void OnClick_ToMainMenu()
    {
        SceneManager.LoadScene(0/*OutgameScene*/);
    }
}
