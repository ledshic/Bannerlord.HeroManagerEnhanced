using System.IO;
using System;
using System.Collections.Generic;
using AdjustableLeveling.Settings;

namespace AdjustableLeveling.Utility
{
  public static class DebugTraceUtility
  {
    public const string LogFilePrefix = "HeroManagerEnhanced_AdjustableLeveling";

    public static bool Enabled { get; set; } = false;

    private static readonly string LogPath = Path.Combine(
      Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
      "Mount and Blade II Bannerlord",
      "Configs",
      "ModLogs",
      "HeroManagerEnhanced_AdjustableLeveling.log");

    public static string LogDirectoryPath => Path.GetDirectoryName(LogPath);

    private static readonly object _lock = new();
    private static readonly Dictionary<string, DateTime> _lastLogByKey = [];
    private static readonly HashSet<string> _loggedOnceKeys = [];

    public static void Log(string message)
    {
      if (!IsEnabled())
        return;

      try
      {
        AppendLine($"[AdjustableLeveling][{DateTime.Now:HH:mm:ss.fff}] {message}");
      }
      catch
      {
      }
    }

    public static void LogAlways(string message)
    {
      try
      {
        AppendLine($"[AdjustableLeveling][{DateTime.Now:HH:mm:ss.fff}] {message}");
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

    public static (long TotalBytes, int FileCount) GetCombinedLogSize()
    {
      try
      {
        var dir = LogDirectoryPath;
        if (string.IsNullOrWhiteSpace(dir) || !Directory.Exists(dir))
          return (0L, 0);

        long total = 0;
        int count = 0;
        foreach (var path in Directory.GetFiles(dir, "*", SearchOption.TopDirectoryOnly))
        {
          var fileName = Path.GetFileName(path);
          if (string.IsNullOrWhiteSpace(fileName))
            continue;
          if (!fileName.StartsWith(LogFilePrefix, StringComparison.OrdinalIgnoreCase))
            continue;
          if (fileName.IndexOf(".log", StringComparison.OrdinalIgnoreCase) < 0)
            continue;

          var fileInfo = new FileInfo(path);
          total += fileInfo.Length;
          count++;
        }

        return (total, count);
      }
      catch
      {
        return (0L, 0);
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

    private static void AppendLine(string line)
    {
      lock (_lock)
      {
        var dir = Path.GetDirectoryName(LogPath);
        if (!string.IsNullOrWhiteSpace(dir))
          Directory.CreateDirectory(dir);

        File.AppendAllText(LogPath, line + Environment.NewLine);
      }
    }
  }
}