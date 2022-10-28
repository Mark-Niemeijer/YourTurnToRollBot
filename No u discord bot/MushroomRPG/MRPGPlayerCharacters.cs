using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace No_u_discord_bot.MushroomRPG
{
	class MRPGPlayerCharacters : MRPGCharacter
	{
		public MRPGPlayerCharacters(Bitmap icon) : base(icon)
		{
			SightRadius = 3;
			MaxMovement = 30;
			CurrentMovement = MaxMovement;
			MaxHealth = 30;
			CurrentHealth = MaxHealth;
			ArmourClass = 16;
			AttackRange = 1;
			HitBonus = 2;
			DamageBonus = 4;
			Alive = true;
		}
	}
}
