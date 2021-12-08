using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using No_u_discord_bot.DataObjects;
using No_u_discord_bot.Helpers;
using No_u_discord_bot.InBotAppManagers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace No_u_discord_bot.Commands
{
	class HangManCommands : BaseCommandModule
	{
		private Dictionary<DiscordChannel, HangManManager> gameManagers;

		[Command("StartHM"), Description("Syntax: $[Command]\nPrepare a lobby for hangman in this channel"), CommandCustomGroupAttribute("Hangman")]
		public async Task StartGame(CommandContext commandContext)
		{
			if (gameManagers == null) gameManagers = new Dictionary<DiscordChannel, HangManManager>();
			if (gameManagers.ContainsKey(commandContext.Channel))
			{
				await commandContext.Channel.SendMessageAsync("I am already running a game on this channel. Should have joined").ConfigureAwait(false);
				return;
			}

			HangManManager hangManManager = new HangManManager(7);
			WordListJson loadedWordList = JsonParser.GetInstance().LoadData<WordListJson>(JsonParser.FileEnum.HangManWordsFile);

			hangManManager.SetWordList(loadedWordList.WordArray);
			gameManagers.Add(commandContext.Channel, hangManManager);

			await commandContext.Channel.SendMessageAsync("I am ready to host a game of hangman. Everyone who wants to join should type '$join' or '/join'\n" +
														  "When you are ready to start, type '$begin' or '/begin'").ConfigureAwait(false);
			await PlayerJoin(commandContext);

			if (commandContext.Channel.IsPrivate)
			{
				CustomDebugInfo.LogInfo("Hangman lobby started in DM channel from " + commandContext.User.Username);
			}
			else
			{
				CustomDebugInfo.LogInfo("Hangman lobby started in '" + commandContext.Channel.Name + "' within '" + commandContext.Channel.Guild.Name + "'");
			}
		}

		[Command("Join"), Description("Syntax: $[Command]\nJoin a game running in this channel"), CommandCustomGroupAttribute("Hangman")]
		public async Task PlayerJoin(CommandContext commandContext)
		{
			HangManManager hangManManager;
			bool gamePresent = gameManagers.TryGetValue(commandContext.Channel, out hangManManager);
			if (!gamePresent || hangManManager.CurrentStatus != HangManManager.GameStatus.Lobby) return;

			bool joiningSuccesful = hangManManager.JoinGame(commandContext.Message.Author);

			if(joiningSuccesful)
			{
				await commandContext.Channel.SendMessageAsync("Welcome to the game " + commandContext.Message.Author.Mention).ConfigureAwait(false);
			}
			else
			{
				await commandContext.Channel.SendMessageAsync("You are already a part of the game " + commandContext.Message.Author.Mention).ConfigureAwait(false);
			}
		}

		[Command("Begin"), Description("Syntax: $[Command]\nStarts the game of hangman on this channel. Must be in lobby to work"), CommandCustomGroupAttribute("Hangman")]
		public async Task BeginGame(CommandContext commandContext)
		{
			HangManManager hangManManager;
			bool gamePresent = gameManagers.TryGetValue(commandContext.Channel, out hangManManager);
			if (!gamePresent || hangManManager.CurrentStatus != HangManManager.GameStatus.Lobby) return;

			if (hangManManager.PlayersInGame.Count > 0)
			{
				hangManManager.BeginGame();
				await commandContext.Channel.SendMessageAsync("Amount of letters: " + hangManManager.PlayerGuessedResult.Count).ConfigureAwait(false);
				await PrintCurrentOutcome(commandContext, hangManManager);

				if (commandContext.Channel.IsPrivate)
				{
					CustomDebugInfo.LogInfo("Hangman game started in DM channel from " + commandContext.User.Username);
				}
				else
				{
					CustomDebugInfo.LogInfo("Hangman game started in '" + commandContext.Channel.Name + "' within '" + commandContext.Channel.Guild.Name + "'");
				}
			}
			else
			{
				await commandContext.Channel.SendMessageAsync("People first need to join. I am not playing this on my own").ConfigureAwait(false);
			}
		}

		[Command("Guess"), Aliases("G"), Description("Syntax: $[Command] [Letter you want to guess]\nGuess a letter in a running game. Must be in the game and your turn"), CommandCustomGroupAttribute("Hangman")]
		public async Task GuessLetter(CommandContext commandContext, [Description("The letter/word you want to guess")] string playerGuess)
		{
			HangManManager hangManManager;
			bool gamePresent = gameManagers.TryGetValue(commandContext.Channel, out hangManManager);
			if (!gamePresent || hangManManager.CurrentStatus != HangManManager.GameStatus.InGame || commandContext.Message.Author != hangManManager.CurrentsPlayerTurn) return;
			playerGuess = playerGuess.Trim();
			playerGuess = playerGuess.ToLower();

			if (playerGuess.Length == 1)
			{
				bool letterCorrect = hangManManager.GuessLetter(playerGuess[0]);
				if(letterCorrect)
				{
					await commandContext.Channel.SendMessageAsync("Good guess :p").ConfigureAwait(false);
				}
				else
				{
					await commandContext.Channel.SendMessageAsync("I am sorry, but it is not there").ConfigureAwait(false);
				}
			}
			else
			{
				bool wordCorrect = hangManManager.GuessWord(playerGuess);
				if (wordCorrect)
				{
					await commandContext.Channel.SendMessageAsync("YOU DID IT, You won the game :partying_face:").ConfigureAwait(false);
					gameManagers.Remove(commandContext.Channel);

					if (commandContext.Channel.IsPrivate)
					{
						CustomDebugInfo.LogInfo("Hangman game won by player in DM channel from " + commandContext.User.Username);
					}
					else
					{
						CustomDebugInfo.LogInfo("Hangman game won by player(s) in '" + commandContext.Channel.Name + "' within '" + commandContext.Channel.Guild.Name + "'");
					}
				}
				else
				{
					await commandContext.Channel.SendMessageAsync("I am sorry, but that is not correct").ConfigureAwait(false);
				}
			}

			if(hangManManager.CurrentStatus == HangManManager.GameStatus.GameWon)
			{
				await commandContext.Channel.SendMessageAsync("Looks like I won this time :p\nThe word was: " + hangManManager.SelectedWord).ConfigureAwait(false);
				gameManagers.Remove(commandContext.Channel);

				if (commandContext.Channel.IsPrivate)
				{
					CustomDebugInfo.LogInfo("Hangman game lost by player in DM channel from " + commandContext.User.Username);
				}
				else
				{
					CustomDebugInfo.LogInfo("Hangman game lost by player(s) in '" + commandContext.Channel.Name + "' within '" + commandContext.Channel.Guild.Name + "'");
				}
			}
			else if (hangManManager.CurrentStatus == HangManManager.GameStatus.InGame)
			{
				hangManManager.SwitchToNextPlayer();
				await PrintCurrentOutcome(commandContext, hangManManager);
			}
		}

		[Command("Reprint"), Aliases("RP"), Description("Syntax: $[Command]\nReprints the current state of the game. A game must be running at this moment"), CommandCustomGroupAttribute("Hangman")]
		public async Task ReprintInfo(CommandContext commandContext)
		{
			HangManManager hangManManager;
			bool gamePresent = gameManagers.TryGetValue(commandContext.Channel, out hangManManager);
			if (!gamePresent || hangManManager.CurrentStatus != HangManManager.GameStatus.InGame) return;

			await PrintCurrentOutcome(commandContext, hangManManager);
		}

		[Command("StopHM"), Description("Stops a currently running game. Only the host of the game can stop it"), CommandCustomGroupAttribute("Hangman")]
		public async Task StopGame(CommandContext commandContext)
		{
			HangManManager hangManManager;
			bool gamePresent = gameManagers.TryGetValue(commandContext.Channel, out hangManManager);
			if (!gamePresent || hangManManager.CurrentStatus != HangManManager.GameStatus.InGame || commandContext.Message.Author != hangManManager.PlayersInGame[0]) return;

			hangManManager.EndGame(false);
			gameManagers.Remove(commandContext.Channel); 
			await commandContext.Channel.SendMessageAsync("Time to stop early? Alright then :p").ConfigureAwait(false);

			if (commandContext.Channel.IsPrivate)
			{
				CustomDebugInfo.LogInfo("Hangman game stopped in DM channel from " + commandContext.User.Username);
			}
			else
			{
				CustomDebugInfo.LogInfo("Hangman game stopped in '" + commandContext.Channel.Name + "' within '" + commandContext.Channel.Guild.Name + "'");
			}
		}

		private async Task PrintCurrentOutcome(CommandContext commandContext, HangManManager hangManManager)
		{
			string incompleteWord = string.Join(' ', hangManManager.PlayerGuessedResult);
			string lettersTried = string.Join(", ", hangManManager.PlayerGuessedLetters);

			await commandContext.Channel.SendMessageAsync("Your tries: " + incompleteWord + "\n" +
				"Attempts Left: " + hangManManager.AttemptsLeft+"\n" +
				"Letters Tried: " + lettersTried + "\n" +
				"Your turn to guess " + hangManManager.CurrentsPlayerTurn.Mention).ConfigureAwait(false);
		}
	}
}
