using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using No_u_discord_bot.Helpers;
using No_u_discord_bot.InBotAppManagers;
using No_u_discord_bot.LooseSystems;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace No_u_discord_bot.Commands
{
	class RPGCommands : BaseCommandModule
	{
		private Dictionary<DiscordChannel, MRPGGameManager> _activeGames;

		[Command("RPGStart"), Description(""), CommandCustomGroupAttribute("RPGCommands")]
		public async Task StartGame(CommandContext commandContext)
		{
			if (_activeGames == null) _activeGames = new Dictionary<DiscordChannel, MRPGGameManager>();

			if (!_activeGames.ContainsKey(commandContext.Channel))
			{
				MRPGGameManager newGameManager = new MRPGGameManager(commandContext.Channel);
				_activeGames.Add(commandContext.Channel, newGameManager);
			}
			MRPGGameManager gameManager = _activeGames[commandContext.Channel];

			if (gameManager.GameStatus == MRPGGameManager.GameState.Lobby && gameManager.getCurrentPlayers().Count > 0)
			{
				await commandContext.Channel.SendMessageAsync("A game is already busy here, but maybe you can still join it");
				return;
			}
			else if (gameManager.GameStatus == MRPGGameManager.GameState.Ingame)
			{
				await commandContext.Channel.SendMessageAsync("A game is already busy here. You can't join anymore");
				return;
			}

			await commandContext.Channel.SendMessageAsync("Welcome to the " + commandContext.Client.CurrentUser.Mention + " tavern, are we starting a new adventure or continuing an old one?\n" +
				"Make sure everyone is around the table before you start. Use the [RPGJoin] Command to come sit with us.\n" +
				"When everyone is seated, use the [RPGNewGame] or [RPGLoadGame] command to start");
			await JoinGame(commandContext);
		}

		[Command("RPGJoin"), Description(""), CommandCustomGroupAttribute("RPGCommands")]
		public async Task JoinGame(CommandContext commandContext)
		{
			if (_activeGames == null || !_activeGames.ContainsKey(commandContext.Channel))
			{
				await commandContext.Channel.SendMessageAsync("No game is currently busy on this channel. Use the [RPGStart] Command first");
				return;
			}
			MRPGGameManager gameManager = _activeGames[commandContext.Channel];

			if (gameManager.GameStatus == MRPGGameManager.GameState.Lobby)
			{
				List<DiscordUser> playersInGame = gameManager.getCurrentPlayers();
				if (playersInGame.Contains(commandContext.User))
				{
					await commandContext.Channel.SendMessageAsync("You are already at the table " + commandContext.User.Mention);
				}
				else
				{
					await commandContext.Channel.SendMessageAsync("Welcome to the table " + commandContext.User.Mention + ", lets have a great adventure");
					gameManager.AddPlayer(commandContext.User);
				}
			}
		}

		[Command("RPGNewGame"), Description(""), CommandCustomGroupAttribute("RPGCommands")]
		public async Task BeginNewGame(CommandContext commandContext)
		{
			if (_activeGames == null || !_activeGames.ContainsKey(commandContext.Channel))
			{
				await commandContext.Channel.SendMessageAsync("No game is currently busy on this channel. Use the [RPGStart] Command first");
				return;
			}
			MRPGGameManager gameManager = _activeGames[commandContext.Channel];

			if (gameManager.GameStatus == MRPGGameManager.GameState.Lobby)
			{
				await commandContext.Channel.SendMessageAsync("I am going to make a dungeon for you, please wait a few seconds");
				gameManager.StartNewGame();
				await commandContext.Channel.SendMessageAsync("Map is ready, here it comes");
				DisplayPlayerView(gameManager.PlayersTurn, gameManager, commandContext.Channel);
			}
			else
			{
				await commandContext.Channel.SendMessageAsync("The adventure has already begun, no need to start it");
			}
		}

		[Command("RPGMove"), Description(""), CommandCustomGroupAttribute("RPGCommands")]
		public async Task MoveCharacter(CommandContext commandContext, string coordinate)
		{
			if (_activeGames == null || !_activeGames.ContainsKey(commandContext.Channel))
			{
				await commandContext.Channel.SendMessageAsync("No game is currently busy on this channel. Use the [RPGStart] Command first");
				return;
			}
			MRPGGameManager gameManager = _activeGames[commandContext.Channel];

			if (coordinate.Length == 2 && Char.IsLetter(coordinate[0]) && Char.IsDigit(coordinate[1]))
			{
				await commandContext.Channel.SendMessageAsync("Valid Coordinate Given");
				gameManager.MoveCharacter(commandContext.User, coordinate);
				DisplayPlayerView(commandContext.User, gameManager, commandContext.Channel);
			}
		}

		[Command("RPGMap"), Description(""), CommandCustomGroupAttribute("RPGCommands")]
		public async Task ShowFullMap(CommandContext commandContext)
		{
			if (_activeGames == null || !_activeGames.ContainsKey(commandContext.Channel))
			{
				await commandContext.Channel.SendMessageAsync("No game is currently busy on this channel. Use the [RPGStart] Command first");
				return;
			}
			MRPGGameManager gameManager = _activeGames[commandContext.Channel];

			string playerViewFilePath = gameManager.VisualizeFullMap();
			using (FileStream playerViewStream = File.Open(playerViewFilePath, FileMode.Open))
			{
				DiscordMessageBuilder messageBuilder = new DiscordMessageBuilder();
				messageBuilder.WithFile(playerViewStream);
				await commandContext.Channel.SendMessageAsync(messageBuilder);
			}

		}

		private async void DisplayPlayerView(DiscordUser player, MRPGGameManager activeGame, DiscordChannel channelToSend)
		{
			string playerViewFilePath = activeGame.VisualizePlayerView(player);
			using (FileStream playerViewStream = File.Open(playerViewFilePath, FileMode.Open))
			{
				DiscordMessageBuilder messageBuilder = new DiscordMessageBuilder();
				messageBuilder.WithFile(playerViewStream);
				await channelToSend.SendMessageAsync(messageBuilder);
			}
		}
	}
}