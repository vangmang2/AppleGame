﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class StaticCanvas : MonoBehaviour
{
    public static StaticCanvas instance { get; private set; }

    [SerializeField] PlayTimeChecker playTimeChecker;

    [RuntimeInitializeOnLoadMethod]
    static void InitializeOnStartUp()
    {
        instance = CreateInstance();
    }

    static StaticCanvas CreateInstance()
    {
        var commonManager = "Prefabs/StaticCanvas".LoadResource<StaticCanvas>();
        var instance = Instantiate(commonManager);
        instance.name = "CommonManager";
        DontDestroyOnLoad(instance);
        return instance;
    }

    public bool CanPlay()
    {
        return playTimeChecker.CanPlay();
    }

    public void UseAppleToPlay()
    {
        playTimeChecker.UseAppleToPlay();
    }
}