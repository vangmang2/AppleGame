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
        int gamePlayTime = PlayerPrefs.GetInt("GamePlayTime", 0);
        gamePlayTime++;
        PlayerPrefs.SetInt("GamePlayTime", gamePlayTime);
        if (gamePlayTime == 5)
            Achievement.Achieve_GamePlayTime5((key) => IngameManager.instance.ShowAchievement(key));
        gameObject.SetActive(true);
    }

    public void OnClick_Replay()
    {
        if (StaticCanvas.instance.CanPlay())
        {
            StaticCanvas.instance.UseAppleToPlay();
            SceneManager.LoadScene(1/*IngameScene*/);
        }
    }

    public void OnClick_ToMainMenu()
    {
        SceneManager.LoadScene(0/*OutgameScene*/);
    }
}
