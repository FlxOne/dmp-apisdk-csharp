using System;

namespace dmpapisdkcsharp
{
	public class ClientException : Exception
	{
		public ClientException (Exception outerException) : base(outerException.Message, outerException)
		{
		}
	}
}

