using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace No_u_discord_bot.DataObjects
{
	public class ConfigJson : IDataFile
	{
		[JsonProperty("token")]
		public string Token { get; private set; }
		[JsonProperty("prefix")]
		public string[] CommandPrefixs { get; private set; }
		[JsonProperty("silencedServerArray")]
		public ulong[] SilencedServerArray { get; set; }

		public void ValidateData()
		{
			if (Token == null) Token = string.Empty;
			if (CommandPrefixs == null) CommandPrefixs = new string[] { };
			if (SilencedServerArray == null) SilencedServerArray = new ulong[] { };
		}
	}
}
