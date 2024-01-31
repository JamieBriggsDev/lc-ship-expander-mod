using System;
using BepInEx.Logging;
using UnityEngine;

namespace ShipExpander.Core;

public abstract class SELogger
{
    public static void Log(String message, LogLevel logLevel = LogLevel.Debug)
    {
        ShipExpanderModBase.Log.Log(logLevel, message);
    }

    public static void Log(String source, String message, LogLevel logLevel = LogLevel.Debug)
    {
        Log($"({source}) -> {message}", logLevel);
    }

    public static void Log(GameObject gameObject, String message, LogLevel logLevel = LogLevel.Debug)
    {
        Log(gameObject.name, message, logLevel);
    }
}