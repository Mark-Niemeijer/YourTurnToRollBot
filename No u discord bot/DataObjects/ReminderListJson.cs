using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace No_u_discord_bot.DataObjects
{
	class ReminderListJson
	{
		[JsonProperty("UserID")]
		public List<ulong> UserID { get; set; }
		[JsonProperty("Channel")]
		public List<ulong> RemindInChannel { get; set; }
		[JsonProperty("DateReminder")]
		public List<DateTime> DateReminder { get; set; }
		[JsonProperty("ReminderMessage")]
		public List<string> ReminderMessage { get; set; }
	}
}
