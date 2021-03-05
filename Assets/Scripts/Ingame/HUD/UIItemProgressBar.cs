using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class UIItemProgressBar : MonoBehaviour
{
    [SerializeField] Image imgProgressBar;

    public void SetFill(float fillAmount, Action<float> callback = null)
    {
        imgProgressBar.fillAmount = fillAmount;
        callback?.Invoke(fillAmount);
    }
}
