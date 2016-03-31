using System;

namespace dmpapisdkcsharp.Clients.Exceptions
{
	public class ClientException : Exception
	{
		public ClientException (Exception outerException) : base(outerException.Message, outerException)
		{
		}
	}
}

