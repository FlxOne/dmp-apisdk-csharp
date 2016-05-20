using System;

namespace dmpapisdkcsharp.Configs
{
	public class Config : AbstractConfig, IConfig
	{
		public static IConfig GetDefault() {
			IConfig config = new Config ();
			config.Endpoint = "https://platform.flxone.com/api";
			config.MaxRetries = 5;
			return config;
		}
	}
}

