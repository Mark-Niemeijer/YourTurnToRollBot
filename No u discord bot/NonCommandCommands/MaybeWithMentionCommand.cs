using DSharpPlus;
using DSharpPlus.EventArgs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace No_u_discord_bot.NonCommandCommands
{
	class MaybeWithMentionCommand: NonCommandInterface
	{
		public async Task ExcuteCommand(MessageCreateEventArgs e, DiscordClient botClient)
		{
			for (int i = 0; i < e.MentionedUsers.Count; i++)
			{
				await e.Message.RespondAsync("Or maybe not " + e.Message.Author.Mention);
			}
		}

		public bool MeetsRequirements(MessageCreateEventArgs e, DiscordClient botClient, string loweredMessage)
		{
			return e.MentionedUsers.Count > 0 && loweredMessage.StartsWith("maybe");
		}
	}
}
