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
		public enum GameDifficulties { Easy = 20, Medium = 50, Hard = 70 }
		public GameState GameStatus { get; private set; }
		public GameDifficulties GameDifficulty { get; private set; }
		public GameDifficulties RoomDifficulty { get; private set; }
		public DiscordUser PlayersTurn { get { return _currentPlayers.Keys.ElementAt(_turnOrderIndex); } }
		private Dictionary<DiscordUser, MRPGCharacter> _currentPlayers;
		private List<MRPGCharacter> _charactersInGame;
		private MRPGMapGenerator _mRPGMap;
		private MRPGMapVisualizer _mapVisualizer;
		private DiscordChannel _playingInChannel;
		private string _fullMapLocation;
		private Bitmap playerToken;
		private Bitmap _orcToken;
		private int _turnOrderIndex;
		private int _movementPerTile;

		public MRPGGameManager(DiscordChannel channel)
		{
			GameStatus = GameState.Lobby;
			_mRPGMap = new MRPGMapGenerator();
			_currentPlayers = new Dictionary<DiscordUser, MRPGCharacter>();
			_charactersInGame = new List<MRPGCharacter>(); 
			_mapVisualizer = new MRPGMapVisualizer();
			playerToken = new Bitmap(Environment.CurrentDirectory + "\\DataObjects\\RPGTokens\\HumanWarrior.png");
			_orcToken = new Bitmap(Environment.CurrentDirectory + "\\DataObjects\\RPGTokens\\OrcGrunt.png");
			_fullMapLocation = Environment.CurrentDirectory + "\\DataObjects\\RPGMaps\\" + channel.Id + ".png";
			_playingInChannel = channel;
			_turnOrderIndex = 0;
			_movementPerTile = 5;
			GameDifficulty = GameDifficulties.Medium;
			RoomDifficulty = GameDifficulties.Easy;
		}

		#region Visualisation
		public string VisualizeFullMap()
		{
			string mapWithTokensPath = Environment.CurrentDirectory + "\\DataObjects\\RPGMaps\\" + _playingInChannel.Id + "-Tokens.png";
			Bitmap mapWithTokens = _mapVisualizer.PlaceTokens(new Bitmap(_fullMapLocation), new List<MRPGToken>(_charactersInGame));
			using (FileStream saveStream = File.Create(mapWithTokensPath))
			{
				mapWithTokens.Save(saveStream, System.Drawing.Imaging.ImageFormat.Png);
			}
			return mapWithTokensPath;
		}

		public string VisualizePlayerView(DiscordUser discordUser)
		{
			Bitmap mapWithTokens = _mapVisualizer.PlaceTokens(new Bitmap(_fullMapLocation), new List<MRPGToken>(_charactersInGame));
			using (FileStream saveStream = File.Create(Environment.CurrentDirectory + "\\DataObjects\\RPGMaps\\" + _playingInChannel.Id + "-" + discordUser.Id + ".png"))
			{
				Bitmap playerView = _mapVisualizer.VisualizePlayerView(mapWithTokens, _currentPlayers[discordUser]);
				playerView.Save(saveStream, System.Drawing.Imaging.ImageFormat.Png);
			}
			return Environment.CurrentDirectory + "\\DataObjects\\RPGMaps\\" + _playingInChannel.Id + "-" + discordUser.Id + ".png";
		}
		#endregion

		#region InternalInteractions
		public void AddPlayer(DiscordUser discordUser)
		{
			if(!_currentPlayers.ContainsKey(discordUser))
			{
				MRPGPlayerCharacters playerCharacter = new MRPGPlayerCharacters(playerToken);
				_currentPlayers.Add(discordUser, playerCharacter);
				_charactersInGame.Add(playerCharacter);
			}
		}

		public List<DiscordUser> getCurrentPlayers()
		{
			return new List<DiscordUser>(_currentPlayers.Keys);
		}

		public void StartNewGame()
		{
			GameStatus = GameState.Ingame;
			_mRPGMap.GenerateNewMap(15, 30, 30);
			Bitmap background = _mapVisualizer.VisualizeBackGround(_mRPGMap);

			for (int i = 0; i < _currentPlayers.Values.Count; i++)
			{
				_currentPlayers.ElementAt(i).Value.SetLocation(_mRPGMap.StartingRoom.PlayerSpawnLocations[i].position);
			}

			List<MRPGRoom> possibleEnemyRooms = _mRPGMap.Rooms.Where(room => room.EnemySpawnLocations != null && room.EnemySpawnLocations.Count > 0).ToList();
			int roomsWithEnemies = (int)MathF.Round((int)GameDifficulty / 100.0f * possibleEnemyRooms.Count, 0);
			Random numberGenerator = new Random();

			for (int i = 0; i < roomsWithEnemies; i++)
			{
				MRPGRoom selectedRoom = possibleEnemyRooms[numberGenerator.Next(0, possibleEnemyRooms.Count)];
				List<MRPGMapTile> possibleSpawnLocations = selectedRoom.EnemySpawnLocations;
				int enemiesInRoom = (int)MathF.Round((int)RoomDifficulty / 100.0f * selectedRoom.EnemySpawnLocations.Count, 0);
				for (int j = 0; j < enemiesInRoom; j++)
				{
					MRPGOrcGrunt orcGrunt = new MRPGOrcGrunt(_orcToken);
					orcGrunt.SetLocation(possibleSpawnLocations[numberGenerator.Next(0, possibleSpawnLocations.Count)].position);
					_charactersInGame.Add(orcGrunt);
				}
				possibleEnemyRooms.Remove(selectedRoom);
			}

			using (FileStream saveStream = File.Create(_fullMapLocation))
			{
				background.Save(saveStream, System.Drawing.Imaging.ImageFormat.Png);
			}
		}

		public MRPGIntVector2 CoordinateToTile(string coordinate, DiscordUser fromUser)
		{
			MRPGCharacter controlledCharacter = _currentPlayers[fromUser];

			int CenterLetter = 65 + _currentPlayers[fromUser].SightRadius;
			int horizontalCoordinate = (int)coordinate.ToUpper()[0];
			int verticalCoordinate = Convert.ToInt32(coordinate[1].ToString());
			int horizontalOffset = horizontalCoordinate - CenterLetter;
			int verticalOffset = verticalCoordinate - 1 - controlledCharacter.SightRadius;
			MRPGIntVector2 offsetVector = new MRPGIntVector2(horizontalOffset, verticalOffset);
			return controlledCharacter.GridLocation + offsetVector;
		}

		public void LoadSaveFile()
		{
			GameStatus = GameState.Ingame;
		}
		#endregion

		#region PlayerActions
		public bool MoveCharacter(DiscordUser user, string coordinate, out bool locationWalkable, out int movementLeft)
		{
			locationWalkable = true;
			movementLeft = 0;

			if(_currentPlayers.ContainsKey(user))
			{
				MRPGCharacter controlledCharacter = _currentPlayers[user];
				MRPGPathFinder pathFinder = new MRPGPathFinder(_mRPGMap.GeneratedMap);
				MRPGIntVector2 newLocation = CoordinateToTile(coordinate, user);
				List<MRPGMapTile> pathToTarget = pathFinder.FindPath(newLocation, controlledCharacter.GridLocation, true, false);
				
				if(_mRPGMap.GeneratedMap[newLocation.X][newLocation.Y].TileFuntion != null)
				{
					if (_currentPlayers[user].CurrentMovement >= pathToTarget.Count * _movementPerTile)
					{
						_currentPlayers[user].MoveCharacter(newLocation, pathToTarget.Count * _movementPerTile);
						movementLeft = _currentPlayers[user].CurrentMovement;
						return true;
					}
				}
				else
				{
					locationWalkable = false;
				}
				movementLeft = _currentPlayers[user].CurrentMovement;
			}
			return false;
		}

		public (bool targetInRange, bool tokenPresentToAttack, bool attackHit, int rollToHit, int damageDealt) AttackTileLocation(DiscordUser attacker, string coordinate)
		{
			MRPGIntVector2 tileUnderAttack = CoordinateToTile(coordinate, attacker);
			MRPGCharacter controlledCharacter = _currentPlayers[attacker];
			List<MRPGMapTile> surroundingTiles = new List<MRPGMapTile>();
			MRPGMapTile tileOfAttacker = _mRPGMap.GetTileAtLocation(controlledCharacter.GridLocation);
			surroundingTiles.Add(tileOfAttacker);

			for (int i = 0; i < controlledCharacter.AttackRange; i++)
			{
				List<MRPGMapTile> tileSurroundingSpecificTile = new List<MRPGMapTile>();
				foreach (MRPGMapTile tile in surroundingTiles)
				{
					tileSurroundingSpecificTile.AddRange(_mRPGMap.GetAdjacentTiles(tile, true));
				}

				foreach (MRPGMapTile tile in tileSurroundingSpecificTile)
				{
					if (surroundingTiles.Contains(tile)) continue;
					surroundingTiles.Add(tile);
				}
			}
			surroundingTiles.Remove(tileOfAttacker);

			if(!surroundingTiles.Contains(_mRPGMap.GetTileAtLocation(tileUnderAttack)))
			{
				//Target out of range
				return (false, false, false, 0, 0);
			}

			MRPGCharacter potentialCharacterUnderAttack = _charactersInGame.FirstOrDefault(character => character.GridLocation.Equals(tileUnderAttack));
			if (potentialCharacterUnderAttack == null)
			{
				//Nobody there to attack
				return (true, false, false, 0, 0);
			}

			int hitRoll;
			int damageDealt;
			bool attackHit = controlledCharacter.AttackEnemy(potentialCharacterUnderAttack, out hitRoll, out damageDealt);
			if(!attackHit)
			{
				//Attack missed
				return (true, true, false, hitRoll, damageDealt);
			}
			else
			{
				//Attack landed;
				return (true, true, true, hitRoll, damageDealt);
			}
		}

		public void EndTurn(DiscordUser user)
		{
			MRPGCharacter controlledCharacter = _currentPlayers[user];
			controlledCharacter.ResetOnTurn();

			_turnOrderIndex = _turnOrderIndex + 1 == _currentPlayers.Count ? 0 : _turnOrderIndex + 1;
		}
		#endregion
	}
}