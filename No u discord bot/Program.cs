using System;

namespace No_u_discord_bot
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Preparing bot startup");
			Bot discordBot = new Bot();
			discordBot.RunBotAsync().GetAwaiter().GetResult();
		}
	}
}
