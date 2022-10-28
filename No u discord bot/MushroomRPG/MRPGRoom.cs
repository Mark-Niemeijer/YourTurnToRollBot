using System;
using System.Linq;
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
		public MRPGIntVector2 RoomLocation;
		public List<MRPGRoom> ConnectedTo;
		public MRPGMapTile TopEntrance { get; protected set; }
		public MRPGMapTile BottomEntrance { get; protected set; }
		public MRPGMapTile RightEntrance { get; protected set; }
		public MRPGMapTile LeftEntrance { get; protected set; }
		public List<MRPGMapTile> RoomTiles { get; protected set; }
		public List<MRPGMapTile> EnemySpawnLocations { get; protected set; }

		public MRPGRoom(int height, int width)
		{
			RoomHeight = height;
			RoomWidth = width;
			ConnectedTo = new List<MRPGRoom>();
			EnemySpawnLocations = new List<MRPGMapTile>();
		}

		public virtual void SetRoomSurface(List<MRPGMapTile> surfaceTiles)
		{
			RoomTiles = surfaceTiles;
			SetRoomEntrances();
			SetEnemySpawnLocations();
		}

		protected virtual void SetRoomEntrances()
		{
			MRPGIntVector2 centerLeft = new MRPGIntVector2(minVector.X, (int)Math.Floor((minVector.Y + maxVector.Y) / 2f));
			MRPGIntVector2 centerRight = new MRPGIntVector2(maxVector.X, (int)Math.Floor((minVector.Y + maxVector.Y) / 2f));
			MRPGIntVector2 centerTop = new MRPGIntVector2((int)Math.Floor((minVector.X + maxVector.X) / 2f), maxVector.Y);
			MRPGIntVector2 centerBottom = new MRPGIntVector2((int)Math.Floor((minVector.X + maxVector.X) / 2f), minVector.Y);
			TopEntrance = RoomTiles.FirstOrDefault(i => centerTop.X == i.position.X && centerTop.Y == i.position.Y);
			BottomEntrance = RoomTiles.FirstOrDefault(i => centerBottom.X == i.position.X && centerBottom.Y == i.position.Y);
			RightEntrance = RoomTiles.FirstOrDefault(i => centerRight.X == i.position.X && centerRight.Y == i.position.Y);
			LeftEntrance = RoomTiles.FirstOrDefault(i => centerLeft.X == i.position.X && centerLeft.Y == i.position.Y);
		}

		protected virtual void SetEnemySpawnLocations()
		{
			if (RoomTiles == null) return;
			EnemySpawnLocations = new List<MRPGMapTile>(RoomTiles);
		}
	}
}
