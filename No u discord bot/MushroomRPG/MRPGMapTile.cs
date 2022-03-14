using System;
using System.Collections.Generic;
using System.Text;

namespace No_u_discord_bot.MushroomRPG
{
	class MRPGMapTile
	{
		public MRPGIntVector2 position { get; private set; }
		public MRPGTileElement TileFuntion;
		public MRPGMapTile(int x, int y)
		{
			position = new MRPGIntVector2(x, y);
		}
	}
}
