﻿using System;
using BepInEx.Logging;
using UnityEngine;

namespace ShipExpander.Core;

public abstract class SELogger
{
    private static void Log(String message, LogLevel logLevel = LogLevel.Message)
    {
        ShipExpanderModBase.Log.Log(logLevel, message);
    }

    public static void Log(String source, String message, LogLevel logLevel = LogLevel.Message)
    {
        Log($"({source}) -> {message}", logLevel);
    }

    public static void Log(GameObject gameObject, String message, LogLevel logLevel = LogLevel.Message)
    {
        Log(gameObject.name, message, logLevel);
    }
    
    public static void LogInfo(String source, String message)
    {
        Log($"({source}) -> {message}", LogLevel.Info);
    }
    
    public static void LogError(String source, String message)
    {
        Log($"({source}) -> {message}", LogLevel.Error);
    }
}