using System;
using System.Collections.Generic;
using System.Text;

namespace No_u_discord_bot.Helpers
{
	class DiceChecks
	{
		private static Random dice;
		public static bool RollForSuccess(int chance)
		{
			if (dice == null)
			{
				dice = new Random();
			}

			int diceResult = dice.Next(0, 100);
			return diceResult < chance;
		}
	}
}
