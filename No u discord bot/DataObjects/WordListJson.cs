using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace No_u_discord_bot.DataObjects
{
	class WordListJson : IDataFile
	{
		[JsonProperty("words")]
		public string[] WordArray { get; private set; }

		public void ValidateData()
		{
			if (WordArray == null) WordArray = new string[] { };
		}
	}
}
