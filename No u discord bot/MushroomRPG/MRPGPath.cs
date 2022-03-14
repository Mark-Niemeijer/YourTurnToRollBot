using System;
using System.Collections.Generic;
using System.Text;

namespace No_u_discord_bot.MushroomRPG
{
	class MRPGPath : MRPGTileElement
	{
		public MRPGRoom StartingRoom { get; private set; }
		public MRPGRoom EndingRoom { get; private set; }
		public List<MRPGMapTile> PathTiles { get; private set; }

		public MRPGPath(MRPGRoom startingRoom, MRPGRoom endingRoom, List<MRPGMapTile> pathTiles)
		{
			StartingRoom = startingRoom;
			EndingRoom = endingRoom;
			PathTiles = pathTiles;
		}
	}
}
