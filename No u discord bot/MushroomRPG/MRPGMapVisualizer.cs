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
		private readonly Bitmap RoomTile = new Bitmap(Environment.CurrentDirectory + "\\DataObjects\\RPGTiles\\RoomTile.png");
		private readonly Bitmap WallTile = new Bitmap(Environment.CurrentDirectory + "\\DataObjects\\RPGTiles\\WallTile.png");
		private readonly Bitmap PathTile = new Bitmap(Environment.CurrentDirectory + "\\DataObjects\\RPGTiles\\PathTile.png");

		public Bitmap VisualizeMap(MRPGMapGenerator mapGenerator)
		{
			Bitmap MapImage = new Bitmap(mapGenerator.Rows * horizontalImageSize, mapGenerator.Collumns * verticalImageSize);
			using (Graphics graphics = Graphics.FromImage(MapImage))
			{
				for (int i = 0; i < mapGenerator.Collumns; i++)
				{
					for (int j = 0; j < mapGenerator.Rows; j++)
					{
						MRPGMapTile tile = mapGenerator.GeneratedMap[i][j];
						Bitmap BitmapForTile = null;
						if (tile.TileFuntion == null)
						{
							BitmapForTile = WallTile;
						}
						else if (tile.TileFuntion.GetType() == typeof(MRPGPath))
						{
							BitmapForTile = PathTile;
						}
						else if (tile.TileFuntion.GetType() == typeof(MRPGRoom))
						{
							BitmapForTile = RoomTile;
						}
						graphics.DrawImage(BitmapForTile, new Point(i * horizontalImageSize, j * verticalImageSize));
					}
				}
			}

			return MapImage;
		}
	}
}
