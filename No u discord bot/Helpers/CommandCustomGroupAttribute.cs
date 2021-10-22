using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace No_u_discord_bot.Helpers
{
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
	class CommandCustomGroupAttribute : CheckBaseAttribute
	{
		public string CustomGroupName { get; private set; }

		public CommandCustomGroupAttribute(string groupName)
		{
			CustomGroupName = groupName;
		}

		public override Task<bool> ExecuteCheckAsync(CommandContext ctx, bool help)
		{
			return Task.FromResult(true);
		}
	}
}
