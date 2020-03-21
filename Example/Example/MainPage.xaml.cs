// Copyright © 2020 Shawn Baker using the MIT License.
using System;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Essentials;
using FrozenNorth.SpotifyAuth;

namespace Example
{
	[DesignTimeVisible(false)]
	public partial class MainPage : ContentPage
	{
		/// <summary>
		/// Constructor - Initializes the page and starts authentication.
		/// </summary>
		public MainPage()
		{
			InitializeComponent();

			Authenticator.AuthChanged += Authenticator_AuthChanged;
			loginButton_Clicked(this, EventArgs.Empty);
		}

		/// <summary>
		/// Configures the UI based on the current state.
		/// </summary>
		private void SetState()
		{
			MainThread.BeginInvokeOnMainThread(() =>
			{
				if (string.IsNullOrEmpty(Authenticator.Guid))
				{
					statusLabel.Text = "Logged out.";
					loginButton.Text = "Login";
					refreshButton.IsEnabled = false;
				}
				else if (string.IsNullOrEmpty(Authenticator.AccessToken))
				{
					statusLabel.Text = "Logging in...";
					loginButton.Text = "Cancel";
					refreshButton.IsEnabled = false;
				}
				else
				{
					statusLabel.Text = "Logged in.";
					loginButton.Text = "Logout";
					refreshButton.IsEnabled = true;
				}
			});
		}

		/// <summary>
		/// Starts or stops the authentication process.
		/// </summary>
		private async void loginButton_Clicked(object sender, System.EventArgs e)
		{
			if (loginButton.Text == "Login")
			{
				LoginPage loginPage = new LoginPage();
				await Navigation.PushAsync(loginPage);
			}
			else
			{
				Authenticator.Reset();
			}
			SetState();
		}

		/// <summary>
		/// Refreshes the access token.
		/// </summary>
		private void refreshButton_Clicked(object sender, System.EventArgs e)
		{
			Authenticator.Refresh();
			SetState();
		}

		private void Authenticator_AuthChanged(object sender, System.EventArgs e)
		{
			SetState();
		}
	}
}
