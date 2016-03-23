using System;
using Newtonsoft.Json.Linq;

namespace dmpapisdkcsharp.Responses
{
	public abstract class AbstractResponse : IResponse
	{
		public JObject JsonOuterResponseObject { get; set;	}

		public AbstractResponse(string json)
		{
			JsonOuterResponseObject = JObject.Parse(json);
		}

		public ResponseStatus GetStatus ()
		{
			try {
				if (this.GetResponseObject()["status"].Value<string>() == "OK") {
					return ResponseStatus.OK;
				}
			} catch (Exception e) {
				//throw new ClientException (e);
			}
			return ResponseStatus.ERROR;
		}

		protected JToken GetResponseObject ()
		{
			return this.JsonOuterResponseObject.GetValue("response");
		}

		public bool Has(string memberName) {
			return this.GetResponseObject()[memberName] != null;
		}

		public JToken Get(string memberName) {
			return this.GetResponseObject()[memberName];
		}

		public string GetCsrfToken() {
			if (this.Has ("csrf")) {
				return this.Get("csrf").Value<string>();
			}
			return "";
		}
	}
}

