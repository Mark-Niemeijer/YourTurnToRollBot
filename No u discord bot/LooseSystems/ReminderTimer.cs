using DSharpPlus;
using DSharpPlus.Entities;
using No_u_discord_bot.DataObjects;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace No_u_discord_bot.LooseSystems
{
	class ReminderTimer
	{
		public static void StartDateChecker(DiscordClient botClient)
		{
			Thread DateChecker = new Thread(CheckDate);
			DateChecker.Start(botClient);
		}

		private static async void CheckDate(object botClient)
		{
			DiscordClient discordClient = (DiscordClient)botClient;
			while (true)
			{
				ReminderListJson reminderFile = JsonParser.GetInstance().LoadData<ReminderListJson>(JsonParser.FileEnum.ReminderFile);
				if (reminderFile == null) return;

				for (int i = 0; i < reminderFile.UserID.Count; i++)
				{
					ulong userId = reminderFile.UserID[i];
					ulong remindInChannel = reminderFile.RemindInChannel[i];
					DateTime dateReminder = reminderFile.DateReminder[i];
					string message = reminderFile.ReminderMessage[i];

					DateTime currentTime = DateTime.Now;
					currentTime = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, currentTime.Hour, currentTime.Minute, 0);

					if (currentTime == dateReminder || dateReminder < currentTime)
					{
						DiscordChannel channel = await discordClient.GetChannelAsync(remindInChannel);
						DiscordUser discordUser = await discordClient.GetUserAsync(userId);

						reminderFile.UserID.RemoveAt(i);
						reminderFile.RemindInChannel.RemoveAt(i);
						reminderFile.DateReminder.RemoveAt(i);
						reminderFile.ReminderMessage.RemoveAt(i);
						JsonParser.GetInstance().SaveData(reminderFile);
						i--;

						if(currentTime == dateReminder)
						{
							await channel.SendMessageAsync("Remember " + discordUser.Mention + ": " + message);
						}
						else
						{
							await channel.SendMessageAsync("I was asleep and missed the time :worried:, but dont forget " + discordUser.Mention + ": " + message);
						}
					}
				}

				Thread.Sleep(TimeSpan.FromMinutes(1));
			}
		}
	}
}
