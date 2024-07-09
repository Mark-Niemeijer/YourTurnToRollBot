using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using No_u_discord_bot.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace No_u_discord_bot.Commands
{
	class DiceRollCommands : BaseCommandModule
	{
		private const int _messageCharacterLimit = 2000;
		private const int _amountOfDiceLimit = 255;
		private const int _heightOfDiceLimit = 100;

		[Command("Roll"), Aliases("R"), Description("Syntax: $[Command] [Number of dice]d[Number of sides on dice]\nRolls a certain die a number of times")]
		public async Task RollDicesWithString(CommandContext commandContext, string inputString)
		{
  			// Split with a regex so that the delimiter ("d") is kept in the splitInput string
			string[] splitInput = Regex.Split(inputString, "(d)");

			int amountOfDice;
			int dieSides;
			bool firstValueValid;
			bool secondValueValid;


		  	if (splitInput.Length == 3)
			{
				if (String.IsNullOrEmpty(splitInput[0]))
    				{
					amountOfDice = 1;
					firstValueValid = true;
				}
    				else
				{
					firstValueValid = int.TryParse(splitInput[0], out amountOfDice);
				}
				secondValueValid = int.TryParse(splitInput[2], out dieSides);
			}
			else
			{
				await commandContext.Channel.SendMessageAsync("Yeah no, try the propper syntax").ConfigureAwait(false);
				return;
			}


			if(!firstValueValid)
			{
				await commandContext.Channel.SendMessageAsync("Yeah no, those dont look like valid amount of dice").ConfigureAwait(false);
				return;
			}
			else if(!secondValueValid)
			{
				await commandContext.Channel.SendMessageAsync("Yeah no, those dont look like valid amount of die sides").ConfigureAwait(false);
				return;
			}

			await RollDicesWithNumbers(commandContext, amountOfDice, dieSides);
		}

		[Command("Roll"), Description("Syntax: $[Command] [Number of dice] [Number of sides on dice]\nRolls a certain die a number of times")]
		public async Task RollDicesWithNumbers(CommandContext commandContext, int amountOfDice, int dieSides)
		{
			if(amountOfDice <= 0)
			{
				await commandContext.Channel.SendMessageAsync("How about you throw 1 or more dice").ConfigureAwait(false);
				return;
			}
			else if(dieSides <= 1)
			{
				await commandContext.Channel.SendMessageAsync("Use a die with more than 1 side").ConfigureAwait(false);
				return;
			}
			else if(amountOfDice > _amountOfDiceLimit)
			{
				await commandContext.Channel.SendMessageAsync("The amount of dice has been limited to " + _amountOfDiceLimit).ConfigureAwait(false);
				return;
			}
			else if(dieSides > _heightOfDiceLimit)
			{
				await commandContext.Channel.SendMessageAsync("The amount of dice sides has been limited to " + _heightOfDiceLimit).ConfigureAwait(false);
				return;
			}

			List<int> resultList = rollDice(dieSides, amountOfDice);
			int total = resultList.Sum(roll => roll);
			string resultMessage = "Your rolls: ";
			resultMessage += string.Join(" + ", resultList);
			resultMessage += "\nThe total: " + total;

			// Splits up the resulting message into multiple if it exceeds the message character limit
			if (resultMessage.Length >= _messageCharacterLimit)
			{
				List<string> stringSegments = new List<string>();
				for (int i = 0; i < resultMessage.Length; i += _messageCharacterLimit)
				{
					stringSegments.Add(resultMessage.Substring(i, Math.Min(_messageCharacterLimit, resultMessage.Length - i)));
				}

				foreach (string segment in stringSegments)
				{
					await commandContext.Channel.SendMessageAsync(segment).ConfigureAwait(false);
				}
			}
			else
			{
				await commandContext.Channel.SendMessageAsync(resultMessage).ConfigureAwait(false);
			}


			if (commandContext.Channel.IsPrivate)
			{
				CustomDebugInfo.LogInfo(commandContext.User.Username + " rolled a combined " + total + " in their DM channel");
			}
			else
			{
				CustomDebugInfo.LogInfo(commandContext.User.Username + " rolled a combined " + total + " in '" + commandContext.Channel.Name + "' within '" + commandContext.Channel.Guild.Name + "'");
			}
		}

		private List<int> rollDice(int diceSides, int amountOfRolls)
		{
			Random dice = new Random();
			List<int> resultList = new List<int>();

			for (int i = 0; i < amountOfRolls; i++)
			{
				resultList.Add(dice.Next(1, (int)diceSides + 1));
			}

			return resultList;
		}
	}
}
