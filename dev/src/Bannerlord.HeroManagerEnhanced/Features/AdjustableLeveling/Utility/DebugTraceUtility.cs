using System;
using System.Collections.Generic;
using System.IO;
using AdjustableLeveling.Settings;
using TaleWorlds.Library;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace AdjustableLeveling.Utility
{
  public static class DebugTraceUtility
  {
    public static bool Enabled { get; set; } = false;
    public static string? LogDirectoryPath => GetLogDirectoryPath();

    private static readonly object _lock = new();
    private static readonly Dictionary<string, DateTime> _lastLogByKey = [];
    private static readonly HashSet<string> _loggedOnceKeys = [];
    private static DateTime _lastMessageDisplayUtc = DateTime.MinValue;
    private const double DebugMessageDisplayCooldownSeconds = 5.0;

    public static void Log(string message)
    {
      if (!IsEnabled())
        return;

      try
      {
        DisplayDebugMessage($"[AdjustableLeveling] {message}");
      }
      catch
      {
      }
    }

    public static void LogAlways(string message)
    {
      try
      {
        DisplayDebugMessage($"[AdjustableLeveling] {message}");
      }
      catch
      {
      }
    }

    public static void LogThrottled(string key, string message, float minSeconds = 1f)
    {
      if (!IsEnabled())
        return;

      try
      {
        var now = DateTime.UtcNow;
        lock (_lock)
        {
          if (_lastLogByKey.TryGetValue(key, out var last)
            && (now - last).TotalSeconds < minSeconds)
            return;

          _lastLogByKey[key] = now;
        }

        Log(message);
      }
      catch
      {
      }
    }

    public static void LogOnce(string key, string message)
    {
      if (!IsEnabled())
        return;

      try
      {
        lock (_lock)
        {
          if (_loggedOnceKeys.Contains(key))
            return;

          _loggedOnceKeys.Add(key);
        }

        Log(message);
      }
      catch
      {
      }
    }

    public static (long totalBytes, int fileCount) GetCombinedLogSize()
    {
      var logDirectoryPath = GetLogDirectoryPath();
      if (string.IsNullOrWhiteSpace(logDirectoryPath) || !Directory.Exists(logDirectoryPath))
        return (0L, 0);

      long totalBytes = 0L;
      int fileCount = 0;

      try
      {
        foreach (var filePath in Directory.EnumerateFiles(logDirectoryPath, "HeroManagerEnhanced_AdjustableLeveling*.log"))
        {
          var fileInfo = new FileInfo(filePath);
          totalBytes += fileInfo.Length;
          fileCount++;
        }
      }
      catch
      {
      }

      return (totalBytes, fileCount);
    }

    public static void Clear()
    {
      lock (_lock)
      {
        _lastLogByKey.Clear();
        _loggedOnceKeys.Clear();
        _lastMessageDisplayUtc = DateTime.MinValue;
      }
    }

    private static bool IsEnabled()
    {
      try
      {
        if (MCMSettings.Settings != null)
          return MCMSettings.Settings.EnableLogging;
      }
      catch
      {
      }

      return Enabled;
    }

    private static void DisplayDebugMessage(string message)
    {
      try
      {
        // Rate limit messages to avoid spam
        var now = DateTime.UtcNow;
        if ((now - _lastMessageDisplayUtc).TotalSeconds < DebugMessageDisplayCooldownSeconds)
        {
          return;
        }

        _lastMessageDisplayUtc = now;

        var text = new TextObject(message);
        InformationManager.DisplayMessage(new InformationMessage(
          text.ToString(),
          Colors.White));

        // Also print to debug output
        Debug.Print($"[HeroManagerEnhanced] {message}");
      }
      catch
      {
      }
    }

    private static string GetLogDirectoryPath()
    {
      try
      {
        var documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        if (string.IsNullOrWhiteSpace(documentsPath))
          return string.Empty;

        return Path.Combine(documentsPath, "Mount and Blade II Bannerlord", "Configs", "ModLogs");
      }
      catch
      {
        return string.Empty;
      }
    }
  }
}
