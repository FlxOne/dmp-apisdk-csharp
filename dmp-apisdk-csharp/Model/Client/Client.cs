using System;
using dmpapisdkcsharp.Configs;

namespace dmpapisdkcsharp.Clients
{
	public class Client : AbstractClient, IClient
	{
		public Client (IConfig config) : base(config)
		{
		}
	}
}

