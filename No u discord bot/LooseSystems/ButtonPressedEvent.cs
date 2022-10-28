using DSharpPlus;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace No_u_discord_bot.LooseSystems
{
	static class ButtonPressedEvent
	{
		public delegate void ButtonEventFired(DiscordClient botClient, DiscordChannel channelMessageWasSend, DiscordUser personPressingButton);
		private static Dictionary<string, List<ButtonEventFired>> _listenerList = new Dictionary<string, List<ButtonEventFired>>();

		public static void ButtonPressed(string id, DiscordClient botClient, DiscordChannel channelMessageWasSend, DiscordUser userPressingButton)
		{
			if(_listenerList.ContainsKey(id))
			{
				foreach (ButtonEventFired registerdEvent in _listenerList[id])
				{
					registerdEvent.Invoke(botClient, channelMessageWasSend, userPressingButton);
				}
			}
		}

		public static void AddListenerer(string buttonId, ButtonEventFired targetMethod)
		{
			if(!_listenerList.ContainsKey(buttonId))
			{
				_listenerList.Add(buttonId, new List<ButtonEventFired>());
			}
			_listenerList[buttonId].Add(targetMethod);
		}
	}
}
