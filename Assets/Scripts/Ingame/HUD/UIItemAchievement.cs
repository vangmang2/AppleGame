using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIItemAchievement : MonoBehaviour
{
    const float TITLE_ANIM_SPEED = 1f;

    [SerializeField] Text txtAchievement;
    [SerializeField] RectTransform rtBody;

    Coroutine coroutine;
    public void ShowAchievementTitle(string key)
    {
        txtAchievement.Set(key);
        if (coroutine != null)
            StopCoroutine(coroutine);
        StartCoroutine(CoPlayTitleAnim());
    }

    private IEnumerator CoPlayTitleAnim()
    {
        var startPosition = Vector2.zero;
        var endPosition = new Vector2(0f, -80f);

        var duration = TITLE_ANIM_SPEED * 0.5f;
        var t = 0f;

        rtBody.anchoredPosition = startPosition;

        while (t <= 1f)
        {
            t += Time.deltaTime / duration;
            rtBody.anchoredPosition = Vector2.Lerp(startPosition, endPosition, t);
            yield return null;
        }

        yield return CommonUtility.GetYieldSec(2f);

        t = 0f;
        while (t <= 1f)
        {
            t += Time.deltaTime / duration;
            rtBody.anchoredPosition = Vector2.Lerp(endPosition, startPosition, t);
            yield return null;
        }
    }
}
