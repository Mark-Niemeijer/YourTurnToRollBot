using System;
using System.Collections.Generic;
using System.Text;

namespace No_u_discord_bot.MushroomRPG
{
	class MRPGRoom : MRPGTileElement
	{
		public int RoomHeight { get; private set; }
		public int RoomWidth { get; private set; }
		public MRPGIntVector2 minVector;
		public MRPGIntVector2 maxVector;

		public MRPGRoom(int height, int width)
		{
			RoomHeight = height;
			RoomWidth = width;
		}
	}
}
