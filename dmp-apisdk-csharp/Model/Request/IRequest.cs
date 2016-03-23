using System;
using System.Collections.Generic;

namespace dmpapisdkcsharp.Requests
{
	public interface IRequest
	{
		Dictionary<string, string> Parameters { get; set; } 
		string Service { get; set; }
	}
}

