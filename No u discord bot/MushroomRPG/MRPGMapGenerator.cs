using System;
using System.Collections.Generic;
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
			GenerateMap(30);
		}

		private void GenerateMap(int rooms)
		{
			Random numberGenerator = new Random();

			for (int i = 0; i < Collumns; i++)
			{
				List<MRPGMapTile> rowTiles = new List<MRPGMapTile>();
				for (int j = 0; j < Rows; j++)
				{
					rowTiles.Add(new MRPGMapTile());
				}
				GeneratedMap.Add(i, rowTiles);
			}

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
					newRoom.minVector = new MRPGIntVector2(roomLocation.X - offsetLeft, roomLocation.Y - offsetTop);
					newRoom.maxVector = new MRPGIntVector2(roomLocation.X - offsetLeft + newRoom.RoomWidth, roomLocation.Y - offsetTop + newRoom.RoomHeight);

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

				for (int j = newRoom.minVector.X; j < newRoom.maxVector.X; j++)
				{
					for (int jj = newRoom.minVector.Y; jj < newRoom.maxVector.Y; jj++)
					{
						GeneratedMap[j][jj].TileFuntion = newRoom;
					}
				}
				_rooms.Add(newRoom);
			}
		}
	}
}
