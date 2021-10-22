using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace No_u_discord_bot.DataObjects
{
	class WordListJson
	{
		[JsonProperty("words")]
		public string[] WordArray { get; private set; }
	}
}
