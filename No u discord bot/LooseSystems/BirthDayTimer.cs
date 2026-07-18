using DSharpPlus;
using DSharpPlus.Entities;
using No_u_discord_bot.DataObjects;
using No_u_discord_bot.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace No_u_discord_bot.LooseSystems
{
	class BirthDayTimer
	{
		private static BirthdayDataJson birthdayData;

		public static void StartDateChecker(DiscordClient botClient)
		{
			birthdayData = JsonParser.GetInstance().LoadData<BirthdayDataJson>(JsonParser.FileEnum.BirthdayFile);
			Thread DateChecker = new Thread(CheckDate);
			DateChecker.Start(botClient);
		}

		private static async void CheckDate(object botClient)
		{
			DiscordClient discordClient = (DiscordClient)botClient;
			while (true)
			{
				List<DiscordUser> birthDayUsers = new List<DiscordUser>();
				foreach (var userBirthdays in birthdayData.UserBirthDays)
				{
					if(userBirthdays.Value.Month == DateTime.Now.Month && userBirthdays.Value.Day == DateTime.Now.Day && userBirthdays.Value.Year == DateTime.Now.Year)
					{
						birthDayUsers.Add(await discordClient.GetUserAsync(userBirthdays.Key));
					}
				}

				if (birthDayUsers.Count > 0)
				{
					foreach (ulong registeredChannelID in birthdayData.RegisteredChannels)
					{
						DiscordChannel channel = await discordClient.GetChannelAsync(registeredChannelID);
						IReadOnlyCollection<DiscordUser> membersOfChannel = await channel.Guild.GetAllMembersAsync();
						foreach (DiscordUser birthDayUser in birthDayUsers)
						{
							if(membersOfChannel.Contains(birthDayUser))
							{
								CustomDebugInfo.LogInfo("User '" + birthDayUser.Mention + "' birthday celebrated in " + channel.Mention);
								await channel.SendMessageAsync("Happy birthday " + birthDayUser.Mention + " :heart: :partying_face:").ConfigureAwait(false);
							}
						}
                    }

					foreach (DiscordUser discordUser in birthDayUsers)
                    {
                        birthdayData.UserBirthDays[discordUser.Id] = birthdayData.UserBirthDays[discordUser.Id].AddYears(1);
                    }
                    JsonParser.GetInstance().SaveData(birthdayData);
                }
				Thread.Sleep(TimeSpan.FromMinutes(30));
			}
		}
	}
}
