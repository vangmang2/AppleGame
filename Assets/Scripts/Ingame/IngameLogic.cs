using System.Collections;
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

    int mScore;
    UIItemApple[,] mAppleArray;
    float mSpace;
    float mCellSize;
    Vector2 offSet => new Vector2(x, -y) * 0.5f * mSpace;

    void Start()
    {
        isGameOver = false;
        var stageSize = new Vector2(Screen.width, Screen.height) + rtStage.sizeDelta;
        mCellSize = Mathf.Min(stageSize.x / x, stageSize.y / y) - 10f;
        mSpace = mCellSize + 10;

        ingameInputHandler.SetCheckSumAction((selectedAppleList) =>
        {
            int sum = selectedAppleList.Sum(apple => apple.number);
            if (sum == SATISFY_TOTAL_SUM)
            {
                graphicRaycaster.enabled = false;
                selectedAppleList.ForEach(apple =>
                {
                    apple.SetParent(trTerminatedApplesParent)
                         .Terminate(() => applePool.Despawn(apple));
                });
                mScore += selectedAppleList.Count;
                if (selectedAppleList.Count > 2)
                    mScore += 10;
                txtScore.Set(mScore);
                comboSystem.ActivateCombo();
                // 중력 모드
                ActivateGravityFall(selectedAppleList, () =>
                {
                    CheckTotalAvaliable(() => 
                    { 
                        graphicRaycaster.enabled = true; 
                    });
                });
            }
        });
        GenerateApples();
    }

    readonly List<UIItemApple> aboutToFallAppleList = new List<UIItemApple>();
    private void ActivateGravityFall(List<UIItemApple> terminatedApples, Action finishCallback/*사과가 다 내려온 후에 검사 시작*/)
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
                var targetApple = mAppleArray[index.x, _y];
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
                     .SetSize(new Vector2(mCellSize, mCellSize))
                     .SetLocalPosition(new Vector2(index.x + 0.5f, -index.y - 0.5f) * mSpace - offSet)
                     .SetLocalRotation(Quaternion.identity);
            aboutToFallAppleList.Add(itemApple);
        });

        aboutToFallAppleList.ForEach(apple =>
        {
            var targetIndex = apple.index;
            targetIndex.y += yAxisLength;
            var targetPos = new Vector2(targetIndex.x + 0.5f, -targetIndex.y - 0.5f) * mSpace - offSet;
            apple.SetIndex(targetIndex)
                 .SetName($"Apple[{targetIndex.x}, {targetIndex.y}]")
                 .MoveToTarget(targetPos);
            mAppleArray[targetIndex.x, targetIndex.y] = apple;
        });
        aboutToFallAppleList.Clear();
        finishCallback();
    }

    private void GenerateApples()
    {
        mAppleArray = new UIItemApple[x, y];

        for (int _y = 0; _y < y; _y++)
        {
            for (int _x = 0; _x < x; _x++)
            {
                UIItemApple itemApple = null;
                applePool.Spawn(ref itemApple, trStage);
                itemApple.SetIndex(new Index(_x, _y))
                         .SetName($"Apple[{_x}, {_y}]")
                         .SetNumber(Random.Range(1, 10))
                         .SetSize(new Vector2(mCellSize, mCellSize))
                         .SetLocalPosition(new Vector2(_x + 0.5f, -_y - 0.5f) * mSpace - offSet)
                         .SetLocalRotation(Quaternion.identity);
                mAppleArray[_x, _y] = itemApple;
            }
        }
    }

    private bool FindAvailableTotal(Action callback = null)
    {
        avaliableAppleList.Clear();
        for (int _y = 0; _y < y; _y++)
        {
            for (int _x = 0; _x < x; _x++)
            {
                if (SearchVertical(_x, _y))
                {
                    callback?.Invoke();
                    return true;
                }
                if (SearchHorizontal(_x, _y))
                {
                    callback?.Invoke();
                    return true;
                }
            }
        }
        return false;

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
                    var apple = mAppleArray[x, i];
                    if (!apple.isTerminated)
                        sum += apple.number;
                }
                if (sum == 10)
                {
                    for (int i = y; i >= idx; i--)
                        avaliableAppleList.Add(mAppleArray[x, i]);
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
                    var apple = mAppleArray[i, y];
                    if (!apple.isTerminated)
                        sum += apple.number;
                }
                if (sum == 10)
                {
                    for (int i = x; i >= idx; i--)
                        avaliableAppleList.Add(mAppleArray[i, y]);
                    return true;
                }
                else if (sum > 10)
                    return false;
            }
            return false;
        }
    }

    public void Onclick_FindAvailableTotal()
    {
        FindAvailableTotal(HighlightAvaliableApples);
    }

    public void CheckTotalAvaliable(Action finishCallback = null)
    {
        if (FindAvailableTotal())
        {
            StartCoroutine(CoWaitForAppleMovement(finishCallback));
            return;
        }
        else
        {
            // 셔플 조건
            // 1) 힌트찾기로 총합10이 가능한 사과 배열을 찾지 못한 경우
            // 2) 사과들의 숫자가 전부 5 초과인 경우
            // 1 -> 기존 사과들로 랜덤 섞기
            // 2 -> 새로 숫자 생성 후 랜덤 섞기

            StartCoroutine(CoApplesMoveToZero(() =>
            {
                var appleList = mAppleArray.ToList();
                appleList.Shuffle();
                int index = 0;
                for (int _y = 0; _y < y; _y++)
                {
                    for (int _x = 0; _x < x; _x++)
                    {
                        var apple = appleList[index];
                        bool isTotalSumInvalid = mAppleArray.All(_apple => _apple.number > 5) || mAppleArray.Count(_apple => _apple.number <= 5) <= 1;
                        if (isTotalSumInvalid)
                            apple.SetNumber(Random.Range(1, 10));
                        mAppleArray[_x, _y] = apple.SetIndex(new Index(_x, _y));
                        index++;
                    }
                }
                StartCoroutine(CoApplesMoveToEachPos(() =>
                {
                    finishCallback?.Invoke();
                    CheckTotalAvaliable(finishCallback);
                }));
            }));
        }
        IEnumerator CoApplesMoveToZero(Action callback = null)
        {
            foreach(var apple in mAppleArray)
            {
                apple.MoveToTarget(Vector2.zero);
            }
            yield return CommonUtility.GetYieldSec(UIItemApple.APPLE_SHUFFLE_MOVEMENT_TIME);
            callback?.Invoke();
        }
        IEnumerator CoApplesMoveToEachPos(Action callback = null)
        {
            foreach (var apple in mAppleArray)
            {
                var index = apple.index;
                var targetPos = new Vector2(index.x + 0.5f, -index.y - 0.5f) * mSpace - offSet;
                apple.MoveToTarget(targetPos);                
            }
            yield return CommonUtility.GetYieldSec(UIItemApple.APPLE_SHUFFLE_MOVEMENT_TIME);
            callback?.Invoke();
        }
    }
    private IEnumerator CoWaitForAppleMovement(Action callback)
    {
        yield return CommonUtility.GetYieldSec(UIItemApple.APPLE_MOVEMENT_TIME + 0.05f);
        callback?.Invoke();
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
            new Vector2(mSpace, mSpace * count) :
            new Vector2(mSpace * count, mSpace);
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
            popupPause.SetScore(mScore)
                      .ShowPopup();
        }

        //InputHandler.HandleKeyboardInput(KeyCode.Q, KeyInput.keyDown, Onclick_FindAvailableTotal);
    }
}
