// Copyright © 2020 Shawn Baker using the MIT License.
using System;
using System.ComponentModel;
using System.Timers;
using Xamarin.Forms;
using Xamarin.Essentials;
using FrozenNorth.SpotifyAuth;

namespace ExternalBrowser
{
	[DesignTimeVisible(false)]
	public partial class MainPage : ContentPage
	{
		// authorization constants
		private const int TimeoutTime = 60000;
		private const int FailureTime = 2000;

		private const string AuthClientId = "a54141e0583a491787fc05837bc30979";
		private const string AuthUrl = "https://spinclass.club/auth/index.php";
		private const string AuthRedirectUrl = "spotauth://ca.frozen.spotauth/auth";
		private const Scope AuthScope = //Scope.AppRemoteControl | 
										Scope.PlaylistModifyPrivate |
										Scope.PlaylistModifyPublic |
										Scope.PlaylistReadCollaborative |
										Scope.PlaylistReadPrivate |
										Scope.Streaming |
										// Scope.UgcImageUpload |
										Scope.UserFollowModify |
										Scope.UserFollowRead |
										Scope.UserLibraryModify |
										Scope.UserLibraryRead |
										Scope.UserModifyPlaybackState |
										Scope.UserReadCurrentlyPlaying |
										Scope.UserReadEmail |
										Scope.UserReadPlaybackState |
										Scope.UserReadPrivate |
										Scope.UserReadRecentlyPlayed |
										Scope.UserTopRead;

		// instance variables
		private Timer timeoutTimer = null;
		private Timer failureTimer = null;

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
		/// Starts authorization.
		/// </summary>
		protected override void OnAppearing()
		{
			base.OnAppearing();

			if (Auth.IsUnauthorized)
			{
				Auth.RequestCode(AuthClientId, AuthScope, AuthUrl, AuthRedirectUrl);
				StartTimeoutTimer();
			}
		}

		/// <summary>
		/// Stops the timers.
		/// </summary>
		protected override void OnDisappearing()
		{
			base.OnDisappearing();

			StopFailureTimer();
			StopTimeoutTimer();
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
		private void loginButton_Clicked(object sender, EventArgs e)
		{
			errorLabel.Text = "";
			if (loginButton.Text == "Login")
			{
				if (Auth.IsUnauthorized)
				{
					Auth.RequestCode(AuthClientId, AuthScope, AuthUrl, AuthRedirectUrl);
					StartTimeoutTimer();
				}
			}
			else
			{
				Auth.Reset();
			}
			SetState();
		}

		/// <summary>
		/// Refreshes the access token.
		/// </summary>
		private void refreshButton_Clicked(object sender, EventArgs e)
		{
			errorLabel.Text = "";
			Auth.Refresh();
			SetState();
		}

		/// <summary>
		/// Display the state when authorization succeeds.
		/// </summary>
		private void Auth_Changed(object sender, EventArgs e)
		{
			SetState();
		}

		/// <summary>
		/// Starts the timeout timer.
		/// </summary>
		private void StartTimeoutTimer()
		{
			StopTimeoutTimer();
			timeoutTimer = new Timer(TimeoutTime);
			timeoutTimer.Elapsed += HandleTimeoutTimerElapsed;
			timeoutTimer.AutoReset = false;
			timeoutTimer.Start();
		}

		/// <summary>
		/// Stops the timeout timer.
		/// </summary>
		private void StopTimeoutTimer()
		{
			if (timeoutTimer != null)
			{
				timeoutTimer.Stop();
				timeoutTimer.Dispose();
				timeoutTimer = null;
			}
		}

		/// <summary>
		/// Starts the failure timer.
		/// </summary>
		private void HandleTimeoutTimerElapsed(object sender, ElapsedEventArgs e)
		{
			StopTimeoutTimer();
			StartFailureTimer();
		}

		/// <summary>
		/// Starts the failure timer.
		/// </summary>
		private void StartFailureTimer()
		{
			StopFailureTimer();

			MainThread.BeginInvokeOnMainThread(() =>
			{
				errorLabel.Text = "Failed to login to Spotify.";
			});
			Auth.Reset();

			failureTimer = new Timer(FailureTime);
			failureTimer.Elapsed += HandleFailureTimerElapsed;
			failureTimer.AutoReset = false;
			failureTimer.Start();
		}

		/// <summary>
		/// Stops the failure timer.
		/// </summary>
		private void StopFailureTimer()
		{
			if (failureTimer != null)
			{
				failureTimer.Stop();
				failureTimer.Dispose();
				failureTimer = null;
			}
		}

		/// <summary>
		/// Stops authorization.
		/// </summary>
		private void HandleFailureTimerElapsed(object sender, ElapsedEventArgs e)
		{
			StopFailureTimer();
			SetState();
		}
	}
}
