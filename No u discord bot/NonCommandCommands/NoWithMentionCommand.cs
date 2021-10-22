using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.EventArgs;

namespace No_u_discord_bot.NonCommandCommands
{
	class NoWithMentionCommand : NonCommandInterface
	{
		public async Task ExcuteCommand(MessageCreateEventArgs e, DiscordClient botClient)
		{
			for (int i = 0; i < e.MentionedUsers.Count; i++)
			{
				await e.Message.RespondAsync("Yes " + e.Message.Author.Mention);
			}
		}

		public bool MeetsRequirements(MessageCreateEventArgs e, DiscordClient botClient, string loweredMessage)
		{
			return e.MentionedUsers.Count > 0 && loweredMessage.StartsWith("no");
		}
	}
}
