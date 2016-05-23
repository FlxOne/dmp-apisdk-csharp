using System;
using dmpapisdkcsharp.Configs;
using dmpapisdkcsharp.Requests;
using System.Net.Http;
using System.Web;
using dmpapisdkcsharp.Responses;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Threading;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Threading.Tasks;
using log4net;
using log4net.Config;
using System.IO;
using dmpapisdkcsharp.Clients.Exceptions;


namespace dmpapisdkcsharp.Clients
{
	public class AbstractClient : IClient
	{
		protected HttpClient client;
		protected IConfig config;
		protected string authToken = "";
		protected string csrfToken = "";
		private Random random;

		public AbstractClient (IConfig config)
		{
			// Initialize LoggingExtensions so we can use `this.Log().Info("Some log line")`
			LoggingExtensions.Logging.Log.InitializeWith<LoggingExtensions.log4net.Log4NetLog> ();

			// Basic configuration for log4net
			BasicConfigurator.Configure();

			this.random = new Random();
			this.client = new HttpClient();
			this.config = config;
		}

		protected bool Authenticate() {
			IRequest request = new Request("auth");
			request.Parameters.Add ("username", this.config.Username);
			request.Parameters.Add ("password", this.config.Password);
			IResponse response = this.post(request);
			if (response == null) {
				return false;
			}
			this.authToken = response.Get("token").Value<string>();
			this.csrfToken = response.GetCsrfToken();
			return true;
		}

        protected IResponse Execute(HttpRequestMessage req)
        {
            this.Log().Info(string.Format("Executing {0} request to {1}", req.Method, req.RequestUri));

            if (!req.RequestUri.ToString().ToLower().Contains("auth"))
            {
                if (this.authToken == "" || this.csrfToken == "")
                {
                    if (!this.Authenticate())
                    {
                        throw new ClientException(new Exception("Failed to authenticate"));
                    }
                }
            }

            req.Headers.Add("X-Auth", this.authToken);
            req.Headers.Add("X-CSRF", this.csrfToken);

            HttpResponseMessage resp;
            Response response = null;
            for (int i = 0; i < this.config.MaxRetries; i++)
            {
                try
                {
                    // Clone the original HttpRequestMessage for retry purposes
                    resp = this.client.SendAsync(req).Result;

                    // Log the response
                    string responseBody = resp.Content.ReadAsStringAsync().Result;
                    this.Log().Info("{0}", responseBody);

                    int status = (int)resp.StatusCode;
                    Console.WriteLine("Status: " + status);
                    if (status == 401)
                    {
                        this.Authenticate();
                        continue;
                    }

                    response = new Response(responseBody);
                    if (response.GetStatus() == ResponseStatus.OK)
                    {
                        // Stop retrying
                        break;
                    }
                }
                catch (Exception ex)
                {
                   this.Log().Error(ex.Message);
                    try
                    {
                        Thread.Sleep((1000 * i * i) + random.Next(1, 101));
                    }
                    catch (ThreadInterruptedException ex2)
                    {
                        this.Log().Error(ex2.Message);
                    }
                }
            }
            return response;
        }


        public IResponse get (IRequest request)
		{
			try {
				HttpRequestMessage req = new HttpRequestMessage();
				req.Method = HttpMethod.Get;
				req.RequestUri = new Uri(this.config.Endpoint + "/" + request.Service + this.CreateURIFromURIBuilder(request));
				return this.Execute(req);
			} catch (Exception e) {
				this.Log().Error(e.Message);
				throw new ClientException (e);
			}
		}

		public IResponse put (IRequest request)
		{
			try {
				HttpRequestMessage req = new HttpRequestMessage();
				req.Method = HttpMethod.Put;
				req.Content = new FormUrlEncodedContent(request.Parameters);
				req.RequestUri = new Uri(this.config.Endpoint + "/" + request.Service);
				return this.Execute(req);
			} catch (Exception e) {
				this.Log().Error(e.Message);
				throw new ClientException (e);
			}
		}

		public IResponse delete (IRequest request)
		{
			try {
				HttpRequestMessage req = new HttpRequestMessage();
				req.Method = HttpMethod.Delete;
				req.RequestUri = new Uri(this.config.Endpoint + "/" + request.Service + this.CreateURIFromURIBuilder(request));

				return this.Execute(req);
			} catch (Exception e) {
				this.Log().Error(e.Message);
				throw new ClientException (e);
			}
		}

		public IResponse post (IRequest request)
		{
			try {
				HttpRequestMessage req = new HttpRequestMessage();
				req.Method = HttpMethod.Post;
				req.Content = new FormUrlEncodedContent(request.Parameters);
				req.RequestUri = new Uri(this.config.Endpoint + "/" + request.Service);
				return this.Execute(req);
			} catch (Exception e) {
				this.Log().Error(e.Message);
				throw new ClientException (e);
			}
		}

		protected string CreateURIFromURIBuilder(IRequest request) {
			// If no parameters are given, return an empty string
			if (request.Parameters.Count == 0) {
				return "";
			}

			NameValueCollection parameters = new NameValueCollection();
			foreach(KeyValuePair<string, string> parameter in request.Parameters) {
				parameters.Add(parameter.Key, parameter.Value);
			}
			var array = (from key in parameters.AllKeys
			             from value in parameters.GetValues (key)
			             select string.Format ("{0}={1}", HttpUtility.UrlEncode (key), HttpUtility.UrlEncode (value)))
				.ToArray ();
			
			return "?" + string.Join ("&", array);
		}

		private HttpRequestMessage CloneHttpRequestMessageAsync(HttpRequestMessage req)
		{
			HttpRequestMessage clone = new HttpRequestMessage(req.Method, req.RequestUri);

			// Copy the request's content (via a MemoryStream) into the cloned object
			var ms = new MemoryStream();
			if (req.Content != null)
			{
				req.Content.CopyToAsync(ms).ConfigureAwait(false);
				ms.Position = 0;
				clone.Content = new StreamContent(ms);

				// Copy the content headers
				if (req.Content.Headers != null)
					foreach (var h in req.Content.Headers)
						clone.Content.Headers.Add(h.Key, h.Value);
			}


			clone.Version = req.Version;

			foreach (KeyValuePair<string, object> prop in req.Properties)
				clone.Properties.Add(prop);

			foreach (KeyValuePair<string, IEnumerable<string>> header in req.Headers)
				clone.Headers.TryAddWithoutValidation(header.Key, header.Value);

			return clone;
		}
	}
}

