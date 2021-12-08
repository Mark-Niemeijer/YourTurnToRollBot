using Newtonsoft.Json;
using No_u_discord_bot.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace No_u_discord_bot.DataObjects
{
	public class JsonParser
	{
		private class ParserInfo
		{
			public ParserInfo(FileEnum FileName, string FileLocation, Type FileType)
			{
				this.FileName = FileName;
				this.FileLocation = FileLocation;
				this.FileType = FileType;
			}

			public FileEnum FileName;
			public string FileLocation;
			public Type FileType;
		}

		private List<ParserInfo> parserInfos;
		public enum FileEnum { ConfigFile, HangManWordsFile, BirthdayFile, ReminderFile };
		private static JsonParser _instance;

		public static JsonParser GetInstance()
		{
			if (_instance == null) _instance = new JsonParser();
			return _instance;
		}

		private JsonParser()
		{
			parserInfos = new List<ParserInfo>();
			parserInfos.Add(new ParserInfo(FileEnum.ConfigFile, "DataObjects//Config.json", typeof(ConfigJson)));
			parserInfos.Add(new ParserInfo(FileEnum.HangManWordsFile, "DataObjects//WordList.json", typeof(WordListJson)));
			parserInfos.Add(new ParserInfo(FileEnum.BirthdayFile, "DataObjects//BirthdayData.json", typeof(BirthdayDataJson)));
			parserInfos.Add(new ParserInfo(FileEnum.ReminderFile, "DataObjects//ReminderList.json", typeof(ReminderListJson)));
		}

		public T LoadData<T>(FileEnum filePath) where T : IDataFile
		{
			string json = string.Empty;
			using (FileStream fileStream = File.OpenRead(parserInfos.First(i=>i.FileName == filePath).FileLocation))
			{
				using (StreamReader streamReader = new StreamReader(fileStream, new UTF8Encoding(false)))
				{
					json = streamReader.ReadToEnd();
				}
			}

			T jsonOutput = JsonConvert.DeserializeObject<T>(json);
			if (jsonOutput == null)
			{
				CustomDebugInfo.LogError("During loading the wrong filepath was provided or the file was empty");
				jsonOutput = (T)Activator.CreateInstance(typeof(T));
			}
			jsonOutput.ValidateData();

			return jsonOutput;
		}

		public void SaveData(IDataFile dataClass)
		{
			ParserInfo parserInfo = parserInfos.FirstOrDefault(i => i.FileType == dataClass.GetType());
			if (parserInfo == null)
			{
				CustomDebugInfo.LogError("During saving the FileType could not be matched to a location");
			}
			else
			{
				using (StreamWriter streamWriter = new StreamWriter(parserInfo.FileLocation, false, new UTF8Encoding(false)))
				{
					string newJson = JsonConvert.SerializeObject(dataClass);
					streamWriter.Write(newJson);
				}
			}
		}
	}
}
