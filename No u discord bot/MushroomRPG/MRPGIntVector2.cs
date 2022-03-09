using System;
using System.Collections.Generic;
using System.Text;

namespace No_u_discord_bot.MushroomRPG
{
	class MRPGIntVector2
	{
		public int X;
		public int Y;
		public float Magnitude { get { return MathF.Pow(X, 2) + MathF.Pow(Y, 2); } }
		public float MagnitudeSquared { get { return MathF.Sqrt(this.Magnitude); } }
		public MRPGIntVector2(int X, int Y)
		{
			this.X = X;
			this.Y = Y;
		}

		public static MRPGIntVector2 operator +(MRPGIntVector2 vec1, MRPGIntVector2 vec2)
		{
			return new MRPGIntVector2(vec1.X + vec2.X, vec1.Y + vec2.Y);
		}

		public static MRPGIntVector2 operator -(MRPGIntVector2 vec1, MRPGIntVector2 vec2)
		{
			return new MRPGIntVector2(vec1.X - vec2.X, vec1.Y - vec2.Y);
		}


	}
}
