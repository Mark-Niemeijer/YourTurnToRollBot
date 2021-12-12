using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace No_u_discord_bot.LooseSystems
{
	class RPGMapGenerator
	{
		private readonly int verticalTiles = 20;
		private readonly int horizontalTiles = 20;
		private readonly int verticalImageSize = 50;
		private readonly int horizontalImageSize = 50;
		private List<Bitmap> tileSet;
		private int[,] mapTemplate;

		public RPGMapGenerator()
		{
			tileSet = new List<Bitmap>();
			mapTemplate = new int[verticalTiles, horizontalTiles];
			LoadMapTiles();
			SetupMapTemplate();
		}

		public Bitmap GenerateMap()
		{
			Bitmap newMap = new Bitmap(horizontalTiles * horizontalImageSize, verticalTiles * verticalImageSize);

			for (int i = 0; i < horizontalTiles; i++)
			{
				for (int ii = 0; ii < verticalTiles; ii++)
				{
					for (int j = 0; j < horizontalImageSize; j++)
					{
						for (int jj = 0; jj < verticalImageSize; jj++)
						{
							int offsetX = i * horizontalImageSize;
							int offsety = ii * verticalImageSize;
							int templateIndex = mapTemplate[i, ii];
							Bitmap tile = tileSet[templateIndex];
							newMap.SetPixel(offsetX + j, offsety + jj, tile.GetPixel(j, jj));
						}
					}
				}
			}

			return newMap;
		}

		private void LoadMapTiles()
		{
			tileSet.Add(new Bitmap(Environment.CurrentDirectory + "\\DataObjects\\RPGTiles\\Tile1.png"));
			tileSet.Add(new Bitmap(Environment.CurrentDirectory + "\\DataObjects\\RPGTiles\\Tile2.png"));
			tileSet.Add(new Bitmap(Environment.CurrentDirectory + "\\DataObjects\\RPGTiles\\Tile3.png"));
		}

		private void SetupMapTemplate()
		{
			Random random = new Random();
			for (int i = 0; i < verticalTiles; i++)
			{
				for (int j = 0; j < horizontalTiles; j++)
				{
					mapTemplate[i, j] = random.Next(0, 3);
				}
			}
		}
	}
}