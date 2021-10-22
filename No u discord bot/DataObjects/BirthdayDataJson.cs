using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace No_u_discord_bot.DataObjects
{
	class BirthdayDataJson
	{
		[JsonProperty("BirthdaysOfUsers")]
		public Dictionary<ulong,DateTimeOffset> UserBirthDays { get; set; }
		[JsonProperty("RegisteredChannels")]
		public IList<ulong> RegisteredChannels { get; set; }
	}
}
