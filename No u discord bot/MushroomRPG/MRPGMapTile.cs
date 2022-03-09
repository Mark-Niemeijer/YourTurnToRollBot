using System;
using System.Collections.Generic;
using System.Text;

namespace No_u_discord_bot.MushroomRPG
{
	class MRPGMapTile
	{
		public int X { get; private set; }
		public int Y { get; private set; }
		public MRPGTileElement TileFuntion;
		public MRPGMapTile(int x, int y)
		{
			this.X = x;
			this.Y = y;
		}
	}
}
