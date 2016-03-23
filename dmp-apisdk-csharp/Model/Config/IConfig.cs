using System;

namespace dmpapisdkcsharp.Configs
{
	public interface IConfig
	{
		string Endpoint { get; set; }
		string Username { get; set; }
		string Password { get; set; }
		int MaxRetries { get; set; }
		void SetCredentials(string username, string password);
	}
}

