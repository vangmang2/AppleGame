﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupGameOver : MonoBehaviour
{
    [SerializeField] Transform trApple;
    [SerializeField] Text txtScore;

    public PopupGameOver SetScore(int score)
    {
        txtScore.Set($"<size=60>Score</size>\n{score}");
        return this;
    }

    public void ShowPopup()
    {
        gameObject.SetActive(true);
    }

    public void OnClick_Replay()
    {

    }

    public void OnClick_ToMainMenu()
    {

    }
}