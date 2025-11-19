
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Auth0.OidcClient;
using IdentityModel.OidcClient;
using IdentityModel.OidcClient.Browser;
using Microsoft.Web.WebView2.Wpf;

namespace PosterTester;
internal class Authentication(string Domain, string ClientId, string Audience)
{
    public Auth0Client? client { get; set; }
    public LoginResult? loginResult { get; set; }

    public async Task<bool> Login()
    {
        client = new Auth0Client(new Auth0ClientOptions
        {
            Domain = Domain,
            ClientId = ClientId,
            Browser = new WebViewBrowser(),
        });

        var extraParameters = new Dictionary<string, string>
        {
            { "audience", Audience }
        };

        loginResult = await client.LoginAsync(extraParameters: extraParameters);

        if(loginResult.IsError)
        {
            MessageBox.Show($"{loginResult.Error}: ${loginResult.ErrorDescription}",
                "Failed to login", MessageBoxButton.OK, MessageBoxImage.Error);
            return false;
        }
        else
        {
            return true;
        }
    }

    public async Task<bool> Logout()
    {
		if (client == null)
		{
			return true;
		}

		var browserResult = await client.LogoutAsync();

        if (browserResult != BrowserResultType.Success)
        {
            var message = browserResult.ToString();
            MessageBox.Show(message);
            return false;
        }

        client = null;
        loginResult = null;

        return true;
    }
}

public class WebViewBrowser : IBrowser
{
	private readonly Func<Window> _windowFactory;

	public WebViewBrowser(Func<Window> windowFactory)
	{
		_windowFactory = windowFactory;
	}

	public WebViewBrowser(string title = "Authenticating...", int width = 1024, int height = 768)
		: this(() => new Window
		{
			Name = "WebAuthentication",
			Title = title,
			Width = width,
			Height = height
		})
	{
	}

	public async Task<BrowserResult> InvokeAsync(BrowserOptions options, CancellationToken cancellationToken = default)
	{
		var tcs = new TaskCompletionSource<BrowserResult>();

		var window = _windowFactory();
		var webView = new WebView2();

		webView.NavigationStarting += (sender, e) =>
		{
			if (e.Uri.StartsWith(options.EndUrl))
			{

				tcs.SetResult(new BrowserResult { ResultType = BrowserResultType.Success, Response = e.Uri.ToString() });
				window.Close();

			}

		};

		window.Closing += (sender, e) =>
		{
			if (!tcs.Task.IsCompleted)
				tcs.SetResult(new BrowserResult { ResultType = BrowserResultType.UserCancel });
		};


		window.Content = webView;
		window.Show();
		await webView.EnsureCoreWebView2Async();
		webView.CoreWebView2.Navigate(options.StartUrl);

		return await tcs.Task;
	}
}
