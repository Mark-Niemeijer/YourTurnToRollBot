using No_u_discord_bot.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace No_u_discord_bot.MushroomRPG
{
	class MRPGMapGenerator
	{
		public Dictionary<int, List<MRPGMapTile>> GeneratedMap { get; private set; }
		private List<MRPGRoom> _rooms;
		private int roomMargin = 1;
		public int Rows { get; private set; }
		public int Collumns { get; private set; }

		public MRPGMapGenerator()
		{
			GeneratedMap = new Dictionary<int, List<MRPGMapTile>>();
			_rooms = new List<MRPGRoom>();
		}

		public void LoadFromFile(string FilePath)
		{

		}

		public void GenerateNewMap(int rooms, int rows, int collumns)
		{
			Random numberGenerator = new Random();
			Rows = rows;
			Collumns = collumns;

			// Prepare map
			for (int i = 0; i < Collumns; i++)
			{
				List<MRPGMapTile> rowTiles = new List<MRPGMapTile>();
				for (int j = 0; j < Rows; j++)
				{
					rowTiles.Add(new MRPGMapTile(i, j));
				}
				GeneratedMap.Add(i, rowTiles);
			}

			// Generate rooms
			for (int i = 0; i < rooms; i++)
			{
				MRPGRoom newRoom = new MRPGRoom(numberGenerator.Next(3, 7), numberGenerator.Next(3, 7));
				MRPGIntVector2 roomSizedHalved = new MRPGIntVector2((int)MathF.Floor(newRoom.RoomWidth / 2f), (int)MathF.Floor(newRoom.RoomHeight / 2f));
				MRPGIntVector2 roomLocation;
				int offsetLeft = (int)MathF.Floor((newRoom.RoomWidth) / 2f);
				int offsetTop = (int)MathF.Floor((newRoom.RoomHeight) / 2f);

				// Generate random locations until a open spot for the whole room is found
				bool locationAvailable = true;
				int placeChecks = 0;
				do
				{
					roomLocation = new MRPGIntVector2(numberGenerator.Next(0 + roomSizedHalved.X, Rows - roomSizedHalved.X), 
													  numberGenerator.Next(0 + roomSizedHalved.Y, Collumns - roomSizedHalved.Y));
					newRoom.RoomLocation = roomLocation;
					newRoom.minVector = new MRPGIntVector2(roomLocation.X - offsetLeft, roomLocation.Y - offsetTop);
					newRoom.maxVector = new MRPGIntVector2(roomLocation.X - offsetLeft + newRoom.RoomWidth - 1, roomLocation.Y - offsetTop + newRoom.RoomHeight - 1);

					foreach (MRPGRoom room in _rooms)
					{
						locationAvailable = newRoom.maxVector.X + roomMargin < room.minVector.X ||
											newRoom.minVector.X - roomMargin > room.maxVector.X ||
											newRoom.minVector.Y - roomMargin > room.maxVector.Y ||
											newRoom.maxVector.Y + roomMargin < room.minVector.Y;
						if(!locationAvailable)
						{ 
							break;
						}
					}
					placeChecks++;
				}
				while (!locationAvailable && placeChecks < 10000);

				if(placeChecks == 10000)
				{
					CustomDebugInfo.LogError("Not all rooms could be placed, random location attempts limit reached");
					break;
				}

				// Tell all the tiles that a room occupies them now
				List<MRPGMapTile> roomTiles = new List<MRPGMapTile>();
				for (int j = newRoom.minVector.X; j <= newRoom.maxVector.X; j++)
				{
					for (int jj = newRoom.minVector.Y; jj <= newRoom.maxVector.Y; jj++)
					{
						GeneratedMap[j][jj].TileFuntion = newRoom;
						roomTiles.Add(GeneratedMap[j][jj]);
					}
				}
				newRoom.SetRoomSurface(roomTiles);
				_rooms.Add(newRoom);
			}

			//Generate pathways
			foreach (MRPGRoom room in _rooms)
			{
				// Pick the next room to connect to
				List<MRPGRoom> roomsByDistance = new List<MRPGRoom>(_rooms);
				roomsByDistance = roomsByDistance.OrderBy(i => (i.RoomLocation - room.RoomLocation).Magnitude).ToList();
				MRPGRoom roomToConnectTo = roomsByDistance.First(i => !i.ConnectedTo.Contains(room) && i != room);
				GeneratePathBetweenRooms(room, roomToConnectTo);
			}

			//BUG: Generation pattern has a chance of generating multiple groups of rooms
			//TEMP FIX: catalogue these groups and connect them
			List<List<MRPGRoom>> roomsGroups = new List<List<MRPGRoom>>();
			List<MRPGRoom> unaccountedRooms = new List<MRPGRoom>(_rooms);

			while (unaccountedRooms.Count > 0)
			{
				List<MRPGRoom> roomGroup = GiveAttachedRooms(unaccountedRooms[0], new List<MRPGRoom>());
				foreach (MRPGRoom room in roomGroup)
				{
					unaccountedRooms.Remove(room);
				}
				roomsGroups.Add(roomGroup);
			}

			if(roomsGroups.Count > 1)
			{
				CustomDebugInfo.LogWarning("Multiple groups with rooms detected");
				MRPGIntVector2 middlePoint = new MRPGIntVector2(Collumns / 2, Rows / 2);
				List<MRPGRoom> roomsClosestToMiddle = new List<MRPGRoom>();
				foreach (List<MRPGRoom> roomGroup in roomsGroups)
				{
					float closestRoomDistance = float.MaxValue;
					MRPGRoom closestRoomOfGroup = null;
					foreach (MRPGRoom room in roomGroup)
					{
						MRPGIntVector2 differenceVector = room.RoomLocation - middlePoint;
						if (differenceVector.Magnitude < closestRoomDistance)
						{
							closestRoomOfGroup = room;
							closestRoomDistance = differenceVector.Magnitude;
						}
					}
					roomsClosestToMiddle.Add(closestRoomOfGroup);
				}

				if(roomsGroups.Count == 2)
				{
					GeneratePathBetweenRooms(roomsClosestToMiddle[0], roomsClosestToMiddle[1]);
				}
				else
				{
					for (int i = 0; i < roomsClosestToMiddle.Count; i++)
					{
						int indexNextRoom = i == roomsClosestToMiddle.Count - 1 ? 0 : i + 1;
						GeneratePathBetweenRooms(roomsClosestToMiddle[i], roomsClosestToMiddle[indexNextRoom]);
					}
				}
			}
		}

		private void GeneratePathBetweenRooms(MRPGRoom start, MRPGRoom end)
		{
			// find the shortest route to the connect points of the rooms
			MRPGPathFinder pathFinder = new MRPGPathFinder(GeneratedMap);
			float smallestDistance = float.MaxValue;
			MRPGIntVector2 startPathAt = start.TopEntrance.position;
			MRPGIntVector2 endPathAt = end.TopEntrance.position;

			List<MRPGIntVector2> startingLocations = new List<MRPGIntVector2>();
			startingLocations.Add(start.TopEntrance.position);
			startingLocations.Add(start.BottomEntrance.position);
			startingLocations.Add(start.LeftEntrance.position);
			startingLocations.Add(start.RightEntrance.position);

			List<MRPGIntVector2> endingLocations = new List<MRPGIntVector2>();
			endingLocations.Add(end.TopEntrance.position);
			endingLocations.Add(end.BottomEntrance.position);
			endingLocations.Add(end.LeftEntrance.position);
			endingLocations.Add(end.RightEntrance.position);

			foreach (MRPGIntVector2 startPos in startingLocations)
			{
				foreach (MRPGIntVector2 endPos in endingLocations)
				{
					MRPGIntVector2 differenceVector = endPos - startPos;
					if (differenceVector.Magnitude < smallestDistance)
					{
						smallestDistance = differenceVector.Magnitude;
						startPathAt = startPos;
						endPathAt = endPos;
					}
				}
			}

			// Find a path between the points and paint them in
			List<MRPGMapTile> pathTiles = pathFinder.FindPath(startPathAt, endPathAt, false, true);
			MRPGPath newPath = new MRPGPath(start, end, pathTiles);
			foreach (MRPGMapTile mapTile in pathTiles)
			{
				if (mapTile.TileFuntion == null)
				{
					mapTile.TileFuntion = newPath;
				}
			}
			start.ConnectedTo.Add(end);
			end.ConnectedTo.Add(start);
		}

		private List<MRPGRoom> GiveAttachedRooms(MRPGRoom room, List<MRPGRoom> knownRooms)
		{
			knownRooms.Add(room);
			foreach(MRPGRoom connectedRoom in room.ConnectedTo)
			{
				if(!knownRooms.Contains(connectedRoom))
				{
					GiveAttachedRooms(connectedRoom, knownRooms);
				}
			}
			return knownRooms;
		}
	}
}
