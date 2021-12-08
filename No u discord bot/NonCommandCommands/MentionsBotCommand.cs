using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.EventArgs;
using No_u_discord_bot.Helpers;

namespace No_u_discord_bot.NonCommandCommands
{
	class MentionsBotCommand : NonCommandInterface
	{
		public async Task ExcuteCommand(MessageCreateEventArgs e, DiscordClient botClient)
		{
			await e.Message.RespondAsync(":eyes:");
			CustomDebugInfo.LogInfo(e.Author.Username + " mentioned me");
		}

		public bool MeetsRequirements(MessageCreateEventArgs e, DiscordClient botClient, string loweredMessage)
		{
			return new List<DSharpPlus.Entities.DiscordUser>(e.Message.MentionedUsers).Contains(botClient.CurrentUser);
		}
	}
}
