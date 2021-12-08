using DSharpPlus;
using No_u_discord_bot.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace No_u_discord_bot.NonCommandCommands
{
	public class Non_Prefix_Commands
	{
		private List<NonCommandInterface> possibleCommands;
		public Non_Prefix_Commands()
		{
			possibleCommands = new List<NonCommandInterface>();

			var classTypesImplementingInterface = AppDomain.CurrentDomain.GetAssemblies()
				.SelectMany(x => x.GetTypes())
				.Where(mytype => typeof(NonCommandInterface).IsAssignableFrom(mytype) && mytype.GetInterfaces()
				.Contains(typeof(NonCommandInterface))); 

			foreach (Type classWithType in classTypesImplementingInterface)
			{
				possibleCommands.Add((NonCommandInterface)Activator.CreateInstance(classWithType));
			}
		}

		public async Task OnClientMessageCreated(DiscordClient sender, DSharpPlus.EventArgs.MessageCreateEventArgs e)
		{
			bool SilentInChannel = Array.Exists(JsonParser.GetInstance().LoadData<ConfigJson>(JsonParser.FileEnum.ConfigFile).SilencedServerArray, el => el ==  e.Channel.Id);
			if (e.Message.Author == sender.CurrentUser || SilentInChannel) return;

			string loweredMessage = e.Message.Content.ToLower();
			foreach (NonCommandInterface command in possibleCommands)
			{
				if(command.MeetsRequirements(e, sender, loweredMessage))
				{
					await command.ExcuteCommand(e, sender);
					break;
				}
			}
		}
	}
}
