using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace PosterTester.Dialogs
{
	public class AuthData : INotifyPropertyChanged
	{
		public string Domain {get => _Domain; set {_Domain = value; OnPropertyChanged(); }}
		private string _Domain = string.Empty;
		public string ClientId {get => _ClientId; set {_ClientId = value; OnPropertyChanged(); }}
		private string _ClientId = string.Empty;
		public string Audience {get => _Audience; set {_Audience = value; OnPropertyChanged(); }}
		private string _Audience = string.Empty;
		public string IdentityToken {get => _IdentityToken; set {_IdentityToken = value; OnPropertyChanged(); }}
		private string _IdentityToken = string.Empty;
        public string AccessToken {get => _AccessToken; set {_AccessToken = value; OnPropertyChanged(); }}
		private string _AccessToken = string.Empty;
		public string RefreshToken {get => _RefreshToken; set {_RefreshToken = value; OnPropertyChanged(); }}
		private string _RefreshToken = string.Empty;
		public string Claims { get => _Claims; set { _Claims = value; OnPropertyChanged(); } }
		private string _Claims = string.Empty;

		private bool IsLoggedIn = false;
		public bool LoggedIn
		{
			get => IsLoggedIn;
			set
			{
				IsLoggedIn = value;
				OnPropertyChanged();
			}
		}

		public event PropertyChangedEventHandler? PropertyChanged;
		protected void OnPropertyChanged([CallerMemberName] string? name = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
		}
	}

	public partial class BrowseAuth : Window
	{
		public BrowseAuth()
		{
			InitializeComponent();
			var data = new AuthData();
			this.AuthOptions = data;
			this.DataContext = data;
		}

		public AuthData AuthOptions { get; }
		private Authentication? authentication = null;

		private void Window_Activated(object sender, EventArgs e)
		{
			this.dlgCount.SelectAll();
			this.dlgCount.Focus();
		}

		private async void LoginExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			if (authentication != null) {
				return;
			}

			authentication = new Authentication(Domain: this.AuthOptions.Domain, ClientId: this.AuthOptions.ClientId, Audience: this.AuthOptions.Audience);
			var result = await authentication.Login();
			if (result)
			{
				this.AuthOptions.IdentityToken = authentication.loginResult?.IdentityToken ?? string.Empty;
				this.AuthOptions.AccessToken = authentication.loginResult?.AccessToken ?? string.Empty;
				this.AuthOptions.RefreshToken = authentication.loginResult?.RefreshToken ?? string.Empty;
				// todo(Gustav): diplay this better
				this.AuthOptions.Claims = string.Join(" | ", authentication.loginResult?.User.Claims.Select(c => $"{c.Type}: {c.Value}") ?? Array.Empty<string>());
				this.AuthOptions.LoggedIn = true;
			}
			else
			{
				this.AuthOptions.IdentityToken = string.Empty;
				this.AuthOptions.AccessToken = string.Empty;
				this.AuthOptions.RefreshToken = string.Empty;
				this.AuthOptions.Claims = string.Empty;
				this.AuthOptions.LoggedIn = false;
				this.authentication = null;
			}
		}

		private async void LogoutExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			if (authentication == null)
			{
				return;
			}
			await authentication.Logout();
			authentication = null;
		}

		private void OkExecuted(object sender, ExecutedRoutedEventArgs e)
		{
			this.DialogResult = true;
			Close();
		}
	}
}
