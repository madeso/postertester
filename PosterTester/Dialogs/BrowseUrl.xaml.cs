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

	public event PropertyChangedEventHandler PropertyChanged;

	public UrlData(string url)
	{
		var u = new Uri(url);
		this.FromUrl(u);
	}

	private void FromUrl(Uri u)
	{
		this.AbsolutePath = u.AbsolutePath;
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

	public string AbsolutePath { get; set; }
	public string AbsoluteUri { get; set; }
	public string Authority { get; set; }
	public string DnsSafeHost { get; set; }
	public string Fragment { get; set; }
	public string Host { get; set; }
	public string IdnHost { get; set; }
	public bool IsDefaultPort { get; set; }
	public bool IsAbsoluteUri { get; set; }
	public bool IsFile { get; set; }
	public bool IsLoopback { get; set; }
	public bool IsUnc { get; set; }
	public int Port { get; set; }
	public string Scheme { get; set; }
	public string[] Segments { get; set; }
	public bool UserEscaped { get; set; }
	public string UserInfo { get; set; }

	protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
	{
		this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}

	protected bool SetField<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
	{
		if (EqualityComparer<T>.Default.Equals(field, value)) return false;
		field = value;
		OnPropertyChanged(propertyName);
		return true;
	}
}
