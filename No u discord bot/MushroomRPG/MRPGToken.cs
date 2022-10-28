using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace No_u_discord_bot.MushroomRPG
{
	class MRPGToken
	{
		public MRPGIntVector2 GridLocation { get; protected set; }
		public Bitmap Icon { get; protected set; }
		public MRPGToken(Bitmap icon)
		{
			Icon = icon;
		}

		public virtual void SetLocation(MRPGIntVector2 location)
		{
			GridLocation = location;
		}
	}
}
