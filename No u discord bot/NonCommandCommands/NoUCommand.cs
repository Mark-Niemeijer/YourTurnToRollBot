using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.EventArgs;
using No_u_discord_bot.Helpers;

namespace No_u_discord_bot.NonCommandCommands
{
	class NoUCommand : NonCommandInterface
	{
		public async Task ExcuteCommand(MessageCreateEventArgs e, DiscordClient botClient)
		{
			await e.Message.RespondAsync("No u!");
			CustomDebugInfo.LogInfo("No U-ed " + e.Author.Username);
		}

		public bool MeetsRequirements(MessageCreateEventArgs e, DiscordClient botClient, string loweredMessage)
		{
			return loweredMessage == "no u";
		}
	}
}
