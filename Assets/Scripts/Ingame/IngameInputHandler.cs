using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class IngameInputHandler : MonoBehaviour
{
    [SerializeField] Transform trCanvas;
    [SerializeField] Camera camera;
    [SerializeField] Image selectionImage;
    readonly List<UIItemApple> appleList = new List<UIItemApple>();

    int prevListCount;
    Action<List<UIItemApple>> checkSum;

    // Update is called once per frame
    void Update()
    {
        if (IngameLogic.isGameOver)
        {
            ReleaseSelectedApples();
            return;
        }
        CheckApplesInCollider();
    }

    Vector2 startPos;
    private void CheckApplesInCollider()
    {
        InputHandler.HandleMouseInput(MouseInput.leftMouseBtnDown, () =>
        {
            selectionImage.gameObject.SetActive(true);
            startPos = camera.MousePosToWorldPos();
            selectionImage.rectTransform.anchoredPosition = startPos;
        });
        InputHandler.HandleMouseInput(MouseInput.leftMouseBtnPress, () =>
        {
            var area = camera.MousePosToWorldPos() - startPos;
            selectionImage.rectTransform.localScale = new Vector2(area.x, area.y);
            var pointA = camera.MousePosToWorldPos() * trCanvas.localScale;
            var pointB = startPos * trCanvas.localScale;
            var apples = Physics2D.OverlapAreaAll(pointA, pointB);

            if (appleList.Count != prevListCount)
            {
                appleList.RemoveAll(apple =>
                {
                    apple.ShowSelectionImage(false);
                    return apple;
                });
            }
            prevListCount = appleList.Count;
            apples.ForEach(apple =>
            {
                UIItemApple itemApple = null;
                Action<UIItemApple> callback = (_itemApple) => itemApple = _itemApple;
                apple.transform.SendMessage("GetItemApple", callback, SendMessageOptions.DontRequireReceiver);

                if (itemApple != null && !appleList.Contains(itemApple))
                {
                    itemApple.ShowSelectionImage(true);
                    appleList.Add(itemApple);
                }
            });
        });
        InputHandler.HandleMouseInput(MouseInput.leftMouseBtnUp, () =>
        {
            ReleaseSelectedApples();
        });
    }

    private void ReleaseSelectedApples()
    {
        checkSum?.Invoke(appleList);
        selectionImage.gameObject.SetActive(false);
        appleList.RemoveAll(apple =>
        {
            apple.ShowSelectionImage(false);
            return apple;
        });
    }

    public void SetCheckSumAction(Action<List<UIItemApple>> callback)
    {
        checkSum = callback;
    }
}
