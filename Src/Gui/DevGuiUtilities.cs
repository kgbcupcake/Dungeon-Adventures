using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DungeonAdventures.Src.Interfaces
{
	public partial class DevGuiRenderer
	{
		public static class DevLog
		{
			public static List<string> Buffer = new List<string>();
			private static int _maxLines = 200;
			public static readonly object _lock = new object();

			public static void Write(string message, string type = "INFO")
			{
				if (string.IsNullOrWhiteSpace(message)) return;
				lock (_lock)
				{
					string timestamp = DateTime.Now.ToString("HH:mm:ss");
					string formattedLine = message.StartsWith("[")
						? $"{timestamp} {message}"
						: $"[{type}] {timestamp}: {message}";

					Buffer.Add(formattedLine);

					if (Buffer.Count > _maxLines)
					{
						Buffer.RemoveAt(0);
					}
				}
			}

			public static void Clear()
			{
				lock (_lock)
				{
					Buffer.Clear();
				}
			}
		}
	}
}
