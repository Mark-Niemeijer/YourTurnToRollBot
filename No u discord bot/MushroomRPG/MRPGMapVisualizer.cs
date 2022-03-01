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

		public MRPGMapVisualizer()
		{

		}

		public Bitmap VisualizeMap(MRPGMapGenerator mapGenerator)
		{
			Bitmap MapImage = new Bitmap(mapGenerator.Rows * horizontalImageSize, mapGenerator.Collumns * verticalImageSize);
			for (int i = 0; i < mapGenerator.Rows; i++)
			{
				for (int ii = 0; ii < mapGenerator.Collumns; ii++)
				{
					bool tileIsRoom = mapGenerator.GeneratedMap[ii][i].TileFuntion != null;
					Bitmap BitmapForTile = tileIsRoom ? RoomTile : WallTile;

					for (int j = 0; j < horizontalImageSize; j++)
					{
						for (int jj = 0; jj < verticalImageSize; jj++)
						{
							MapImage.SetPixel(j + (ii * verticalImageSize), jj + (i * horizontalImageSize), BitmapForTile.GetPixel(j, jj));
						}
					}
				}
			}

			return MapImage;
		}
	}
}
