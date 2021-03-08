using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComboSystem : MonoBehaviour
{
    [SerializeField] float comboAvailableTime;
    [SerializeField] Text txtCombo;
    int currentCombo;
    float prevComboActivatedTime;

    bool isAvailableToShowCombo => Time.realtimeSinceStartup - prevComboActivatedTime <= comboAvailableTime;
    private void SetComboText(int combo)
    {
        var str = $"<size=25>COMBO</size>\n{combo}";
        txtCombo.Set(str);
    }

    public void ActivateCombo()
    {
        if (isAvailableToShowCombo || currentCombo == 0)
            currentCombo++;

        if (currentCombo == 5)
            Achievement.Achieve_MakeCombo5((key) => IngameManager.instance.ShowAchievement(key));
        else if(currentCombo == 10)
            Achievement.Achieve_MakeCombo10((key) => IngameManager.instance.ShowAchievement(key));

        prevComboActivatedTime = Time.realtimeSinceStartup;
        SetComboText(currentCombo);
    }

    void Update()
    {
        if(!isAvailableToShowCombo && currentCombo > 0)
        {
            txtCombo.text = string.Empty;
            currentCombo = 0;
        }
    }
}
