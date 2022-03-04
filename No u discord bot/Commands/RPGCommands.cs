using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using No_u_discord_bot.Helpers;
using No_u_discord_bot.LooseSystems;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace No_u_discord_bot.Commands
{
	class RPGCommands : BaseCommandModule
	{
		//[Command("DisplayMap"), Description("Syntax: $[Command]\nStops the bot from reacting to non-command sentences"), CommandCustomGroupAttribute("NonCommand Commands")]
		//public async Task DisplayMap(CommandContext commandContext)
		//{
		//	RPGMapGenerator mapGenerator = new RPGMapGenerator();
		//	Bitmap RPGMap = mapGenerator.GenerateMap();

		//	using (FileStream stream = File.Create(Environment.CurrentDirectory + "\\DataObjects\\RPGMaps\\" + commandContext.Channel.Id + ".png"))
		//	{
		//		RPGMap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
		//	}

		//	using (FileStream stream = File.Open(Environment.CurrentDirectory + "\\DataObjects\\RPGMaps\\" + commandContext.Channel.Id + ".png", FileMode.Open))
		//	{
		//		DiscordMessageBuilder messageBuilder = new DiscordMessageBuilder();
		//		messageBuilder.WithFile(stream);
		//		await commandContext.Channel.SendMessageAsync(messageBuilder);
		//	}
		//}

		[Command("DisplayMap"), Description("Syntax: $[Command]\nStops the bot from reacting to non-command sentences"), CommandCustomGroupAttribute("NonCommand Commands")]
		public async Task DisplayMap(CommandContext commandContext)
		{
			DateTime startDisplay = DateTime.Now;
			await commandContext.Channel.SendMessageAsync("Generating map");
			MushroomRPG.MRPGMapGenerator mapGenerator = new MushroomRPG.MRPGMapGenerator(100, 100);
			MushroomRPG.MRPGMapVisualizer mapVisualizer = new MushroomRPG.MRPGMapVisualizer();
			await commandContext.Channel.SendMessageAsync("Map generated, visualizing");
			Bitmap visualMap = mapVisualizer.VisualizeMap(mapGenerator);

			using (FileStream stream = File.Create(Environment.CurrentDirectory + "\\DataObjects\\RPGMaps\\" + commandContext.Channel.Id + ".png"))
			{
				visualMap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
			}

			using (FileStream stream = File.Open(Environment.CurrentDirectory + "\\DataObjects\\RPGMaps\\" + commandContext.Channel.Id + ".png", FileMode.Open))
			{
				DiscordMessageBuilder messageBuilder = new DiscordMessageBuilder();
				messageBuilder.WithFile(stream);
				await commandContext.Channel.SendMessageAsync(messageBuilder);
			}
			DateTime endDisplay = DateTime.Now;
			TimeSpan secondsWaiting = endDisplay - startDisplay;
			await commandContext.Channel.SendMessageAsync("Time needed to generate: " + secondsWaiting.Seconds + " seconds");

		}
	}
}
