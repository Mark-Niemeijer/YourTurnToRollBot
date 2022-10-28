using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace No_u_discord_bot.MushroomRPG
{
	class MRPGCharacter : MRPGToken
	{
		public int SightRadius { get; protected set; }
		public int MaxMovement { get; protected set; }
		public int CurrentMovement { get; protected set; }
		public int MaxHealth { get; protected set; }
		public int CurrentHealth { get; protected set; }
		public int ArmourClass { get; protected set; }
		public int HitBonus { get; protected set; }
		public int DamageBonus { get; protected set; }
		public int AttackRange { get; protected set; }
		public bool Alive { get; protected set; }

		public MRPGCharacter(Bitmap icon) : base(icon)
		{

		}

		public void MoveCharacter(MRPGIntVector2 location, int movementCost)
		{
			CurrentMovement -= movementCost;
			SetLocation(location);
		}

		public void ResetOnTurn()
		{
			CurrentMovement = MaxMovement;
		}

		public bool AttackEnemy(MRPGCharacter enemy, out int hitroll, out int damage)
		{
			Random dice = new Random();
			hitroll = dice.Next(0, 20) + 1 + HitBonus;
			damage = 0;
			if (hitroll >= enemy.ArmourClass)
			{
				damage = dice.Next(0, 6) + 1 + DamageBonus;
				enemy.RecieveDamage(damage);
				return true;
			}
			else
			{
				return false;
			}
		}

		public void RecieveDamage(int damage)
		{
			CurrentHealth -= damage;
			if(CurrentHealth <= 0)
			{
				Alive = false;
			}
		}
	}
}
