using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;
using Microsoft.Extensions.Logging;
using No_u_discord_bot.Commands;
using No_u_discord_bot.DataObjects;
using No_u_discord_bot.Helpers;
using No_u_discord_bot.LooseSystems;
using No_u_discord_bot.NonCommandCommands;
using System;
using System.Threading.Tasks;

namespace No_u_discord_bot
{
	public class Bot
	{
		public DiscordClient Client { get; private set; }
		public InteractivityExtension Interactivity { get; private set; }
		public CommandsNextExtension Commands { get; private set; }

		public async Task RunBotAsync()
		{
			ConfigJson configJson = JsonParser.GetInstance().LoadData<ConfigJson>(JsonParser.FileEnum.ConfigFile);

			DiscordConfiguration discordConfig = new DiscordConfiguration();
			discordConfig.Token = configJson.Token;
			discordConfig.TokenType = TokenType.Bot;
			discordConfig.AutoReconnect = true;
			discordConfig.MinimumLogLevel = LogLevel.Debug;

			Client = new DiscordClient(discordConfig);
			Client.Ready += OnClientReady;
			Client.MessageCreated += new Non_Prefix_Commands().OnClientMessageCreated;
			Client.ClientErrored += Client_ClientErrored;

			CommandsNextConfiguration commandsNextConfig = new CommandsNextConfiguration();
			commandsNextConfig.StringPrefixes = configJson.CommandPrefixs;
			commandsNextConfig.EnableMentionPrefix = false;
			commandsNextConfig.EnableDms = true;
			commandsNextConfig.CaseSensitive = false;
			commandsNextConfig.DmHelp = false;
			commandsNextConfig.IgnoreExtraArguments = false;

			Commands = Client.UseCommandsNext(commandsNextConfig);
			Commands.SetHelpFormatter<CustomHelpFormatter>();
			Commands.RegisterCommands<ActivationCommands>();
			Commands.RegisterCommands<HangManCommands>();
			Commands.RegisterCommands<DiceRollCommands>();
			Commands.RegisterCommands<PetCommand>();
			Commands.RegisterCommands<BirthdayCelebratorCommands>();
			Commands.RegisterCommands<ReminderCommands>();
			Commands.RegisterCommands<RPGCommands>();
			Commands.CommandErrored += Commands_CommandErrored;

			Client.UseInteractivity(new InteractivityConfiguration()
			{
				PollBehaviour = PollBehaviour.DeleteEmojis,
				Timeout = TimeSpan.FromMinutes(1)
			});

			await Client.ConnectAsync();
			//Keep bot running
			await Task.Delay(-1);
		}

		private Task Commands_CommandErrored(CommandsNextExtension sender, CommandErrorEventArgs e)
		{
			CustomDebugInfo.LogError("Command threw an exception:" + e.Exception.Message + "\nFull information: " + e.Exception.ToString());
			return Task.CompletedTask;
		}

		private Task Client_ClientErrored(DiscordClient sender, DSharpPlus.EventArgs.ClientErrorEventArgs e)
		{
			CustomDebugInfo.LogError("Event " + e.EventName + " threw an exception:\n" + e.Exception);
			return Task.CompletedTask;
		}

		private Task OnClientReady(DiscordClient sender, DSharpPlus.EventArgs.ReadyEventArgs e)
		{
			BirthDayTimer.StartDateChecker(sender);
			ReminderTimer.StartDateChecker(sender);
			CustomDebugInfo.LogInfo("Bot ready to go");

			return Task.CompletedTask;
		}
	}
}
