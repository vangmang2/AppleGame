using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class UIItemApple : MonoBehaviour
{
    [SerializeField] Image imageSelection;
    [SerializeField] Image imageBg;
    [SerializeField] Text txtNumber;
    public int id { get; private set; }
    public int number { get; private set; }

    public UIItemApple SetLocalPosition(Vector2 position)
    {
        transform.localPosition = position;
        return this;
    }
    public UIItemApple SetID(int id)
    {
        this.id = id;
        return this;
    }
    public UIItemApple SetSprite(Sprite sprite)
    {
        imageBg.SetSprite(sprite);
        return this;
    }

    public UIItemApple SetNumber(int number)
    {
        this.number = number;
        txtNumber.Set(number);
        return this;
    }

    public UIItemApple SetTextColor(Color color)
    {
        txtNumber.color = color;
        return this;
    }

    public UIItemApple SetLocalPosition(Vector3 localPosition)
    {
        transform.localPosition = localPosition;
        return this;
    }
    
    public void ShowSelectionImage(bool enable)
    {
        imageSelection.color = enable ? imageSelection.color.CopyColor(a: 1f) : imageSelection.color.CopyColor(a: 0f);
    }

    void GetItemApple(Action<UIItemApple> callback)
    {
        callback?.Invoke(this);
    }
}
