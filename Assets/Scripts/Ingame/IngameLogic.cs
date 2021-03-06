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
    const float LIMIT_TIME = 60;

    [SerializeField] IngameInputHandler ingameInputHandler;
    [SerializeField] UIItemProgressBar leftTimeProgressBar;
    [SerializeField] Transform trStage;
    [SerializeField] UIItemApple applePrefab;
    [SerializeField] int x, y;
    [SerializeField] float space;
    [SerializeField] Text txtScore;
    [SerializeField] PopupGameOver popupPause;
    int score;
    UIItemApple[,] appleArray;

    void Start()
    {
        isGameOver = false;
        ingameInputHandler.SetCheckSumAction((selectedAppleList) =>
        {
            int sum = selectedAppleList.Sum(apple => apple.number);
            if(sum == SATISFY_TOTAL_SUM)
            {
                selectedAppleList.ForEach(apple => apple.Terminate());
                score += selectedAppleList.Count;
                if (selectedAppleList.Count > 2)
                    score += 10;
                txtScore.Set(score);
            }
        });
        GenerateApples();
    }

    private void GenerateApples()
    {
        int count = 0;
        var offSet = new Vector2(x, -y) * 0.5f * space;
        appleArray = new UIItemApple[x, y];

        for (int _y = 0; _y < y; _y++)
        {
            for (int _x = 0; _x < x; _x++)
            {
                var itemApple = Instantiate(applePrefab, trStage);
                itemApple.SetID(++count)
                         .SetLocalPosition(new Vector2(_x + 0.5f, -_y - 0.5f) * space - offSet)
                         .SetNumber(Random.Range(1, 10));
                appleArray[_x, _y] = itemApple;
            }
        }
    }

    public void FindAvailableTotal()
    {
        for (int _y = 0; _y < y; _y++)
        {
            for (int _x = 0; _x < x; _x++)
            {
                var itemApple = appleArray[_x, _y];
            }
        }
    }

    float passedTime = 0f;
    void Update()
    {
        passedTime += Time.deltaTime;
        float time = Mathf.Max(LIMIT_TIME - passedTime, 0f);
        leftTimeProgressBar.SetFill(time / LIMIT_TIME);

        if(time <= 0f && !isGameOver)
        {
            isGameOver = true;
            popupPause.SetScore(score)
                      .ShowPopup();
        }
    }


}
