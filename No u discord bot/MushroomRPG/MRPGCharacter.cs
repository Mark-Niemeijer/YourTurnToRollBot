using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace No_u_discord_bot.MushroomRPG
{
	class MRPGCharacter : MRPGToken
	{
		public int SightRadius;
		public int MaxMovement;
		public int CurrentMovement;
		public MRPGCharacter(Bitmap icon) : base(icon)
		{

		}
	}
}
