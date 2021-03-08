using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.UI;

public class PopupGameOver : MonoBehaviour
{
    [SerializeField] Transform trApple;
    [SerializeField] Text txtScore;

    public PopupGameOver SetScore(int score)
    {
        txtScore.Set($"<size=35>Score</size>\n<size=55>{score}</size>");
        return this;
    }

    public void ShowPopup()
    {
        gameObject.SetActive(true);
    }

    public void OnClick_Replay()
    {
        SceneManager.LoadScene(1/*IngameScene*/);
    }

    public void OnClick_ToMainMenu()
    {
        SceneManager.LoadScene(0/*OutgameScene*/);
    }
}
