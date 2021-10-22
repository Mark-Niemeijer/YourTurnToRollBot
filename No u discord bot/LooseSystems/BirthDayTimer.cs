using DSharpPlus;
using DSharpPlus.Entities;
using No_u_discord_bot.DataObjects;
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
					Dictionary<DiscordChannel, IReadOnlyCollection<DiscordUser>> registerdChannelsWithMembers = new Dictionary<DiscordChannel, IReadOnlyCollection<DiscordUser>>();
					foreach (ulong registeredChannelID in birthdayData.RegisteredChannels)
					{
						DiscordChannel channel = await discordClient.GetChannelAsync(registeredChannelID);
						IReadOnlyCollection<DiscordUser> membersOfChannel = await channel.Guild.GetAllMembersAsync();
						foreach (DiscordUser birthDayUser in birthDayUsers)
						{
							if(membersOfChannel.Contains(birthDayUser))
							{
								await channel.SendMessageAsync("Happy birthday " + birthDayUser.Mention + " :heart: :partying_face:").ConfigureAwait(false);
								birthdayData.UserBirthDays[birthDayUser.Id] = birthdayData.UserBirthDays[birthDayUser.Id].AddYears(1);
								JsonParser.GetInstance().SaveData(JsonParser.FileEnum.BirthdayFile, birthdayData);
							}
						}
						registerdChannelsWithMembers.Add(channel, membersOfChannel);
					}
				}
				Thread.Sleep(TimeSpan.FromHours(1));
			}
		}
	}
}
