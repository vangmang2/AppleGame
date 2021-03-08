using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;

// 게임의 전반적인 흐름, 유틸리티를 담당한다.
public class IngameManager : MonoBehaviour
{
    public static IngameManager instance { get; private set; }

    [SerializeField] UIItemAchievement itemAchievement;

    void Awake()
    {
        instance = this;
    }

    public void OnClick_ToMainMenu()
    {
        SceneManager.LoadScene(0/*OutgameScene*/);
    }

    void Update()
    {
        //TEST
        InputHandler.HandleKeyboardInput(KeyCode.Q, KeyInput.keyDown, PlayerPrefs.DeleteAll);
    }

    public void ShowAchievement(string key)
    {
        itemAchievement.ShowAchievementTitle($"업적 {key} 달성!");
    }
}

public static class Achievement
{
    public const string KEY_MAKECOMBO_5 = "MakeCombo5";
    public const string KEY_MAKECOMBO_10 = "MakeCombo10";
    public const string KEY_GAMEPLAY_TIME_5 = "GamePlayTime5";

    // 콤보 5회 이상 달성
    public static void Achieve_MakeCombo5(Action<string> callback)
    {
        if (PlayerPrefs.HasKey(KEY_MAKECOMBO_5))
            return;
        PlayerPrefs.SetInt(KEY_MAKECOMBO_5, 1);
        callback(KEY_MAKECOMBO_5);
    }

    // 콤보 10회 이상 달성
    public static void Achieve_MakeCombo10(Action<string> callback)
    {
        if (PlayerPrefs.HasKey(KEY_MAKECOMBO_10))
            return;
        PlayerPrefs.SetInt(KEY_MAKECOMBO_10, 1);
        callback(KEY_MAKECOMBO_10);
    }

    // 게임 플레이 5회 이상 달성 (게임을 플레이하고 게임 오버 팝업이 뜰 때 조건 체크)
    public static void Achieve_GamePlayTime5(Action<string> callback)
    {
        if (PlayerPrefs.HasKey(KEY_GAMEPLAY_TIME_5))
            return;
        PlayerPrefs.SetInt(KEY_GAMEPLAY_TIME_5, 1);
        callback(KEY_GAMEPLAY_TIME_5);
    }
}