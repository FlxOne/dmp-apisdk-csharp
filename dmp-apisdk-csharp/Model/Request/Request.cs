using System;

namespace dmpapisdkcsharp.Requests
{
	public class Request : AbstractRequest, IRequest
	{
		public Request (string service) : base(service)
		{
		}
	}
}

