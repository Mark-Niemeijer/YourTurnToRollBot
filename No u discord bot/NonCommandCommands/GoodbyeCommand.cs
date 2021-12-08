using DSharpPlus;
using DSharpPlus.EventArgs;
using No_u_discord_bot.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace No_u_discord_bot.NonCommandCommands
{
	class GoodbyeCommand : NonCommandInterface
	{
		string[] goodbyeTriggers = { "good night", "good bye", "slaap lekker", "goodnight", "goodbye", "slaaplekker" };
		string[] goodbyeResponces = { ":wave:", "Goodnight :heart:", "Slaap lekker :p" };

		public async Task ExcuteCommand(MessageCreateEventArgs e, DiscordClient botClient)
		{
			Random numberGenerator = new Random();
			await e.Message.RespondAsync(goodbyeResponces[numberGenerator.Next(0, goodbyeResponces.Length)]);
			CustomDebugInfo.LogInfo("Whished goodnight to " + e.Author.Username);
		}

		public bool MeetsRequirements(MessageCreateEventArgs e, DiscordClient botClient, string loweredMessage)
		{			
			List<DSharpPlus.Entities.DiscordUser> mentionedUsers = new List<DSharpPlus.Entities.DiscordUser>(e.Message.MentionedUsers);
			return Array.Exists(goodbyeTriggers, e => loweredMessage.Contains(e)) && (mentionedUsers.Contains(botClient.CurrentUser) || e.Message.MentionEveryone);
		}
	}
}
