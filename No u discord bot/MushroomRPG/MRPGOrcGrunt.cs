﻿using System;
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
			MaxHealth = 10;
			CurrentHealth = MaxHealth;
			ArmourClass = 11;
			HitBonus = 2;
			AttackRange = 1;
			DamageBonus = 2;
			Alive = true;
		}
	}
}