using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace PosterTester.Dialogs;

public partial class BrowseUrl : Window
{
	UrlData Url { get; set; }
	public BrowseUrl(string url)
	{
		InitializeComponent();
		this.Url = new UrlData(url);
		this.DataContext = this.Url;
	}

	private void OnOk(object sender, ExecutedRoutedEventArgs e)
	{
		this.DialogResult = true;
		Close();
	}
}

public class UrlData : INotifyPropertyChanged
{
	private string _absoluteUri = string.Empty;
	private string _authority = string.Empty;
	private string _dnsSafeHost = string.Empty;
	private string _fragment = string.Empty;
	private string _host = string.Empty;
	private string _idnHost = string.Empty;
	private string _scheme = string.Empty;
	private string _userInfo = string.Empty;
	private int _port = 0;
	private bool _isDefaultPort = false;
	private bool _isAbsoluteUri = false;
	private bool _isFile = false;
	private bool _isLoopback = false;
	private bool _isUnc = false;
	private bool _userEscaped = false;
	private string[] _segments = {};
	public event PropertyChangedEventHandler? PropertyChanged = null;

	public UrlData(string url)
	{
		var u = new Uri(url);
		this.FromUrl(u);
	}

	private void FromUrl(Uri u)
	{
		this.AbsoluteUri = u.AbsoluteUri;
		this.Authority = u.Authority;
		this.DnsSafeHost = u.DnsSafeHost;
		this.Fragment = u.Fragment;
		this.Host = u.Host;
		this.IdnHost = u.IdnHost;
		this.IsDefaultPort = u.IsDefaultPort;
		this.IsAbsoluteUri = u.IsAbsoluteUri;
		this.IsFile = u.IsFile;
		this.IsLoopback = u.IsLoopback;
		this.IsUnc = u.IsUnc;
		this.Port = u.Port;
		this.Scheme = u.Scheme;
		this.Segments = u.Segments;
		this.UserEscaped = u.UserEscaped;
		this.UserInfo = u.UserInfo;
	}

	private static Uri? ParseUri(string u)
	{
		if (Uri.TryCreate(u, UriKind.RelativeOrAbsolute, out var uri))
		{
			return uri;
		}
		else
		{
			return null;
		}
	}

	public string AbsoluteUri
	{
		get => this._absoluteUri;
		set
		{
			if (SetField(ref this._absoluteUri, value))
			{
				var u = ParseUri(this._absoluteUri);
				if (u == null) return;
				FromUrl(u);
			}
		}
	}

	public string Authority
	{
		get => this._authority;
		set => SetField(ref this._authority, value);
	}

	public string DnsSafeHost
	{
		get => this._dnsSafeHost;
		set => SetField(ref this._dnsSafeHost, value);
	}

	public string Fragment
	{
		get => this._fragment;
		set => SetField(ref this._fragment, value);
	}

	public string Host
	{
		get => this._host;
		set => SetField(ref this._host, value);
	}

	public string IdnHost
	{
		get => this._idnHost;
		set => SetField(ref this._idnHost, value);
	}

	public string Scheme
	{
		get => this._scheme;
		set => SetField(ref this._scheme, value);
	}

	public string UserInfo
	{
		get => this._userInfo;
		set => SetField(ref this._userInfo, value);
	}

	public int Port
	{
		get => this._port;
		set => SetField(ref this._port, value);
	}

	public bool IsDefaultPort
	{
		get => this._isDefaultPort;
		set => SetField(ref this._isDefaultPort, value);
	}

	public bool IsAbsoluteUri
	{
		get => this._isAbsoluteUri;
		set => SetField(ref this._isAbsoluteUri, value);
	}

	public bool IsFile
	{
		get => this._isFile;
		set => SetField(ref this._isFile, value);
	}

	public bool IsLoopback
	{
		get => this._isLoopback;
		set => SetField(ref this._isLoopback, value);
	}

	public bool IsUnc
	{
		get => this._isUnc;
		set => SetField(ref this._isUnc, value);
	}

	public bool UserEscaped
	{
		get => this._userEscaped;
		set => SetField(ref this._userEscaped, value);
	}

	public string[] Segments
	{
		get => this._segments;
		set => SetField(ref this._segments, value);
	}


	protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
	{
		this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}

	protected bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
	{
		if (EqualityComparer<T>.Default.Equals(field, value)) return false;
		field = value;
		OnPropertyChanged(propertyName);
		return true;
	}
}
