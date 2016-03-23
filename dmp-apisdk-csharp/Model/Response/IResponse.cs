using System;
using Newtonsoft.Json.Linq;

namespace dmpapisdkcsharp.Responses
{
	public interface IResponse
	{
		bool Has(string memberName);
		JToken Get(string memberName);
		string GetCsrfToken();
	}
}

