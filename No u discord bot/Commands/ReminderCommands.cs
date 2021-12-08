using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using No_u_discord_bot.Helpers;
using No_u_discord_bot.DataObjects;
using System.Collections.Generic;
using System;
using System.Threading.Tasks;

namespace No_u_discord_bot.Commands
{
	class ReminderCommands : BaseCommandModule
	{
		[Command("Reminder"), Description("Syntax: $[Command] [Number of units to wait] [The unit type] [What the bot should say then]\nThe bot will remind you of whatever you said")]
		public async Task RemindPerson(CommandContext commandContext, int reminderNumber, string reminderUnit, [RemainingText] string Message)
		{			
			if (reminderNumber < 1)
			{
				await commandContext.Channel.SendMessageAsync("I am not gonna remind you of something that has already happend :unamused:").ConfigureAwait(false);
				return;
			}

			DateTime timeToBeReminded = DateTime.Now;
			timeToBeReminded = new DateTime(timeToBeReminded.Year, timeToBeReminded.Month, timeToBeReminded.Day, timeToBeReminded.Hour, timeToBeReminded.Minute, 0);

			switch (reminderUnit)
			{
				case "min":
					timeToBeReminded = timeToBeReminded.AddMinutes(reminderNumber);
					break;
				case "hour":
					timeToBeReminded = timeToBeReminded.AddHours(reminderNumber);
					break;
				case "day":
					timeToBeReminded = timeToBeReminded.AddDays(reminderNumber);
					break;
				default:
					await commandContext.Channel.SendMessageAsync("I am not familiar with that unit. Try \"min\", \"hour\", or \"day\"").ConfigureAwait(false);
					return;
			}

			ReminderListJson currentRemindersList = JsonParser.GetInstance().LoadData<ReminderListJson>(JsonParser.FileEnum.ReminderFile);
			currentRemindersList.UserID.Add(commandContext.User.Id);
			currentRemindersList.DateReminder.Add(timeToBeReminded);
			currentRemindersList.ReminderMessage.Add(Message);
			currentRemindersList.RemindInChannel.Add(commandContext.Channel.Id);

			JsonParser.GetInstance().SaveData(currentRemindersList);

			await commandContext.Channel.SendMessageAsync("Alright, I'll do my best to remind you :smile:").ConfigureAwait(false);
		}
	}
}