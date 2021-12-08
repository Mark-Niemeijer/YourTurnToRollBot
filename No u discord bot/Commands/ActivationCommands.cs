using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using No_u_discord_bot.DataObjects;
using No_u_discord_bot.Helpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace No_u_discord_bot.Commands
{
	public class ActivationCommands : BaseCommandModule
	{
		[Command("Silent"), Description("Syntax: $[Command]\nStops the bot from reacting to non-command sentences"), CommandCustomGroupAttribute("NonCommand Commands")]
		public async Task SilenceBot(CommandContext commandContext)
		{
			ConfigJson configJson = JsonParser.GetInstance().LoadData<ConfigJson>(JsonParser.FileEnum.ConfigFile);
			List<ulong> silentServerIdList = new List<ulong>(configJson.SilencedServerArray);

			if (!silentServerIdList.Contains(commandContext.Channel.Id))
			{
				silentServerIdList.Add(commandContext.Channel.Id);
				configJson.SilencedServerArray = silentServerIdList.ToArray();
				JsonParser.GetInstance().SaveData(configJson);
				await commandContext.Channel.SendMessageAsync("Alright :(").ConfigureAwait(false);
			}
		}

		[Command("Speak"), Description("Syntax: $[Command]\nAllows the bot to react to non-command sentences"), CommandCustomGroupAttribute("NonCommand Commands")]
		public async Task Awaken(CommandContext commandContext)
		{
			ConfigJson configJson = JsonParser.GetInstance().LoadData<ConfigJson>(JsonParser.FileEnum.ConfigFile);
			List<ulong> silentServerIdList = new List<ulong>(configJson.SilencedServerArray);			

			if (silentServerIdList.Contains(commandContext.Channel.Id))
			{
				silentServerIdList.Remove(commandContext.Channel.Id);
				configJson.SilencedServerArray = silentServerIdList.ToArray();
				JsonParser.GetInstance().SaveData(configJson);
				await commandContext.Channel.SendMessageAsync("Yaaaaaay :)").ConfigureAwait(false);
			}
		}
	}
}
