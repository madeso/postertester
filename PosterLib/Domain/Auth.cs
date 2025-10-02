using System.Net.Http;
using System.Net.Http.Headers;

namespace PosterLib.Domain;

public interface Auth
{
	void Setup(HttpClient client);
}


public class NoAuth : Auth
{
	public void Setup(HttpClient client)
	{
		client.DefaultRequestHeaders.Authorization = null;
	}
}

public class BearerAuth(string token) : Auth
{
	public void Setup(HttpClient client)
	{
		client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
	}
}
