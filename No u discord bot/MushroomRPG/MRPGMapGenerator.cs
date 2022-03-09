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
		public int Rows { get; private set; }
		public int Collumns { get; private set; }

		public MRPGMapGenerator(int rows, int collumns)
		{
			GeneratedMap = new Dictionary<int, List<MRPGMapTile>>();
			_rooms = new List<MRPGRoom>();
			Rows = rows;
			Collumns = collumns;
			GenerateMap(10);
		}

		private void GenerateMap(int rooms)
		{
			Random numberGenerator = new Random();

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

				bool locationAvailable = true;
				do
				{
					roomLocation = new MRPGIntVector2(numberGenerator.Next(0 + roomSizedHalved.X, Rows - roomSizedHalved.X), 
													  numberGenerator.Next(0 + roomSizedHalved.Y, Collumns - roomSizedHalved.Y));
					newRoom.RoomLocation = roomLocation;
					newRoom.minVector = new MRPGIntVector2(roomLocation.X - offsetLeft, roomLocation.Y - offsetTop);
					newRoom.maxVector = new MRPGIntVector2(roomLocation.X - offsetLeft + newRoom.RoomWidth - 1, roomLocation.Y - offsetTop + newRoom.RoomHeight - 1);

					foreach (MRPGRoom room in _rooms)
					{
						locationAvailable = newRoom.maxVector.X < room.minVector.X ||
											newRoom.minVector.X > room.maxVector.X ||
											newRoom.minVector.Y > room.maxVector.Y ||
											newRoom.maxVector.Y < room.minVector.Y;
						if(!locationAvailable)
						{ 
							break;
						}
					}
				}
				while (!locationAvailable);

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
				List<MRPGRoom> roomsByDistance = new List<MRPGRoom>(_rooms);
				roomsByDistance = roomsByDistance.OrderBy(i => (i.RoomLocation - room.RoomLocation).Magnitude).ToList();
				MRPGRoom roomToConnectTo = roomsByDistance.First(i => !i.ConnectedTo.Contains(room));

				MRPGIntVector2 locationDifference = room.RoomLocation - roomToConnectTo.RoomLocation;
				if(locationDifference.Y > 0)
				{
					//this room above target
					if(locationDifference.X > 0)
					{
						//room is right of target
						if(numberGenerator.Next(0,2) == 0)
						{
							//Build pathway from left side
						}
						else
						{
							//Build pathway from bottom side
						}
					}
					else
					{
						//room is left of target
						if (numberGenerator.Next(0, 2) == 0)
						{
							//Build pathway from right side
						}
						else
						{
							//Build pathway from bottom side
						}

					}
				}
				else
				{
					//this room below target
					if (locationDifference.X > 0)
					{
						//room is right of target
						if (numberGenerator.Next(0, 2) == 0)
						{
							//Build pathway from left side
						}
						else
						{
							//Build pathway from top side
						}

					}
					else
					{
						//room is left of target
						if (numberGenerator.Next(0, 2) == 0)
						{
							//Build pathway from right side
						}
						else
						{
							//Build pathway from top side
						}

					}
				}
			}
		}
	}
}
