// Copyright © 2020 Shawn Baker using the MIT License.
using System;
using System.Timers;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;
using FrozenNorth.SpotifyAuth;

namespace InternalWebView
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class LoginPage : ContentPage
	{
		// authorization constants
		private const int TimeoutTime = 60000;
		private const int FailureTime = 2000;

		private const string AuthClientId = "a54141e0583a491787fc05837bc30979";
		private const string AuthUrl = "https://spinclass.club/auth/index.php";
		private const string AuthRedirectUrl = "https://spinclass.club/auth/redirect.php";
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
		/// Constructor - Initializes the page.
		/// </summary>
		public LoginPage()
		{
			InitializeComponent();

			webView.Navigated += WebView_Navigated;
		}

		/// <summary>
		/// Starts authorization.
		/// </summary>
		protected override void OnAppearing()
		{
			base.OnAppearing();

			if (Auth.IsUnauthorized)
			{
				Auth.RequestCode(AuthClientId, AuthScope, AuthUrl, AuthRedirectUrl, webView);
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
		/// Sets the authorization code and exchanges it for the access and refresh tokens.
		/// </summary>
		private async void WebView_Navigated(object sender, WebNavigatedEventArgs e)
		{
			if (e.Url.StartsWith(AuthRedirectUrl))
			{
				// get the authorization response URI
				Uri uri = new Uri(e.Url);

				// set the authorization code
				if (await Auth.SetCodeAsync(uri))
				{
					await Navigation.PopAsync();
				}
				else
				{
					StartFailureTimer();
				}
			}
			else
			{
				loginLabel.IsVisible = false;
			}
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
				loginLabel.Text = "Failed to login to Spotify.";
				loginLabel.TextColor = Color.Red;
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
		/// Closes this page.
		/// </summary>
		private void HandleFailureTimerElapsed(object sender, ElapsedEventArgs e)
		{
			StopFailureTimer();
			MainThread.BeginInvokeOnMainThread(async () =>
			{
				await Navigation.PopAsync();
			});
		}
	}
}