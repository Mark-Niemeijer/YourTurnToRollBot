using DSharpPlus.Entities;
using No_u_discord_bot.MushroomRPG;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace No_u_discord_bot.InBotAppManagers
{
	class MRPGGameManager
	{
		public enum GameState { Lobby, Ingame }
		public GameState GameStatus { get; private set; }
		private Dictionary<DiscordUser, MRPGCharacter> _currentPlayers;
		private MRPGMapGenerator _mRPGMap;
		private MRPGMapVisualizer _mapVisualizer;
		private DiscordChannel _playingInChannel;
		private string _fullMapLocation;
		private Bitmap playerToken;
		private DiscordUser _currentUserTurn;

		public MRPGGameManager(DiscordChannel channel)
		{
			GameStatus = GameState.Lobby;
			_mRPGMap = new MRPGMapGenerator();
			_currentPlayers = new Dictionary<DiscordUser, MRPGCharacter>();
			_mapVisualizer = new MRPGMapVisualizer();
			playerToken = new Bitmap(Environment.CurrentDirectory + "\\DataObjects\\RPGTokens\\HumanWarrior.png");
			_fullMapLocation = Environment.CurrentDirectory + "\\DataObjects\\RPGMaps\\" + channel.Id + ".png";
			_playingInChannel = channel;
		}

		public void StartNewGame()
		{
			GameStatus = GameState.Ingame;
			_mRPGMap.GenerateNewMap(15, 30, 30);
			Bitmap background = _mapVisualizer.VisualizeBackGround(_mRPGMap);

			for (int i = 0; i < _currentPlayers.Values.Count; i++)
			{
				_currentPlayers.ElementAt(i).Value.SetLocation(_mRPGMap.StartingRoom.RoomTiles[i].position);
			}

			using (FileStream saveStream = File.Create(_fullMapLocation))
			{
				background.Save(saveStream, System.Drawing.Imaging.ImageFormat.Png);
			}
		}

		public void LoadSaveFile()
		{
			GameStatus = GameState.Ingame;
		}

		public string VisualizeFullMap()
		{
			string mapWithTokensPath = Environment.CurrentDirectory + "\\DataObjects\\RPGMaps\\" + _playingInChannel.Id + "-Tokens.png";
			Bitmap mapWithTokens = _mapVisualizer.PlaceTokens(new Bitmap(_fullMapLocation), new List<MRPGToken>(_currentPlayers.Values));
			using (FileStream saveStream = File.Create(mapWithTokensPath))
			{
				mapWithTokens.Save(saveStream, System.Drawing.Imaging.ImageFormat.Png);
			}
			return mapWithTokensPath;
		}

		public string VisualizePlayerView(DiscordUser discordUser)
		{
			Bitmap mapWithTokens = _mapVisualizer.PlaceTokens(new Bitmap(_fullMapLocation), new List<MRPGToken>(_currentPlayers.Values));
			using (FileStream saveStream = File.Create(Environment.CurrentDirectory + "\\DataObjects\\RPGMaps\\" + _playingInChannel.Id + "-" + discordUser.Id + ".png"))
			{
				Bitmap playerView = _mapVisualizer.VisualizePlayerView(mapWithTokens, _currentPlayers[discordUser]);
				playerView.Save(saveStream, System.Drawing.Imaging.ImageFormat.Png);
			}
			return Environment.CurrentDirectory + "\\DataObjects\\RPGMaps\\" + _playingInChannel.Id + "-" + discordUser.Id + ".png";
		}

		public void AddPlayer(DiscordUser discordUser)
		{
			if(!_currentPlayers.ContainsKey(discordUser))
			{
				MRPGCharacter playerCharacter = new MRPGCharacter(playerToken);
				playerCharacter.SightRadius = 3;
				playerCharacter.MaxMovement = 30;
				playerCharacter.CurrentMovement = playerCharacter.MaxMovement;
				_currentPlayers.Add(discordUser, playerCharacter);
			}

			if(_currentUserTurn == null)
			{
				_currentUserTurn = discordUser;
			}
		}

		public List<DiscordUser> getCurrentPlayers()
		{
			return new List<DiscordUser>(_currentPlayers.Keys);
		}

		public void MoveCharacter(DiscordUser user, string coordinate)
		{
			if(_currentPlayers.ContainsKey(user) && _currentPlayers[user].CurrentMovement > 0)
			{
				MRPGCharacter controlledCharacter = _currentPlayers[user];
				int CenterLetter = 65 + _currentPlayers[user].SightRadius;
				int horizontalCoordinate = (int)coordinate.ToUpper()[0];
				int verticalCoordinate = Convert.ToInt32(coordinate[1].ToString());
				int horizontalOffset = horizontalCoordinate - CenterLetter;
				int verticalOffset = verticalCoordinate - 1 - controlledCharacter.SightRadius;
				MRPGIntVector2 offsetVector = new MRPGIntVector2(horizontalOffset, verticalOffset);
				MRPGIntVector2 newLocation = controlledCharacter.GridLocation + offsetVector;
				
				if(_mRPGMap.GeneratedMap[newLocation.X][newLocation.Y].TileFuntion != null)
				{
					controlledCharacter.SetLocation(newLocation);
				}
			}
		}
	}
}