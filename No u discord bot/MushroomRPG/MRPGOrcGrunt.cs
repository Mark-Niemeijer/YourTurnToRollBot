using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace No_u_discord_bot.MushroomRPG
{
	class MRPGOrcGrunt : MRPGCharacter
	{
		public MRPGOrcGrunt(Bitmap icon) : base(icon)
		{
			SightRadius = 3;
			MaxMovement = 30;
			CurrentMovement = MaxMovement;
		}
	}
}