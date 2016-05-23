using System;
using dmpapisdkcsharp.Configs;
using dmpapisdkcsharp.Clients;
using dmpapisdkcsharp.Requests;
using dmpapisdkcsharp.Responses;
using Newtonsoft.Json.Linq;

namespace dmpapisdkcsharp
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			#region Setup SDK
			IConfig config = Config.GetDefault();

			config.SetCredentials("your username", "your password");

			Client client = new Client(config);
			#endregion

			#region Example 1: Get the current user's information
			string service = "user/current";
			IRequest request = new Request(service);
			try {
				IResponse response = client.get(request);
				Console.WriteLine(response.Get("user")["first_name"] + " " + response.Get("user")["last_name"]);
			} catch (Exception ex) {
				Console.WriteLine ("Request failed: " + ex.Message);
			}
			#endregion

			#region Example 2: Create a Data Collection Pixel using a POST request
			 service = "tracking/beacon";
			 request = new Request(service);
			request.Parameters.Add("name", "C# SDK-Created Pixel");
			request.Parameters.Add("type", "smart_pixel");
			int pixelId = -1; // Store the pixelId for further usage in the PUT/DELETE examples
			try {
				IResponse response = client.post(request);
				pixelId = (int)response.Get("beacon")["id"];
				Console.WriteLine(string.Format("Created a new pixel with id [{0}] and name: [{1}]", pixelId, response.Get("beacon")["name"]));
			} catch (Exception ex) {
				Console.WriteLine ("Request failed: " + ex.Message);
			}
			#endregion

			#region Example 3: Change the name of our previously created Pixel using a PUT request
			 service = "tracking/beacon";
			 request = new Request(service);
			request.Parameters.Add("id", pixelId.ToString());
			request.Parameters.Add("name", "C# SDK-Created Pixel has had it's name changed");
			try {
				IResponse response = client.put(request);
				Console.WriteLine(string.Format("Changed the name of the pixel with id [{0}] and name: [{1}]", pixelId, response.Get("beacon")["name"]));
			} catch (Exception ex) {
				Console.WriteLine ("Request failed: " + ex.Message);
			}
			#endregion

			#region Example 4: Delete our previously created Pixel using a DELETE request
			 service = "tracking/beacon";
			 request = new Request(service);
			request.Parameters.Add("id", pixelId.ToString());
			try {
				IResponse response = client.delete(request);
				Console.WriteLine(string.Format("Did we delete the previously created Pixel? --> [{0}]", (bool)response.Get("deleted")));
			} catch (Exception ex) {
				Console.WriteLine ("Request failed: " + ex.Message);
			}
			#endregion
		}
	}
}