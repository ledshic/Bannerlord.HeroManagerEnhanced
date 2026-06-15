using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Library;

namespace AdjustableLeveling.Utility
{
	public static class GeneralUtility
	{
		public static void Trace(string s)
		{
			DebugTraceUtility.Log(s);
		}

		public static void TraceThrottled(string key, string s, float minSeconds = 1f)
		{
			DebugTraceUtility.LogThrottled(key, s, minSeconds);
		}

		public static void TraceOnce(string key, string s)
		{
			DebugTraceUtility.LogOnce(key, s);
		}

		public static void Message(string s, bool stacktrace = true, Color? color = null, bool log = true)
		{
			try
			{
				if (log)
					DebugTraceUtility.Log(s + (stacktrace ? $"\n{Environment.StackTrace}" : ""));

				InformationManager.DisplayMessage(new InformationMessage(s, color ?? new Color(1f, 0f, 0f)));
			}
			catch
			{
			}
		}
	}
}
