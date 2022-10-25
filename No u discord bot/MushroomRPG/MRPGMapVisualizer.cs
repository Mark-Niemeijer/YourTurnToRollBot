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
		private readonly List<Bitmap> _horizontalCoordinats = new List<Bitmap>();
		private readonly List<Bitmap> _verticalCoordinats = new List<Bitmap>();

		public MRPGMapVisualizer()
		{
			for (int i = 1; i < 9; i++)
			{
				char letter = (char)(64 + i);
				_verticalCoordinats.Add(new Bitmap(Environment.CurrentDirectory + "\\DataObjects\\RPGTiles\\Tile" + i + ".png"));
				_horizontalCoordinats.Add(new Bitmap(Environment.CurrentDirectory + "\\DataObjects\\RPGTiles\\Tile" + letter.ToString() + ".png"));
			}
		}

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
			//SightRadius times 2 for diameter + 1 for tile they are standing on 
			//Another +1 for coordinateImages
			int tilesInView = character.SightRadius * 2 + 2;

			Bitmap playerVision = new Bitmap(horizontalImageSize * (tilesInView), verticalImageSize * (tilesInView));
			using (Graphics playerVisionGraphics = Graphics.FromImage(playerVision))
			{
				for (int i = 1; i < tilesInView; i++)
				{
					playerVisionGraphics.DrawImage(_horizontalCoordinats[i - 1], new Rectangle(horizontalImageSize * i, 0, horizontalImageSize, verticalImageSize));
					playerVisionGraphics.DrawImage(_verticalCoordinats[i - 1], new Rectangle(0, verticalImageSize * i, horizontalImageSize, verticalImageSize));
				}

				int characterImageLocationX = character.GridLocation.X * horizontalImageSize;
				int characterImageLocationY = character.GridLocation.Y * verticalImageSize;
				int offset = character.SightRadius * horizontalImageSize;
				playerVisionGraphics.DrawImage(fullMap, 
					new Rectangle(horizontalImageSize, verticalImageSize, playerVision.Width - horizontalImageSize, playerVision.Height - verticalImageSize),
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
