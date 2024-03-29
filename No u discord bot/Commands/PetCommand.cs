﻿using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using No_u_discord_bot.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace No_u_discord_bot.Commands
{
	class PetCommand : BaseCommandModule
	{
		string[] petResponces = { ":blush:", ":smile:" };
		[Command("Pet"), Description("$Syntax: [Command]\nPets the good boy")]
		public async Task PetBot(CommandContext commandContext, [RemainingText] string junkString)
		{
			await commandContext.Channel.SendMessageAsync(petResponces[new Random().Next(0, petResponces.Length)]).ConfigureAwait(false);
			CustomDebugInfo.LogInfo("The bot was pet by '" + commandContext.User.Username + "' with the text: " + junkString);
		}
	}
}
