using System;
using dmpapisdkcsharp.Responses;
using dmpapisdkcsharp.Requests;

namespace dmpapisdkcsharp.Clients
{
	public interface IClient
	{
		IResponse get(IRequest request);
		IResponse put(IRequest request);
		IResponse delete(IRequest request);
		IResponse post(IRequest request);
	}
}

