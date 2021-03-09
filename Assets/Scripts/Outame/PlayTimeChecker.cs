using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class PlayTimeChecker : MonoBehaviour
{
    public const int PLAY_AVALIABLE_MAX_TIME = 5;
    const string SAVED_QUIT_TIME    = "SavedQuitTime";
    const string SAVED_REFILL_TIME  = "SavedRefillTime";
    const string SAVED_APPLE_COUNT  = "SavedAppleCount";

    [SerializeField] Text txtTime;
    [SerializeField] UIItemApples itemApples;
    [SerializeField] GameObject goPivot;

    int mCurrentApples;

    readonly TimeSpan mRefillTime = new TimeSpan(0, 2, 0);
    TimeSpan mCurrentRefillTime;

    void Start()
    {
        var savedQuitTime = PlayerPrefs.GetString(SAVED_QUIT_TIME, DateTime.Now.ToString()).ParseToDateTime();
        var savedRefillTime = PlayerPrefs.GetString(SAVED_REFILL_TIME, mRefillTime.ToString()).ParseToTimeSpan();

        var currentTime = DateTime.Now;
        var timeGapFromQuit = currentTime.Subtract(savedQuitTime);

        mCurrentApples = PlayerPrefs.GetInt(SAVED_APPLE_COUNT, PLAY_AVALIABLE_MAX_TIME);
        mCurrentRefillTime = savedRefillTime;


        if (mCurrentApples == PLAY_AVALIABLE_MAX_TIME)
        {
            itemApples.EnableApples(PLAY_AVALIABLE_MAX_TIME);
            txtTime.Set("--:--");
            return;
        }
        else
        {
            mCurrentRefillTime = mCurrentRefillTime.Subtract(timeGapFromQuit);
            var avaliableApplesCount = 0;
            if(mCurrentRefillTime.TotalSeconds < 0)
            {
                mCurrentRefillTime = mCurrentRefillTime.Add(mRefillTime);
                avaliableApplesCount++;
            }
            mCurrentApples = Mathf.Min(mCurrentApples + avaliableApplesCount, PLAY_AVALIABLE_MAX_TIME);
            itemApples.EnableApples(mCurrentApples);

            StartCoroutine(CoPlayTimer());
        }
    }

    public bool CanPlay()
    {
        return mCurrentApples > 0;
    }

    public void UseAppleToPlay()
    {
        var appleCountBeforeUse = mCurrentApples;
        mCurrentApples--;
        if (appleCountBeforeUse == PLAY_AVALIABLE_MAX_TIME)
        {
            mCurrentRefillTime = mRefillTime;
            StartCoroutine(CoPlayTimer());
        }
        itemApples.EnableApples(mCurrentApples);
    }

    public void SetActive(bool enable)
    {
        goPivot.SetActive(enable);
    }

    private IEnumerator CoPlayTimer()
    {
        while (mCurrentApples < PLAY_AVALIABLE_MAX_TIME)
        {
            if(mCurrentRefillTime.Ticks <= 0)
            {
                mCurrentRefillTime = mRefillTime;
                mCurrentApples++;
                itemApples.EnableApples(mCurrentApples);
            }
            mCurrentRefillTime = mCurrentRefillTime.Subtract(new TimeSpan(0, 0, 1));
            var minute = mCurrentRefillTime.Minutes;
            var second = mCurrentRefillTime.Seconds;

            var strMinute = minute < 10 ? $"0{minute}" : minute.ToString();
            var strSecond = second < 10 ? $"0{second}" : second.ToString();

            txtTime.Set($"{strMinute}:{strSecond}");
            yield return CommonUtility.GetYieldSec(1f);
        }
        txtTime.Set("--:--");
    }


    void OnApplicationQuit()
    {
        //PlayerPrefs.DeleteAll();
        PlayerPrefs.SetString(SAVED_QUIT_TIME, DateTime.Now.ToString());
        PlayerPrefs.SetString(SAVED_REFILL_TIME, mCurrentApples == PLAY_AVALIABLE_MAX_TIME ? mRefillTime.ToString() : mCurrentRefillTime.ToString());
        PlayerPrefs.SetInt(SAVED_APPLE_COUNT, mCurrentApples);
    }
}
