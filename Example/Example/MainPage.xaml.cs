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
		/// Constructor - Initializes the page and starts authorization.
		/// </summary>
		public MainPage()
		{
			InitializeComponent();

			Auth.Changed += Auth_Changed;
			loginButton_Clicked(this, EventArgs.Empty);
		}

		/// <summary>
		/// Configures the UI based on the current state.
		/// </summary>
		private void SetState()
		{
			MainThread.BeginInvokeOnMainThread(() =>
			{
				if (Auth.IsUnauthorized)
				{
					statusLabel.Text = "Logged out.";
					loginButton.Text = "Login";
					refreshButton.IsEnabled = false;
				}
				else if (!Auth.IsAuthorized)
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
		/// Starts or stops the authorization process.
		/// </summary>
		private async void loginButton_Clicked(object sender, EventArgs e)
		{
			if (loginButton.Text == "Login")
			{
				LoginPage loginPage = new LoginPage();
				await Navigation.PushAsync(loginPage);
			}
			else
			{
				//DependencyService.Get<IDeleteCookies>().DeleteAll();
				Auth.Reset();
			}
			SetState();
		}

		/// <summary>
		/// Refreshes the access token.
		/// </summary>
		private void refreshButton_Clicked(object sender, EventArgs e)
		{
			Auth.Refresh();
			SetState();
		}

		/// <summary>
		/// Pop the login page when authorization succeeds.
		/// </summary>
		private void Auth_Changed(object sender, EventArgs e)
		{
			if (Auth.IsAuthorized && Navigation.NavigationStack[Navigation.NavigationStack.Count - 1] is LoginPage)
			{
				Navigation.PopAsync();
			}
			SetState();
		}
	}
}
