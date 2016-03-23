using System;
using System.Collections.Generic;

namespace dmpapisdkcsharp.Requests
{
	public abstract class AbstractRequest : IRequest
	{
		public Dictionary<string, string> Parameters { get; set; }
		public string Service { get; set; }

		public AbstractRequest (string service)
		{
			this.Service = service;
			this.Parameters = new Dictionary<string, string> ();
		}

		public void SetParameters(Dictionary<string, string> parameters) {
			foreach (KeyValuePair<string, string> parameter in parameters) {
				this.Parameters.Add (parameter.Key, parameter.Value);
			}
		}
	}
}

