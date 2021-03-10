using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Random = UnityEngine.Random;

public struct Index
{
    public int x, y;

    public Index(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public override string ToString()
    {
        return $"x:{x} y:{y}";
    }
}

public class UIItemApple : MonoBehaviour, IPoolable<UIItemApple>
{
    public const float APPLE_MOVEMENT_TIME = 0.25f;
    public const float APPLE_SHUFFLE_MOVEMENT_TIME = 1f;

    [SerializeField] RectTransform rtBody;
    [SerializeField] Collider2D collider2D;
    [SerializeField] Image imageSelection;
    [SerializeField] Text txtNumber;

    public GameObject getGameObject => gameObject;
    public Vector2 getLocalPosition => transform.localPosition;
    public bool isTerminated { get; private set; }
    public Index index { get; private set; }
    public int number { get; private set; }

    public UIItemApple SetParent(Transform parent)
    {
        transform.SetParent(parent);
        return this;
    }
    public UIItemApple SetSize(Vector2 size)
    {
        rtBody.sizeDelta = size;
        return this;
    }
    public UIItemApple SetLocalPosition(Vector2 position)
    {
        transform.localPosition = position;
        return this;
    }
    public UIItemApple SetLocalRotation(Quaternion rotation)
    {
        transform.localRotation = rotation;
        return this;
    }

    public UIItemApple SetIndex(Index index)
    {
        this.index = index;
        return this;
    }

    public UIItemApple SetName(string name)
    {
        gameObject.name = name;
        return this;
    }

    public UIItemApple SetNumber(int number)
    {
        this.number = number;
        txtNumber.Set(number);
        return this;
    }
    
    public void ShowSelectionImage(bool enable)
    {
        imageSelection.color = enable ? imageSelection.color.CopyColor(a: 1f) : imageSelection.color.CopyColor(a: 0f);
    }

    public void Terminate(Action callback = null)
    {
        isTerminated = true;
        collider2D.enabled = false;
        StartCoroutine(CoPlayTerminationAnim(callback));
    }

    private IEnumerator CoPlayTerminationAnim(Action callback)
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
        callback?.Invoke();
    }

    public void MoveToTarget(Vector2 target, float movementSpeed = APPLE_MOVEMENT_TIME)
    {
        StartCoroutine(CoMoveToTarget(target, movementSpeed));
    }

    private IEnumerator CoMoveToTarget(Vector2 target, float movementSpeed)
    {
        var startPos = transform.localPosition;
        var t = 0f;
        while(t <= 1f)
        {
            t += Time.deltaTime / movementSpeed;
            transform.localPosition = Vector2.Lerp(startPos, target, t);
            yield return null;
        }
    }

    /*Reflection*/
    public void GetInstance(Action<UIItemApple> callback)
    {
        callback?.Invoke(this);
    }

    public void OnSpawn()
    {
        collider2D.enabled = true;
        isTerminated = false;
    }
}
