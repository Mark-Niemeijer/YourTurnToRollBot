using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace No_u_discord_bot.MushroomRPG
{
	class MRPGMapVisualizer
	{
		private readonly int horizontalImageSize = 50;
		private readonly int verticalImageSize = 50;
		private readonly Bitmap RoomTile = new Bitmap(Environment.CurrentDirectory + "\\DataObjects\\RPGTiles\\Tile1.png");
		private readonly Bitmap WallTile = new Bitmap(Environment.CurrentDirectory + "\\DataObjects\\RPGTiles\\Tile2.png");
		private readonly Bitmap PathTile = new Bitmap(Environment.CurrentDirectory + "\\DataObjects\\RPGTiles\\Tile3.png");

		public Bitmap VisualizeMap(MRPGMapGenerator mapGenerator)
		{
			Bitmap MapImage = new Bitmap(mapGenerator.Rows * horizontalImageSize, mapGenerator.Collumns * verticalImageSize);
			//for (int i = 0; i < mapGenerator.Rows; i++)
			//{
			//	for (int ii = 0; ii < mapGenerator.Collumns; ii++)
			//	{
			//		bool tileIsRoom = mapGenerator.GeneratedMap[ii][i].TileFuntion != null;
			//		Bitmap BitmapForTile = tileIsRoom ? RoomTile : WallTile;

			//		for (int j = 0; j < horizontalImageSize; j++)
			//		{
			//			for (int jj = 0; jj < verticalImageSize; jj++)
			//			{
			//				MapImage.SetPixel(j + (ii * verticalImageSize), jj + (i * horizontalImageSize), BitmapForTile.GetPixel(j, jj));
			//			}
			//		}
			//	}
			//}
			for (int i = 0; i < MapImage.Height; i++)
			{
				for (int j = 0; j < MapImage.Width; j++)
				{
					int mapCollumn = (int)Math.Floor(j / (float)horizontalImageSize);
					int mapRow = (int)Math.Floor(i / (float)verticalImageSize);
					MRPGMapTile tile = mapGenerator.GeneratedMap[mapCollumn][mapRow];
					Bitmap BitmapForTile = null;

					if (tile.TileFuntion == null)
					{
						BitmapForTile = WallTile;
					}
					else if(tile.TileFuntion.GetType() == typeof(MRPGPath))
					{
						BitmapForTile = PathTile;
					}
					else if(tile.TileFuntion.GetType() == typeof(MRPGRoom))
					{
						BitmapForTile = RoomTile;
					}

					int tilePixelX = j % horizontalImageSize;
					int tilePixelY = i % verticalImageSize;

					MapImage.SetPixel(j, i, BitmapForTile.GetPixel(tilePixelX, tilePixelY));
				}
			}

			return MapImage;
		}
	}
}
