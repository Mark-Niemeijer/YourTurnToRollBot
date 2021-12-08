using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace No_u_discord_bot.DataObjects
{
	class BirthdayDataJson : IDataFile
	{
		[JsonProperty("BirthdaysOfUsers")]
		public Dictionary<ulong,DateTimeOffset> UserBirthDays { get; set; }
		[JsonProperty("RegisteredChannels")]
		public List<ulong> RegisteredChannels { get; set; }

		public void ValidateData()
		{
			if (UserBirthDays == null) UserBirthDays = new Dictionary<ulong, DateTimeOffset>();
			if (RegisteredChannels == null) RegisteredChannels = new List<ulong>();
		}
	}
}
