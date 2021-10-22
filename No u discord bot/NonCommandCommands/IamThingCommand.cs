using DSharpPlus;
using DSharpPlus.EventArgs;
using No_u_discord_bot.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace No_u_discord_bot.NonCommandCommands
{
	class IamThingCommand : NonCommandInterface
	{
		private string[] acceptableTriggers = { "i'm ", "i am ", "iam ", "im " };

		public async Task ExcuteCommand(MessageCreateEventArgs e, DiscordClient botClient)
		{
			string loweredMessage = e.Message.Content.ToLower();
			List<string> splitMessage = new List<string>(loweredMessage.Split(acceptableTriggers, StringSplitOptions.None));
			splitMessage.RemoveAt(0);

			if (splitMessage.Count >= 1)
			{
				string fullResponse = splitMessage[splitMessage.Count - 1].Trim();
				string[] brokenUpResponse = fullResponse.Split('.', StringSplitOptions.RemoveEmptyEntries);
				if (brokenUpResponse.Length == 1)
				{
					await e.Message.RespondAsync("Hello " + brokenUpResponse[0] + ". I am " + botClient.CurrentUser.Mention);
				}
			}
		}

		public bool MeetsRequirements(MessageCreateEventArgs e, DiscordClient botClient, string loweredMessage)
		{
			if (DiceChecks.RollForSuccess(90)) return false;

			foreach (string item in acceptableTriggers)
			{
				if (loweredMessage.Contains(" " + item) || loweredMessage.StartsWith(item))
				{
					return true;
				}
			}
			return false;
		}
	}
}
