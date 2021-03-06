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
    const float LIMIT_TIME = 6000;

    [SerializeField] IngameInputHandler ingameInputHandler;
    [SerializeField] UIItemProgressBar leftTimeProgressBar;
    [SerializeField] Transform trStage;
    [SerializeField] UIItemApple applePrefab;
    [SerializeField] int x, y;
    [SerializeField] float space;
    [SerializeField] Text txtScore;
    [SerializeField] PopupGameOver popupPause;
    [SerializeField] RectTransform rtHint;
    int score;
    UIItemApple[,] appleArray;
    readonly List<UIItemApple> avaliableAppleList = new List<UIItemApple>();


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
        var offSet = new Vector2(x, -y) * 0.5f * space;
        appleArray = new UIItemApple[x, y];

        for (int _y = 0; _y < y; _y++)
        {
            for (int _x = 0; _x < x; _x++)
            {
                var itemApple = Instantiate(applePrefab, trStage);
                itemApple.SetIndex(new Vector2(_x, _y))
                         .SetName($"Apple[{_x}, {_y}]")
                         .SetNumber(Random.Range(1, 10))
                         .SetLocalPosition(new Vector2(_x + 0.5f, -_y - 0.5f) * space - offSet);
                appleArray[_x, _y] = itemApple;
            }
        }
    }

    public void Onclick_FindAvailableTotal()
    {
        avaliableAppleList.Clear();
        for (int _y = 0; _y < y; _y++)
        {
            for (int _x = 0; _x < x; _x++)
            {
                if (SearchVertical(_x, _y))
                {
                    HighlightAvaliableApples();
                    return;
                }
                if (SearchHorizontal(_x, _y))
                {
                    HighlightAvaliableApples();
                    return;
                }
            }
        }

        bool SearchVertical(int x, int y)
        {
            int minY = 0;
            if (y == minY)
                return false;
            int idx = y;

            while(idx > minY)
            {
                idx--;
                var sum = 0;
                for (int i = y; i >= idx; i--)
                {
                    var apple = appleArray[x, i];
                    if(!apple.isTerminated)
                        sum += apple.number;
                }
                if (sum == 10)
                {
                    for (int i = y; i >= idx; i--)
                        avaliableAppleList.Add(appleArray[x, i]);
                    return true;
                }
                else if (sum > 10)
                    return false;
            }
            return false;
        }

        bool SearchHorizontal(int x, int y)
        {
            int minX = 0;
            if (x == minX)
                return false;
            int idx = x;

            while(idx > minX)
            {
                idx--;
                var sum = 0;
                for(int i = x; i >= idx; i--)
                {
                    var apple = appleArray[i, y];
                    if (!apple.isTerminated)
                        sum += apple.number;
                }
                if (sum == 10)
                {
                    for (int i = x; i >= idx; i--)
                        avaliableAppleList.Add(appleArray[i, y]);
                    return true;
                }
                else if (sum > 10)
                    return false;
            }
            return false;
        }
    }

    Coroutine coroutine;
    // 10의 합이 가능한 사과들을 하이라이트 효과처리함
    private void HighlightAvaliableApples()
    {
        if (coroutine != null)
            StopCoroutine(coroutine);
        coroutine = StartCoroutine(CoPlayHighlightHint());

        var firstApple = avaliableAppleList[0];
        var secondApple = avaliableAppleList[avaliableAppleList.Count - 1];

        int xAxisLength = (int)Mathf.Abs(firstApple.index.x - secondApple.index.x) + 1;
        int yAxisLength = (int)Mathf.Abs(firstApple.index.y - secondApple.index.y) + 1;
        bool isVertical = yAxisLength > 1;

        var centerPos = firstApple.getLocalPosition + secondApple.getLocalPosition;
        centerPos /= 2;

        int count = (isVertical ? yAxisLength : xAxisLength);
        rtHint.sizeDelta = isVertical ? 
            new Vector2(space, space * count) : 
            new Vector2(space * count, space);
        rtHint.anchoredPosition = centerPos;
        avaliableAppleList.Clear();
    }

    private IEnumerator CoPlayHighlightHint()
    {
        rtHint.gameObject.SetActive(true);
        var t = 0f;
        var duration = 2f;

        while (t <= duration)
        {
            t += Time.deltaTime;
            yield return null;
        }
        rtHint.gameObject.SetActive(false);
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

        //InputHandler.HandleKeyboardInput(KeyCode.Q, KeyInput.keyDown, Onclick_FindAvailableTotal);
    }


}
