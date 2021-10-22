using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using No_u_discord_bot.DataObjects;
using No_u_discord_bot.Helpers;
using System;
using System.Threading.Tasks;

namespace No_u_discord_bot.Commands
{
	class BirthdayCelebratorCommands : BaseCommandModule
	{
		[Command("bdAdd"), Description("Syntax: $[Command] [Birthday] [Birthmonth]\nAdds your birthday to the database"), CommandCustomGroupAttribute("Birthday celebrator")]
		public async Task LinkUserWithDate(CommandContext commandContext, int day, int month)
		{
			BirthdayDataJson birthdayDataJson = JsonParser.GetInstance().LoadData<BirthdayDataJson>(JsonParser.FileEnum.BirthdayFile);
			if (birthdayDataJson.UserBirthDays.ContainsKey(commandContext.User.Id))
			{
				await commandContext.Channel.SendMessageAsync("You already have yours registered, but I will overwrite it for you").ConfigureAwait(false);
				birthdayDataJson.UserBirthDays.Remove(commandContext.User.Id);
			}

			if (!GivenDateValid(month, day))
			{
				await commandContext.Channel.SendMessageAsync("That date ain't gonna do anything, are you lying to me?").ConfigureAwait(false);
				return;
			}

			DateTime birthday = new DateTime(DateTime.Now.Year, month, day);
			if (DateTime.Now > birthday) birthday.AddYears(1);
			birthdayDataJson.UserBirthDays.Add(commandContext.User.Id, birthday);
			JsonParser.GetInstance().SaveData(JsonParser.FileEnum.BirthdayFile, birthdayDataJson);
			await commandContext.Channel.SendMessageAsync("I have written your birthdate down :smile:").ConfigureAwait(false);
		}

		[Command("bdLink"), Description("Syntax: $[Command]\nSet this channel to send all happy birhtday wishes"), CommandCustomGroupAttribute("Birthday celebrator")]
		public async Task LinkChannelForWishes(CommandContext commandContext)
		{
			BirthdayDataJson birthdayDataJson = JsonParser.GetInstance().LoadData<BirthdayDataJson>(JsonParser.FileEnum.BirthdayFile);
			if (birthdayDataJson.RegisteredChannels.Contains(commandContext.Channel.Id))
			{
				await commandContext.Channel.SendMessageAsync("This channel is already used for birthday wishes").ConfigureAwait(false);
				return;
			}

			birthdayDataJson.RegisteredChannels.Add(commandContext.Channel.Id);
			JsonParser.GetInstance().SaveData(JsonParser.FileEnum.BirthdayFile, birthdayDataJson);
			await commandContext.Channel.SendMessageAsync("I will use this channel to send birthday wishes :smile:").ConfigureAwait(false);
		}

		[Command("bdUnLink"), Description("Syntax: $[Command]\nUnset this channel to send all happy birhtday wishes"), CommandCustomGroupAttribute("Birthday celebrator")]
		public async Task UnLinkChannelForWishes(CommandContext commandContext)
		{
			BirthdayDataJson birthdayDataJson = JsonParser.GetInstance().LoadData<BirthdayDataJson>(JsonParser.FileEnum.BirthdayFile);
			if (!birthdayDataJson.RegisteredChannels.Contains(commandContext.Channel.Id))
			{
				await commandContext.Channel.SendMessageAsync("This channel wasn't used for birthday wishes").ConfigureAwait(false);
				return;
			}

			birthdayDataJson.RegisteredChannels.Remove(commandContext.Channel.Id);
			JsonParser.GetInstance().SaveData(JsonParser.FileEnum.BirthdayFile, birthdayDataJson);
			await commandContext.Channel.SendMessageAsync("I will stop using this channel to send birthday wishes").ConfigureAwait(false);
		}

		private bool GivenDateValid(int month, int day)
		{
			if (month > 12) return false;
			if (day > 31) return false;
			if ((month == 4 || month == 6 || month == 7 || month == 9 || month == 11) && day > 30) return false;
			if (month == 2 && day > 29) return false;
			return true;
		}
	}
}