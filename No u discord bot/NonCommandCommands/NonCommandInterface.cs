using DSharpPlus;
using DSharpPlus.EventArgs;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace No_u_discord_bot.NonCommandCommands
{
	interface NonCommandInterface
	{
		bool MeetsRequirements(MessageCreateEventArgs e, DiscordClient botClient, string loweredMessage);
		Task ExcuteCommand(MessageCreateEventArgs e, DiscordClient botClient);
	}
}
