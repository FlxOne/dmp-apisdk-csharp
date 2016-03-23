using System;

namespace dmpapisdkcsharp.Configs
{
	public abstract class AbstractConfig : IConfig
	{
		public string Endpoint { get; set; }
		public string Username { get; set; }
		public string Password { get; set; }
		public int MaxRetries { get; set; }

		public AbstractConfig ()
		{
		}

		public void SetCredentials(string username, string password) {
			this.Username = username;
			this.Password = password;
		}
	}
}

