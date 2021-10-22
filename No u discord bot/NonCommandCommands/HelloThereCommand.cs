using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.EventArgs;

namespace No_u_discord_bot.NonCommandCommands
{
	class HelloThereCommand : NonCommandInterface
	{
		public async Task ExcuteCommand(MessageCreateEventArgs e, DiscordClient botClient)
		{
			await e.Message.RespondAsync("General Kenobi!");
		}

		public bool MeetsRequirements(MessageCreateEventArgs e, DiscordClient botClient, string loweredMessage)
		{
			if (loweredMessage.StartsWith("hello there"))
			{
				return true;
			}
			return false;
		}
	}
}
