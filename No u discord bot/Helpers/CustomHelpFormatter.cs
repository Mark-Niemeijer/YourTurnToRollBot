using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.CommandsNext.Entities;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace No_u_discord_bot.Helpers
{
	public class CustomHelpFormatter : BaseHelpFormatter
	{
		private struct CommandInformation
		{
			public string CommandGroup;
			public string CommandName;
			public string CommandDescription;
			public List<string> CommandAlliases;
		}

		private List<CommandInformation> RegisteredCommands;

		public CustomHelpFormatter(CommandContext ctx) : base(ctx)
		{
			RegisteredCommands = new List<CommandInformation>();
			// Help formatters do support dependency injection.
			// Any required services can be specified by declaring constructor parameters. 

			// Other required initialization here ...
		}

		public override BaseHelpFormatter WithCommand(Command command)
		{
			CommandCustomGroupAttribute customGroupAttribute = null;

			foreach (CheckBaseAttribute attribute in command.ExecutionChecks)
			{
				if(attribute.GetType() == typeof(CommandCustomGroupAttribute))
				{
					customGroupAttribute = (CommandCustomGroupAttribute)attribute;
				}
			}

			CommandInformation commandInformation = new CommandInformation();
			commandInformation.CommandName = command.Name;
			commandInformation.CommandAlliases = command.Aliases.ToList();
			commandInformation.CommandDescription = command.Description;
			commandInformation.CommandGroup = customGroupAttribute == null ? "None" : customGroupAttribute.CustomGroupName;
			RegisteredCommands.Add(commandInformation);

			return this;
		}

		public override BaseHelpFormatter WithSubcommands(IEnumerable<Command> subcommands)
		{
			foreach (var cmd in subcommands)
			{
				WithCommand(cmd);
			}

			return this;
		}

		public override CommandHelpMessage Build()
		{
			string previousGroup = string.Empty;
			DiscordEmbedBuilder discordEmbedBuilder = new DiscordEmbedBuilder();
			List<IGrouping<string, CommandInformation>> sortedList = RegisteredCommands.OrderBy(i => i.CommandGroup).GroupBy(i=> i.CommandGroup).ToList();

			discordEmbedBuilder.Title = "List of all available commands";
			foreach (IGrouping<string, CommandInformation> commandsPerGroup in sortedList)
			{
				StringBuilder groupString = new StringBuilder();

				List<CommandInformation> sortedCommandsPerGroup = commandsPerGroup.OrderBy(i => i.CommandName).ToList();
				foreach (CommandInformation command in sortedCommandsPerGroup)
				{
					groupString.AppendLine($"**{command.CommandName}{(command.CommandAlliases.Count > 0 ? "," : "")} {string.Join(',',command.CommandAlliases)}**");
					groupString.AppendLine(command.CommandDescription == null ? "No Description" : command.CommandDescription);
				}

				discordEmbedBuilder.AddField("Category: " + commandsPerGroup.Key, groupString.ToString());
			}
			return new CommandHelpMessage(embed: discordEmbedBuilder);
		}
	}
}