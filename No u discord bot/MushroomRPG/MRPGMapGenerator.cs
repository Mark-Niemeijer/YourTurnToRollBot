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
			GenerateMap(5);
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
				MRPGRoom room = new MRPGRoom(numberGenerator.Next(3, 7), numberGenerator.Next(3, 7));
				MRPGIntVector2 roomSizedHalved = new MRPGIntVector2((int)MathF.Floor(room.RoomWidth / 2f), (int)MathF.Floor(room.RoomHeight / 2f));
				MRPGIntVector2 roomLocation;

				bool locationAvailable = false;
				do
				{
					//Fix room availability detection - box box collisions
					roomLocation = new MRPGIntVector2(numberGenerator.Next(0 + roomSizedHalved.X, Rows - roomSizedHalved.X), 
													  numberGenerator.Next(0 + roomSizedHalved.Y, Collumns - roomSizedHalved.Y));

					if(GeneratedMap[roomLocation.Y][roomLocation.X].TileFuntion == null)
					{
						locationAvailable = true;
					}
				}
				while (!locationAvailable);

				int offsetLeft = (int)MathF.Floor((room.RoomWidth - 1) / 2f);
				int offsetTop = (int)MathF.Ceiling((room.RoomHeight - 1) / 2f);

				for (int j = 0; j < room.RoomWidth; j++)
				{
					for (int jj = 0; jj < room.RoomHeight; jj++)
					{
						GeneratedMap[roomLocation.X + j - offsetLeft][roomLocation.X + jj - offsetTop].TileFuntion = room;
					}
				}
				_rooms.Add(room);
			}
		}
	}
}
