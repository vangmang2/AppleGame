using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIItemProgressBar : MonoBehaviour
{
    [SerializeField] Image imgProgressBar;

    public void SetFill(float fillAmount)
    {
        imgProgressBar.fillAmount = fillAmount;
    }
}
