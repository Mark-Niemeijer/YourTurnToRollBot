using No_u_discord_bot.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace No_u_discord_bot.MushroomRPG
{
	class MRPGPathFinder
	{
		private Dictionary<int, List<MRPGMapTile>> _map;
		public MRPGPathFinder(Dictionary<int, List<MRPGMapTile>> map)
		{
			_map = map;
		}

		public List<MRPGMapTile> FindPath(MRPGIntVector2 start, MRPGIntVector2 end, bool includeRooms, bool includeWalls)
		{
			List<MRPGMapTile> path = new List<MRPGMapTile>();
			Dictionary<MRPGMapTile, float> distanceFromStartList = new Dictionary<MRPGMapTile, float>();

			MRPGMapTile startTile = _map[start.X][start.Y];
			MRPGMapTile endTile = _map[end.X][end.Y];
			distanceFromStartList.Add(startTile, 0);

			// Assign each tile a value based on distance (and modifiers) from the starting point 
			for (int i = 0; i < distanceFromStartList.Count; i++)
			{
				MRPGMapTile currentTile = distanceFromStartList.ElementAt(i).Key;
				List<MRPGMapTile> surroundingTiles = retrieveSurroundingTiles(distanceFromStartList.ElementAt(i).Key, includeRooms, includeWalls);
				foreach (MRPGMapTile tile in surroundingTiles)
				{
					bool tileIsPath = tile.TileFuntion != null && tile.TileFuntion.GetType() == typeof(MRPGPath);
					float newDistance = tileIsPath ? distanceFromStartList[currentTile] + 0.5f : distanceFromStartList[currentTile] + 1f;

					// Re-calculate values if a shorter path is available
					if (distanceFromStartList.ContainsKey(tile) && newDistance < distanceFromStartList[tile])
					{
						distanceFromStartList.Remove(tile);
						distanceFromStartList.Add(tile, newDistance);
						i--;
					}
					else if (!distanceFromStartList.ContainsKey(tile))
					{
						distanceFromStartList.Add(tile, newDistance);
					}
				}
			}

			if(distanceFromStartList.Count == 1)
			{
				CustomDebugInfo.LogError("Rooms pasted together go wrong");
				return path;
			}

			bool distanceIsZero = false;
			MRPGMapTile nextStep = endTile;
			float smallestDistance = float.MaxValue;

			// Follow the values back to the starting point
			while (!distanceIsZero)
			{
				List<MRPGMapTile> surroundingTiles = retrieveSurroundingTiles(nextStep, includeRooms, includeWalls);
				foreach (MRPGMapTile tile in surroundingTiles)
				{
					float distance = distanceFromStartList[tile];
					if(distance < smallestDistance)
					{
						smallestDistance = distance;
						nextStep = tile;
					}
				}

				if(path.Count > 0 && nextStep == path[path.Count - 1])
				{
					CustomDebugInfo.LogWarning("The pathfinder could not find the starting point, this is expected for map generation. Otherwise something went wrong");
					distanceIsZero = true;
				}

				path.Add(nextStep);
				if(nextStep == startTile)
				{
					distanceIsZero = true;
				}
			}
			

			return path;
		}

		/// <summary>
		/// Return the tiles surrounding the given one, ignoring rooms or walls if needed
		/// </summary>
		/// <param name="currentTile">Center tile to get surrounding ones from</param>
		/// <param name="ignoreRooms">Add tiles that have a room in them</param>
		/// <param name="ignoreWalls">Add tiles that have walls in them</param>
		/// <returns></returns>
		private List<MRPGMapTile> retrieveSurroundingTiles(MRPGMapTile currentTile, bool includeRooms, bool includeWalls)
		{
			List<MRPGMapTile> surroundingTiles = new List<MRPGMapTile>();
			if (currentTile.position.Y + 1 < _map[currentTile.position.X].Count)
			{
				surroundingTiles.Add(_map[currentTile.position.X][currentTile.position.Y + 1]);
			}

			if (currentTile.position.Y - 1 >= 0)
			{
				surroundingTiles.Add(_map[currentTile.position.X][currentTile.position.Y - 1]);
			}

			if (currentTile.position.X + 1 < _map.Keys.Count)
			{
				surroundingTiles.Add(_map[currentTile.position.X + 1][currentTile.position.Y]);
			}

			if (currentTile.position.X - 1 >= 0)
			{
				surroundingTiles.Add(_map[currentTile.position.X - 1][currentTile.position.Y]);
			}

			for (int i = 0; i < surroundingTiles.Count; i++)
			{
				if(surroundingTiles[i].TileFuntion == null)
				{
					if(!includeWalls)
					{
						surroundingTiles.RemoveAt(i);
						i--;
					}
					continue;
				}

				if(surroundingTiles[i].TileFuntion.GetType() == typeof(MRPGRoom) && !includeRooms)
				{
					surroundingTiles.RemoveAt(i);
					i--;
					continue;
				}
			}

			return surroundingTiles;
		}
	}
}
