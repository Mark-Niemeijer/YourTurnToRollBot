using DSharpPlus;
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
		private readonly string _joinLobbyButtonID = "RPGJoinLobby";
		private readonly string _createNewWorldButtonID = "RPGNewGame";
		private readonly string _loadGameButtonID = "RPGLoadGame";

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

			ButtonPressedEvent.AddListenerer(_joinLobbyButtonID, JoinLobby);
			ButtonPressedEvent.AddListenerer(_createNewWorldButtonID, CreateNewDungeon);

			DiscordMessageBuilder lobbyMessage = new DiscordMessageBuilder();
			lobbyMessage.WithContent("Welcome to the " + commandContext.Client.CurrentUser.Mention + " tavern, are we starting a new adventure or continuing an old one?\n" +
				"Make sure everyone is around the table before you start.");
			DiscordButtonComponent joinButton = new DiscordButtonComponent(DSharpPlus.ButtonStyle.Primary, _joinLobbyButtonID, "Join this adventure");
			DiscordButtonComponent startNewButton = new DiscordButtonComponent(DSharpPlus.ButtonStyle.Primary, _createNewWorldButtonID, "Delve through a new dungeon");
			DiscordButtonComponent loadGame = new DiscordButtonComponent(DSharpPlus.ButtonStyle.Primary, _loadGameButtonID, "Revisit an old dungeon");
			lobbyMessage.AddComponents(new DiscordComponent[] { joinButton, startNewButton, loadGame });

			await commandContext.Channel.SendMessageAsync(lobbyMessage);
			await JoinGame(commandContext);
		}

		[Command("RPGJoin"), Description(""), CommandCustomGroupAttribute("RPGCommands")]
		public async Task JoinGame(CommandContext commandContext)
		{
			JoinLobby(commandContext.Client, commandContext.Channel, commandContext.User);
		}

		[Command("RPGNewGame"), Description(""), CommandCustomGroupAttribute("RPGCommands")]
		public async Task BeginNewGame(CommandContext commandContext)
		{
			CreateNewDungeon(commandContext.Client, commandContext.Channel, commandContext.User);
		}

		[Command("RPGMove"), Description(""), CommandCustomGroupAttribute("RPGCommands")]
		public async Task MoveCharacter(CommandContext commandContext, string coordinate)
		{
			//For future, let people move by reacting to a message with the letter emoticons which are preplaced by the bot
			var validationReturn = await PerformInGameValidation(commandContext);
			if (!validationReturn.ValidationSucces) return;

			if (coordinate.Length == 2 && Char.IsLetter(coordinate[0]) && Char.IsDigit(coordinate[1]))
			{
				await commandContext.Channel.SendMessageAsync("Valid Coordinate Given");
				int movementLeft;
				bool locationAvailable;
				bool moveSuccesful = validationReturn.gameManager.MoveCharacter(commandContext.User, coordinate, out locationAvailable, out movementLeft);
				if (!moveSuccesful)
				{
					if (!locationAvailable)
					{
						await commandContext.Channel.SendMessageAsync("You cannot move into a wall, try a more open spot");
						return;
					}
					else
					{
						await commandContext.Channel.SendMessageAsync("You don't have enough move points left to go that far");
						return;
					}
				}
				else
				{
					await DisplayPlayerView(commandContext.User, validationReturn.gameManager, commandContext.Channel);
					await commandContext.Channel.SendMessageAsync("Put you right there, you still have " + movementLeft + " feet left");
					return;
				}
			}
			else
			{
				await commandContext.Channel.SendMessageAsync("That's not a valid coordinate. The letter comes first, then the number");
				return;
			}
		}

		[Command("RPGAttack"), Description(""), CommandCustomGroupAttribute("RPGCommands")]
		public async Task AttackCharacterCommand(CommandContext commandContext, string coordinate)
		{
			var validationReturn = await PerformInGameValidation(commandContext);
			if (!validationReturn.ValidationSucces) return;
			if (coordinate.Length != 2 || !Char.IsLetter(coordinate[0]) || !Char.IsDigit(coordinate[1]))
			{
				await commandContext.Channel.SendMessageAsync("That's not a valid coordinate. The letter comes first, then the number");
				return;
			}

			var resultAttackTile = validationReturn.gameManager.AttackTileLocation(commandContext.User, coordinate);
			if (!resultAttackTile.targetInRange)
			{
				await commandContext.Channel.SendMessageAsync("They are to far away for you to hit them");
				return;
			}
			else if(!resultAttackTile.tokenPresentToAttack)
			{
				await commandContext.Channel.SendMessageAsync("You hit your weapon hard against the stone of the dungeon. It would be insulted if it could feel things");
				return;
			}
			else if (!resultAttackTile.attackHit)
			{
				await commandContext.Channel.SendMessageAsync(String.Format("You swing your weapon for {0} at the target, but they dodge out of the way", resultAttackTile.rollToHit));
				return;
			}
			else
			{
				await commandContext.Channel.SendMessageAsync(String.Format("You swing for {0} and hit the target!\nYou dealt {1} damage to them", resultAttackTile.rollToHit, resultAttackTile.damageDealt));
				return;
			}
		}

		[Command("RPGEndTurn"), Description(""), CommandCustomGroupAttribute("RPGCommands")]
		public async Task EndTurn(CommandContext commandContext)
		{
			var validationReturn = await PerformInGameValidation(commandContext);
			if (!validationReturn.ValidationSucces) return;

			validationReturn.gameManager.EndTurn(commandContext.User);
			await commandContext.Channel.SendMessageAsync(validationReturn.gameManager.PlayersTurn.Mention + ", its your turn to go now");
			await DisplayPlayerView(validationReturn.gameManager.PlayersTurn, validationReturn.gameManager, commandContext.Channel);
		}

		[Command("RPGMap"), Description(""), CommandCustomGroupAttribute("RPGCommands")]
		public async Task ShowFullMap(CommandContext commandContext)
		{
			var validationReturn = await PerformInGameValidation(commandContext);
			if (!validationReturn.ValidationSucces) return;

			string playerViewFilePath = validationReturn.gameManager.VisualizeFullMap();
			using (FileStream playerViewStream = File.Open(playerViewFilePath, FileMode.Open))
			{
				DiscordMessageBuilder messageBuilder = new DiscordMessageBuilder();
				messageBuilder.WithFile(playerViewStream);
				await commandContext.Channel.SendMessageAsync(messageBuilder);
			}

		}

		private async Task<(bool ValidationSucces, MRPGGameManager gameManager)> PerformInGameValidation(CommandContext commandContext)
		{
			if (_activeGames == null || !_activeGames.ContainsKey(commandContext.Channel))
			{
				await commandContext.Channel.SendMessageAsync("No game is currently busy on this channel. Use the [RPGStart] Command first");
				return (false, null);
			}
			MRPGGameManager gameManager = _activeGames[commandContext.Channel];
			if (gameManager.PlayersTurn != commandContext.User)
			{
				await commandContext.Channel.SendMessageAsync("It is not your turn yet, be patient");
				return (false, null);
			}
			return (true, gameManager);
		}

		private async void JoinLobby(DiscordClient botClient, DiscordChannel channelButtonWasPressedIn, DiscordUser userPressingTheButton)
		{
			if (_activeGames == null || !_activeGames.ContainsKey(channelButtonWasPressedIn))
			{
				await channelButtonWasPressedIn.SendMessageAsync("No game is currently busy on this channel. Use the [RPGStart] Command first");
				return;
			}
			MRPGGameManager gameManager = _activeGames[channelButtonWasPressedIn];

			if (gameManager.GameStatus == MRPGGameManager.GameState.Lobby)
			{
				List<DiscordUser> playersInGame = gameManager.getCurrentPlayers();
				if (playersInGame.Contains(userPressingTheButton))
				{
					await channelButtonWasPressedIn.SendMessageAsync("You are already at the table " + userPressingTheButton.Mention);
				}
				else
				{
					await channelButtonWasPressedIn.SendMessageAsync("Welcome to the table " + userPressingTheButton.Mention + ", lets have a great adventure");
					gameManager.AddPlayer(userPressingTheButton);
				}
			}
			else
			{
				await channelButtonWasPressedIn.SendMessageAsync("I am sorry, but the game has already begun. Next dungeon you can play along");
			}
		}

		private async void CreateNewDungeon(DiscordClient botClient, DiscordChannel channelButtonWasPressedIn, DiscordUser userPressingTheButton)
		{
			if (_activeGames == null || !_activeGames.ContainsKey(channelButtonWasPressedIn))
			{
				await channelButtonWasPressedIn.SendMessageAsync("No game is currently busy on this channel. Use the [RPGStart] Command first");
				return;
			}
			MRPGGameManager gameManager = _activeGames[channelButtonWasPressedIn];

			if (gameManager.GameStatus == MRPGGameManager.GameState.Lobby)
			{
				await channelButtonWasPressedIn.SendMessageAsync("I am grabbing my tiles and tokens, hold on a second");
				gameManager.StartNewGame();
				await channelButtonWasPressedIn.SendMessageAsync("I prepared the dungeon, here it comes");
				await DisplayPlayerView(gameManager.PlayersTurn, gameManager, channelButtonWasPressedIn);
			}
			else
			{
				await channelButtonWasPressedIn.SendMessageAsync("The adventure has already begun, no need to start it");
			}
		}

		private async Task<bool> DisplayPlayerView(DiscordUser player, MRPGGameManager activeGame, DiscordChannel channelToSend)
		{
			string playerViewFilePath = activeGame.VisualizePlayerView(player);
			using (FileStream playerViewStream = File.Open(playerViewFilePath, FileMode.Open))
			{
				DiscordMessageBuilder messageBuilder = new DiscordMessageBuilder();
				messageBuilder.WithFile(playerViewStream);
				await channelToSend.SendMessageAsync(messageBuilder);
			}
			return true;
		}
	}
}