using System.ComponentModel;
using System.Timers;
using Xamarin.Forms;
using Xamarin.Essentials;
using FrozenNorth.SpotifyAuth;

namespace Example
{
	[DesignTimeVisible(false)]
	public partial class MainPage : ContentPage
	{
		// authentication constants
		private const string AuthClientId = "clientid";
		private const string AuthUrl = "authurl";
		private const string AuthRedirectUrl = "redirecturl";
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

		/// <summary>
		/// Initializes the page and starts authentication.
		/// </summary>
		public MainPage()
		{
			InitializeComponent();

			Authenticator.AuthChanged += Authenticator_AuthChanged;
			Authenticator.GetCodeAsync(AuthClientId, AuthScope, AuthUrl, AuthRedirectUrl);
			StartTimeoutTimer();
			SetState("");
		}

		/// <summary>
		/// Configures the UI based on the current state.
		/// </summary>
		/// <param name="error">Optional error message</param>
		private void SetState(string error = null)
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
				if (error != null)
				{
					errorLabel.Text = error;
				}
			});
		}

		/// <summary>
		/// Starts or stops the authentication process.
		/// </summary>
		private void loginButton_Clicked(object sender, System.EventArgs e)
		{
			if (loginButton.Text == "Login")
			{
				Authenticator.GetCodeAsync(AuthClientId, AuthScope, AuthUrl, AuthRedirectUrl);
				StartTimeoutTimer();
			}
			else
			{
				Authenticator.Reset();
				StopTimeoutTimer();
			}
			SetState("");
		}

		/// <summary>
		/// Refreshes the access token.
		/// </summary>
		private void refreshButton_Clicked(object sender, System.EventArgs e)
		{
			Authenticator.Refresh();
			StopTimeoutTimer();
			SetState();
		}

		private void Authenticator_AuthChanged(object sender, System.EventArgs e)
		{
			StopTimeoutTimer();
			SetState("");
		}

		/// <summary>
		/// Start the timeout timer.
		/// </summary>
		private void StartTimeoutTimer()
		{
			StopTimeoutTimer();
			timeoutTimer = new Timer(10000);
			timeoutTimer.Elapsed += HandleTimeoutTimerElapsed;
			timeoutTimer.AutoReset = false;
			timeoutTimer.Start();
		}

		/// <summary>
		/// Stop the timeout timer.
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
		/// Refresh the access token when the timer goes off.
		/// </summary>
		private void HandleTimeoutTimerElapsed(object sender, ElapsedEventArgs e)
		{
			StopTimeoutTimer();
			Authenticator.Reset();
			SetState("Request timed out.");
		}
	}
}
