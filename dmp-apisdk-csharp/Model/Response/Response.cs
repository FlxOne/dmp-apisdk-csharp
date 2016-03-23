using System;

namespace dmpapisdkcsharp.Responses
{
	public class Response : AbstractResponse, IResponse
	{
		public Response (string json) : base(json)
		{
		}
	}
}

