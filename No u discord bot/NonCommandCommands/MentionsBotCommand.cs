using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.EventArgs;

namespace No_u_discord_bot.NonCommandCommands
{
	class MentionsBotCommand : NonCommandInterface
	{
		public async Task ExcuteCommand(MessageCreateEventArgs e, DiscordClient botClient)
		{
			await e.Message.RespondAsync(":eyes:");
		}

		public bool MeetsRequirements(MessageCreateEventArgs e, DiscordClient botClient, string loweredMessage)
		{
			return new List<DSharpPlus.Entities.DiscordUser>(e.Message.MentionedUsers).Contains(botClient.CurrentUser);
		}
	}
}
