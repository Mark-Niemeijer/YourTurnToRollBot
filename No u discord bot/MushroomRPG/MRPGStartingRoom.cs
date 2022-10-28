using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace No_u_discord_bot.MushroomRPG
{
	class MRPGStartingRoom : MRPGRoom
	{
		public List<MRPGMapTile> PlayerSpawnLocations { get; private set; }

		public MRPGStartingRoom(int height, int width) : base(height, width)
		{
			PlayerSpawnLocations = new List<MRPGMapTile>();
		}

		public override void SetRoomSurface(List<MRPGMapTile> surfaceTiles)
		{
			base.SetRoomSurface(surfaceTiles);
			setPlayerSpawnLocations(6);
		}

		protected override void SetEnemySpawnLocations()
		{
			EnemySpawnLocations = null;
		}

		private void setPlayerSpawnLocations(int amountSpawnLocations)
		{
			Random numberGenerator = new Random();
			for (int i = 0; i < amountSpawnLocations; i++)
			{
				MRPGMapTile selectedTile;
				do
				{
					selectedTile = RoomTiles[numberGenerator.Next(0, RoomTiles.Count)];
				}
				while (PlayerSpawnLocations.Contains(selectedTile));

				PlayerSpawnLocations.Add(selectedTile);
			}
		}
	}
}
