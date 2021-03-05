using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class IngameLogic : MonoBehaviour
{
    public static bool isGameOver = false;
    const int SATISFY_TOTAL_SUM = 10;
    const float LIMIT_TIME = 5;
    [SerializeField] IngameInputHandler ingameInputHandler;
    [SerializeField] UIItemProgressBar leftTimeProgressBar;
    [SerializeField] Transform trStage;
    [SerializeField] UIItemApple applePrefab;
    [SerializeField] int x, y;
    [SerializeField] float space;
    [SerializeField] Text txtScore;
    [SerializeField] PopupGameOver popupPause;
    int score;

    void Start()
    {
        ingameInputHandler.SetCheckSumAction((selectedAppleList) =>
        {
            int sum = selectedAppleList.Sum(apple => apple.number);
            if(sum == SATISFY_TOTAL_SUM)
            {
                selectedAppleList.ForEach(apple => apple.gameObject.SetActive(false));
                score += selectedAppleList.Count;
                if (selectedAppleList.Count > 2)
                    score += 10;
                txtScore.text = score.ToString();
            }
        });
        GenerateApples();
    }

    private void GenerateApples()
    {
        int count = 0;
        var offSet = new Vector2(x, y) * 0.5f * space;
        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < y; j++)
            {
                var itemApple = Instantiate(applePrefab, trStage);
                itemApple.SetID(++count)
                         .SetLocalPosition(offSet - new Vector2(i + 0.5f, j + 0.5f) * space)
                         .SetNumber(Random.Range(1, 10));
            }
        }
    }

    void Update()
    {
        float time = Mathf.Max(LIMIT_TIME - Time.realtimeSinceStartup, 0f);
        leftTimeProgressBar.SetFill(time / LIMIT_TIME);

        if(time <= 0f && !isGameOver)
        {
            isGameOver = true;
            popupPause.SetScore(score)
                      .ShowPopup();
        }
    }


}
