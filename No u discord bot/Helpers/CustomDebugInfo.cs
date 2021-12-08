using System;
using System.Collections.Generic;
using System.Text;

namespace No_u_discord_bot.Helpers
{
	static class CustomDebugInfo
	{
		public static void LogInfo(string info)
		{
			Console.Write("[ " + DateTime.Now.ToString() + " ] ");
			Console.ForegroundColor = ConsoleColor.Blue;
			Console.Write("[ INFO ] ");
			Console.ForegroundColor = ConsoleColor.Gray;
			Console.WriteLine(info);
		}

		public static void LogWarning(string info)
		{
			Console.Write("[ " + DateTime.Now.ToString() + " ] ");
			Console.ForegroundColor = ConsoleColor.DarkYellow;
			Console.Write("[ WARNING ] ");
			Console.ForegroundColor = ConsoleColor.Gray;
			Console.WriteLine(info);
		}

		public static void LogError(string info)
		{
			Console.Write("[ " + DateTime.Now.ToString() + " ] ");
			Console.ForegroundColor = ConsoleColor.Red;
			Console.Write("[ ERROR ] ");
			Console.ForegroundColor = ConsoleColor.Gray;
			Console.WriteLine(info);
		}
	}
}
