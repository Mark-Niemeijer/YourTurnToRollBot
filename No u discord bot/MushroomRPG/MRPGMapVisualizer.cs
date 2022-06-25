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

		public Bitmap VisualizeBackGround(MRPGMapGenerator mapGenerator)
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
						else if (tile.TileFuntion is MRPGRoom)
						{
							BitmapForTile = RoomTile;
						}
						graphics.DrawImage(BitmapForTile, new Point(i * horizontalImageSize, j * verticalImageSize));
					}
				}
			}

			return MapImage;
		}

		public Bitmap VisualizePlayerView(Bitmap fullMap, MRPGCharacter character)
		{
			Bitmap playerVision = new Bitmap(horizontalImageSize * (character.SightRadius * 2 + 1), verticalImageSize * (character.SightRadius * 2 + 1));
			using (Graphics playerVisionGraphics = Graphics.FromImage(playerVision))
			{
				int characterImageLocationX = character.GridLocation.X * horizontalImageSize;
				int characterImageLocationY = character.GridLocation.Y * verticalImageSize;
				int offset = character.SightRadius * horizontalImageSize;
				playerVisionGraphics.DrawImage(fullMap, new Rectangle(0, 0, playerVision.Width, playerVision.Height),
					new Rectangle(characterImageLocationX - offset, characterImageLocationY - offset, 
					offset * 2 + horizontalImageSize, offset * 2 + verticalImageSize), GraphicsUnit.Pixel);
			}
			return playerVision;
		}

		public Bitmap PlaceTokens(Bitmap background, List<MRPGToken> tokens)
		{
			Bitmap backgroundWithTokens = new Bitmap(background);
			using (Graphics graphics = Graphics.FromImage(backgroundWithTokens))
			{
				foreach (MRPGToken token in tokens)
				{
					graphics.DrawImage(token.Icon, new Point(token.GridLocation.X * horizontalImageSize, token.GridLocation.Y * verticalImageSize));
				}
			}
			return backgroundWithTokens;
		}
	}
}
