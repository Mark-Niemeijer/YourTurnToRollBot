using No_u_discord_bot.Helpers;
using System;

namespace No_u_discord_bot
{
	class Program
	{
		static void Main(string[] args)
		{
			CustomDebugInfo.LogInfo("Preparing bot startup");
			Bot discordBot = new Bot();
			discordBot.RunBotAsync().GetAwaiter().GetResult();
		}
	}
}
