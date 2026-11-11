using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public enum LogLevel
{
    Info,
    Warning,
    Error,
    Debug
}

public static class MainLogger
{
    public static Action<LogMessage> OnLog;

    public static void Log(
        string message,
        string category,
        LogLevel level = LogLevel.Info,
        bool showInUnity = true,
        [CallerMemberName] string caller = "",
        [CallerFilePath] string file = "",
        [CallerLineNumber] int line = 0)
    {
        var logMsg = new LogMessage
        {
            Level = level,
            Category = category,
            Message = message,
            Caller = caller,
            File = file,
            Line = line,
            Time = DateTime.Now
        };

        OnLog?.Invoke(logMsg);

        if (showInUnity)
        {
            switch (level)
            {
                case LogLevel.Info:
                case LogLevel.Debug:
                    Debug.Log(Format(logMsg));
                    break;
                case LogLevel.Warning:
                    Debug.LogWarning(Format(logMsg));
                    break;
                case LogLevel.Error:
                    Debug.LogError(Format(logMsg));
                    break;
            }
        }
    }

    private static string Format(LogMessage log)
        => $"[{log.Category}] {log.Message}";

    private static string FormatGameConsole(LogMessage log)
        => $"[{log.Time:HH:mm:ss}] [{log.Category}] [{log.Level}] {log.Message} (at {log.Caller}:{log.Line})";
}

/// Удобная обёртка для конкретной категории
public class Logger
{
    private readonly string _category;
    private readonly bool _showInUnity;

    public Logger(string category, bool showInUnity = true)
    {
        _category = category;
        _showInUnity = showInUnity;
    }

    public void Info(string message,
        [CallerMemberName] string caller = "",
        [CallerFilePath] string file = "",
        [CallerLineNumber] int line = 0)
        => MainLogger.Log(message, _category, LogLevel.Info, _showInUnity, caller, file, line);

    public void Warning(string message,
        [CallerMemberName] string caller = "",
        [CallerFilePath] string file = "",
        [CallerLineNumber] int line = 0)
        => MainLogger.Log(message, _category, LogLevel.Warning, _showInUnity, caller, file, line);

    public void Error(string message,
        [CallerMemberName] string caller = "",
        [CallerFilePath] string file = "",
        [CallerLineNumber] int line = 0)
        => MainLogger.Log(message, _category, LogLevel.Error, _showInUnity, caller, file, line);

    public void Debug(string message,
        [CallerMemberName] string caller = "",
        [CallerFilePath] string file = "",
        [CallerLineNumber] int line = 0)
        => MainLogger.Log(message, _category, LogLevel.Debug, _showInUnity, caller, file, line);
}

[Serializable]
public class LogMessage
{
    public LogLevel Level;
    public string Category;
    public string Message;
    public string Caller;
    public string File;
    public int Line;
    public DateTime Time;
}