using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace No_u_discord_bot.DataObjects
{
	public class JsonParser
	{
		public enum FileEnum { ConfigFile, HangManWordsFile, BirthdayFile, ReminderFile };
		private Dictionary<FileEnum, string> fileLocations;
		private static JsonParser _instance;

		public static JsonParser GetInstance()
		{
			if (_instance == null) _instance = new JsonParser();
			return _instance;
		}

		private JsonParser()
		{
			fileLocations = new Dictionary<FileEnum, string>();
			fileLocations.Add(FileEnum.ConfigFile, "DataObjects//Config.json");
			fileLocations.Add(FileEnum.HangManWordsFile, "DataObjects//WordList.json");
			fileLocations.Add(FileEnum.BirthdayFile, "DataObjects//BirthdayData.json");
			fileLocations.Add(FileEnum.ReminderFile, "DataObjects//ReminderList.json");
		}

		public T LoadData<T>(FileEnum filePath) where T : IDataFile
		{
			string json = string.Empty;
			using (FileStream fileStream = File.OpenRead(fileLocations[filePath]))
			{
				using (StreamReader streamReader = new StreamReader(fileStream, new UTF8Encoding(false)))
				{
					json = streamReader.ReadToEnd();
				}
			}
			T jsonOutput = JsonConvert.DeserializeObject<T>(json);
			jsonOutput.ValidateData();

			return jsonOutput;
		}

		public void SaveData(FileEnum file, object dataClass)
		{
			using (StreamWriter streamWriter = new StreamWriter(fileLocations[file], false, new UTF8Encoding(false)))
			{
				string newJson = JsonConvert.SerializeObject(dataClass);
				streamWriter.Write(newJson);
			}
		}
	}
}
