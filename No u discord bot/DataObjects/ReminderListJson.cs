using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace No_u_discord_bot.DataObjects
{
	class ReminderListJson : IDataFile
	{
		[JsonProperty("UserID")]
		public List<ulong> UserID { get; set; }
		[JsonProperty("Channel")]
		public List<ulong> RemindInChannel { get; set; }
		[JsonProperty("DateReminder")]
		public List<DateTime> DateReminder { get; set; }
		[JsonProperty("ReminderMessage")]
		public List<string> ReminderMessage { get; set; }

		public void ValidateData()
		{
			if (UserID == null) UserID = new List<ulong>();
			if (RemindInChannel == null) RemindInChannel = new List<ulong>();
			if (DateReminder == null) DateReminder = new List<DateTime>();
			if (ReminderMessage == null) ReminderMessage = new List<string>();
		}
	}
}
