﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using UnityEngine.UI;
using Random = UnityEngine.Random;

// 게임의 매커니즘, 시스템을 담당한다.
public class IngameLogic : MonoBehaviour
{
    public static bool isGameOver = false;
    const int SATISFY_TOTAL_SUM = 10;
    const float LIMIT_TIME = 60f;

    [SerializeField] ComboSystem comboSystem;
    [SerializeField] UnityObjectPool applePool;
    [SerializeField] GraphicRaycaster graphicRaycaster;
    [SerializeField] IngameInputHandler ingameInputHandler;
    [SerializeField] UIItemProgressBar leftTimeProgressBar;
    [SerializeField] Transform trStage, trTerminatedApplesParent;
    [SerializeField] Text txtScore;
    [SerializeField] PopupGameOver popupPause;
    [SerializeField] RectTransform rtHint, rtStage;
    [SerializeField] int x, y;

    readonly List<UIItemApple> avaliableAppleList = new List<UIItemApple>();

    int score;
    UIItemApple[,] appleArray;
    float space;
    float cellSize;
    Vector2 offSet => new Vector2(x, -y) * 0.5f * space;

    void Start()
    {
        isGameOver = false;
        var stageSize = new Vector2(Screen.width, Screen.height) + rtStage.sizeDelta;
        cellSize = Mathf.Min(stageSize.x / x, stageSize.y / y) - 10f;
        space = cellSize + 10;

        ingameInputHandler.SetCheckSumAction((selectedAppleList) =>
        {
            int sum = selectedAppleList.Sum(apple => apple.number);
            if (sum == SATISFY_TOTAL_SUM)
            {
                StartCoroutine(CoRaycastAfterHandleInput());
                selectedAppleList.ForEach(apple =>
                {
                    apple.SetParent(trTerminatedApplesParent);
                    apple.Terminate(() => applePool.Despawn(apple));
                });
                score += selectedAppleList.Count;
                if (selectedAppleList.Count > 2)
                    score += 10;
                txtScore.Set(score);
                comboSystem.ActivateCombo();
                // 중력 모드
                ActivateGravityFall(selectedAppleList);
            }
        });
        GenerateApples();
    }

    private IEnumerator CoRaycastAfterHandleInput()
    {
        graphicRaycaster.enabled = false;
        yield return CommonUtility.GetYieldSec(UIItemApple.APPLE_MOVEMENT_TIME + 0.2f);
        graphicRaycaster.enabled = true;
    }

    readonly List<UIItemApple> aboutToFallAppleList = new List<UIItemApple>();
    private void ActivateGravityFall(List<UIItemApple> terminatedApples)
    {
        var orderedAppleList = terminatedApples.ToList();
        orderedAppleList.Sort((apple1, apple2) =>
        {
            if (apple1.index.y < apple2.index.y)
                return -1;
            else if (apple1.index.y > apple2.index.y)
                return 1;
            else
            {
                if (apple1.index.x < apple2.index.x)
                    return -1;
                else
                    return 1;
            }
        });

        var firstApple = orderedAppleList[0];
        var lastApple = orderedAppleList[terminatedApples.Count - 1];

        int xAxisLength = Mathf.Abs(firstApple.index.x - lastApple.index.x) + 1;
        int yAxisLength = Mathf.Abs(firstApple.index.y - lastApple.index.y) + 1;
        var x = 0;
        var y = 0;

        orderedAppleList.ForEach(apple =>
        {
            UIItemApple itemApple = null;
            applePool.Spawn(ref itemApple, trStage);

            var index = apple.index;

            for (int _y = index.y - yAxisLength; _y >= 0; _y--)
            {
                var targetApple = appleArray[index.x, _y];
                if (!aboutToFallAppleList.Contains(targetApple))
                    aboutToFallAppleList.Add(targetApple);
            }
            if(xAxisLength > 1 && yAxisLength > 1)
            {
                if (x % xAxisLength == 0)
                    y--;
                x++;
            }
            else if (y > -yAxisLength)
                y--;
            index.y = y;
            itemApple.SetIndex(index)
                     .SetNumber(Random.Range(1, 10))
                     .SetSize(new Vector2(cellSize, cellSize))
                     .SetLocalPosition(new Vector2(index.x + 0.5f, -index.y - 0.5f) * space - offSet)
                     .SetLocalRotation(Quaternion.identity);
            aboutToFallAppleList.Add(itemApple);
        });

        aboutToFallAppleList.ForEach(apple =>
        {
            var currentIndex = apple.index;
            var targetIndex = apple.index;
            targetIndex.y += yAxisLength;
            var targetPos = new Vector2(targetIndex.x + 0.5f, -targetIndex.y - 0.5f) * space - offSet;
            apple.SetIndex(targetIndex)
                 .SetName($"Apple[{targetIndex.x}, {targetIndex.y}]")
                 .MoveToTarget(targetPos);
            appleArray[targetIndex.x, targetIndex.y] = apple;
        });
        aboutToFallAppleList.Clear();
    }

    private void GenerateApples()
    {
        appleArray = new UIItemApple[x, y];

        for (int _y = 0; _y < y; _y++)
        {
            for (int _x = 0; _x < x; _x++)
            {
                UIItemApple itemApple = null;
                applePool.Spawn(ref itemApple, trStage);
                itemApple.SetIndex(new Index(_x, _y))
                         .SetName($"Apple[{_x}, {_y}]")
                         .SetNumber(Random.Range(1, 10))
                         .SetSize(new Vector2(cellSize, cellSize))
                         .SetLocalPosition(new Vector2(_x + 0.5f, -_y - 0.5f) * space - offSet)
                         .SetLocalRotation(Quaternion.identity);
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

            while (idx > minY)
            {
                idx--;
                var sum = 0;
                for (int i = y; i >= idx; i--)
                {
                    var apple = appleArray[x, i];
                    if (!apple.isTerminated)
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

            while (idx > minX)
            {
                idx--;
                var sum = 0;
                for (int i = x; i >= idx; i--)
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
        var lastApple = avaliableAppleList[avaliableAppleList.Count - 1];

        int xAxisLength = Mathf.Abs(firstApple.index.x - lastApple.index.x) + 1;
        int yAxisLength = Mathf.Abs(firstApple.index.y - lastApple.index.y) + 1;
        bool isVertical = yAxisLength > 1;

        var centerPos = firstApple.getLocalPosition + lastApple.getLocalPosition;
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

        if (time <= 0f && !isGameOver)
        {
            isGameOver = true;
            popupPause.SetScore(score)
                      .ShowPopup();
        }

        //InputHandler.HandleKeyboardInput(KeyCode.Q, KeyInput.keyDown, Onclick_FindAvailableTotal);
    }
}
