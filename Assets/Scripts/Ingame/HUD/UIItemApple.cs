using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Random = UnityEngine.Random;

public class UIItemApple : MonoBehaviour
{
    [SerializeField] Collider2D collider2D;
    [SerializeField] Image imageSelection;
    [SerializeField] Image imageBg;
    [SerializeField] Text txtNumber;
    public bool isTerminated { get; private set; }

    public int id;// { get; private set; }
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

    public void Terminate()
    {
        isTerminated = true;
        collider2D.enabled = false;
        StartCoroutine(CoPlayTerminationAnim());
    }

    private IEnumerator CoPlayTerminationAnim()
    {
        var startPos = (Vector2)transform.localPosition;
        var endPos = new Vector2(startPos.x + Random.Range(-350f, 350f), -Screen.height);
        var center = (startPos + endPos) * 0.5f;
        center.y = startPos.y + Random.Range(230f, 450f);

        var t = 0f;
        var startRot = transform.localRotation;
        var randAngle = Random.Range(-180f, 180f);
        while (t <= 1f)
        {
            t += Time.deltaTime;
            // Slerp로는 원하는 연출이 나오지 않아서 Lerp로 곡선이동 구현
            var pos1 = Vector2.Lerp(startPos, center, t);
            var pos2 = Vector2.Lerp(center, endPos, t);
            var targetPos = Vector2.Lerp(pos1, pos2, t);
            transform.localPosition = targetPos;
            transform.localRotation = Quaternion.Lerp(startRot, Quaternion.Euler(0f, 0f, randAngle), t);
            yield return null;
        }
        gameObject.SetActive(false);
    }

    /*Reflection*/
    void GetItemApple(Action<UIItemApple> callback)
    {
        callback?.Invoke(this);
    }
}
