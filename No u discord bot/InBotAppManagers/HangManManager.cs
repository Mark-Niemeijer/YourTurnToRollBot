using DSharpPlus.Entities;
using No_u_discord_bot.DataObjects;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace No_u_discord_bot.InBotAppManagers
{
	class HangManManager
	{
		public enum GameStatus { Lobby, InGame, GameLost, GameWon }
		public List<DiscordUser> PlayersInGame { get; private set; }
		public GameStatus CurrentStatus { get; private set; }
		public int AttemptsLeft { get; private set; }
		public DiscordUser CurrentsPlayerTurn { get; private set; }
		public List<char> PlayerGuessedLetters { get; private set; }
		public List<char> PlayerGuessedResult { get; private set; }
		private string[] wordList;
		public string SelectedWord { get; set; }
		private int currentPlayerIndex;

		public HangManManager(int totalAttempts)
		{
			PlayersInGame = new List<DiscordUser>();
			CurrentStatus = GameStatus.Lobby;
			PlayerGuessedResult = new List<char>();
			PlayerGuessedLetters = new List<char>();
			AttemptsLeft = totalAttempts;
			currentPlayerIndex = 0;
		}

		public void SetWordList(string[] wordList)
		{
			this.wordList = wordList;
		}

		public void BeginGame()
		{
			CurrentStatus = GameStatus.InGame;
			ChooseRandomWord();
			CurrentsPlayerTurn = PlayersInGame[currentPlayerIndex];
		}

		public bool JoinGame(DiscordUser discordUser)
		{
			if(PlayersInGame.Contains(discordUser))
			{
				return false;
			}
			PlayersInGame.Add(discordUser);
			return true;
		}

		public bool GuessWord(string word)
		{
			if(word == SelectedWord)
			{
				EndGame(true);
				return true;
			}
			GuessFailed();
			return false;
		}

		public bool GuessLetter(char letter)
		{
			if (!PlayerGuessedLetters.Contains(letter))
			{
				PlayerGuessedLetters.Add(letter);
			}

			if (SelectedWord.Contains(letter))
			{
				for (int i = 0; i < SelectedWord.Length; i++)
				{
					if(SelectedWord[i] == letter)
					{
						PlayerGuessedResult[i] = letter;
					}
				}
				return true;
			}
			GuessFailed();
			return false;
		}

		public void SwitchToNextPlayer()
		{
			currentPlayerIndex++;
			if(currentPlayerIndex == PlayersInGame.Count)
			{
				currentPlayerIndex = 0;
			}
			CurrentsPlayerTurn = PlayersInGame[currentPlayerIndex];
		}

		public void EndGame(bool playersWon)
		{
			PlayersInGame = null;
			CurrentStatus = playersWon ? GameStatus.GameLost : GameStatus.GameWon;
			CurrentsPlayerTurn = null;
		}

		private void GuessFailed()
		{
			AttemptsLeft--;
			if(AttemptsLeft == 0)
			{
				EndGame(false);
			}
		}

		private void ChooseRandomWord()
		{
			Random randomGenerator = new Random();
			SelectedWord = wordList[randomGenerator.Next(0, wordList.Length)].ToLower();

			for (int i = 0; i < SelectedWord.Length; i++)
			{
				PlayerGuessedResult.Add('.');
			}
		}
	}
}
