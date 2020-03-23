// Copyright © 2020 Shawn Baker using the MIT License.
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Timers;
using System.Web;
using Newtonsoft.Json;
using Xamarin.Forms;
using Xamarin.Essentials;

namespace FrozenNorth.SpotifyAuth
{
	public static class Authenticator
	{
		// public properties
		public static string ClientId { get; private set; } = "";
		public static Scope Scope { get; private set; } = Scope.None;
		public static string AuthUrl { get; private set; } = "";
		public static string RedirectUrl { get; private set; } = "";
		public static bool IsAuthenticated { get; private set; } = false;
		public static bool IsLoggedOut => string.IsNullOrEmpty(Guid);
		public static bool IsLoggedIn => !string.IsNullOrEmpty(Guid) && !string.IsNullOrEmpty(Authenticator.AccessToken);
		public static string Error { get; private set; } = "";
		public static string Code { get; private set; } = "";
		public static string Guid { get; private set; } = "";
		public static Tokens Tokens { get; private set; } = null;
		public static string AccessToken => (Tokens != null) ? Tokens.AccessToken : "";

		// public events
		public static event EventHandler AuthChanged = null;

		// private variables
		private static Timer timer = null;

		/// <summary>
		/// Request an authorization code from the server.
		/// </summary>
		/// <param name="clientId">Client ID to use for the request.</param>
		/// <param name="scope">Scope to use for the request.</param>
		/// <param name="authUrl">URL of the athentication server.</param>
		/// <param name="redirectUrl">Redirect URL to use for the request.</param>
		/// <param name="webView">Web view to display the requests in.</param>
		public static void RequestCode(string clientId, Scope scope, string authUrl, string redirectUrl, WebView webView = null)
		{
			// initialize the state
			Reset();
			Guid = System.Guid.NewGuid().ToString();
			ClientId = clientId;
			Scope = scope;
			AuthUrl = authUrl;
			RedirectUrl = redirectUrl;

			// perform authentication in the web view
			string url = AuthUrl +
							"?action=auth" +
							"&guid=" + Guid +
							"&client_id=" + ClientId +
							"&scope=" + Scope.GetFlagsDescription() +
							"&redirect_uri=" + RedirectUrl +
							"&platform=" + DeviceInfo.Platform +
							"&version=" + DeviceInfo.VersionString +
							"&idiom=" + DeviceInfo.Idiom;
			if (webView != null)
			{
				webView.Source = url;
			}
			else
			{
				Browser.OpenAsync(Uri.EscapeUriString(url));
			}
		}

		/// <summary>
		/// Set the authorization code from the response to the RequestCode().
		/// </summary>
		/// <param name="uri">URI of the response to the authorization request.</param>
		public static async Task<bool> SetCodeAsync(Uri uri)
		{
			// get and check the URI query fields
			var fields = HttpUtility.ParseQueryString(uri.Query);
			string state = fields.Get("state");
			string code = fields.Get("code");
			if (string.IsNullOrEmpty(state) || string.IsNullOrEmpty(code))
			{
				string error = fields.Get("error");
				Error = string.IsNullOrEmpty(error) ? "Unknown Error" : error;
				return false;
			}
			if (state != Guid)
			{
				Error = string.Format("Unknown Error", state, Guid);
				return false;
			}
			Code = code;

			// get the access and refresh tokens
			Tokens tokens = await GetTokenAsync("token", Code);
			if (tokens != null)
			{
				Tokens = tokens;
				IsAuthenticated = true;
				StartTimer();
				AuthChanged?.Invoke(Tokens, EventArgs.Empty);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Resets the state to unauthenticated.
		/// </summary>
		public static void Reset()
		{
			StopTimer();

			ClientId = "";
			Scope = Scope.None;
			AuthUrl = "";
			RedirectUrl = "";
			IsAuthenticated = false;
			Error = "";
			Code = "";
			Guid = "";
			Tokens = null;
		}

		/// <summary>
		/// Refreshes the access token immediately.
		/// </summary>
		public static void Refresh()
		{
			if (IsAuthenticated)
			{
				HandleTimerElapsed(null, null);
			}
		}

		/// <summary>
		/// Makes a request for a token.
		/// </summary>
		/// <param name="action">Type of token request, either "token" or "refresh".</param>
		/// <param name="value">Value to pass into the request.</param>
		/// <returns>The token or null.</returns>
		private static async Task<Tokens> GetTokenAsync(string action, string value = "")
		{
			try
			{
				FormUrlEncodedContent content = new FormUrlEncodedContent(new Dictionary<string, string>
				{
					{"action", action},
					{"guid", Guid},
					{"value", value}
				});
				HttpClient client = new HttpClient();
				HttpResponseMessage response = await client.PostAsync(AuthUrl, content);

				Tokens tokens = JsonConvert.DeserializeObject<Tokens>(await response.Content.ReadAsStringAsync());
				if (tokens != null && string.IsNullOrEmpty(tokens.Error) && !string.IsNullOrEmpty(tokens.AccessToken))
				{
					return tokens;
				}
			}
			catch
			{
			}

			return null;
		}

		/// <summary>
		/// Start the refresh timer.
		/// </summary>
		private static void StartTimer()
		{
			StopTimer();
			if (Tokens != null)
			{
				timer = new Timer(Tokens.ExpiresIn * 1000);
				timer.Elapsed += HandleTimerElapsed;
				timer.AutoReset = false;
				timer.Start();
			}
		}

		/// <summary>
		/// Stop the refresh timer.
		/// </summary>
		private static void StopTimer()
		{
			if (timer != null)
			{
				timer.Stop();
				timer.Dispose();
				timer = null;
			}
		}

		/// <summary>
		/// Refresh the access token when the timer goes off.
		/// </summary>
		private static async void HandleTimerElapsed(object sender, ElapsedEventArgs e)
		{
			StopTimer();
			Tokens tokens = await GetTokenAsync("refresh", Tokens.RefreshToken);
			if (tokens != null)
			{
				Tokens = tokens;
				StartTimer();
				AuthChanged?.Invoke(Tokens, EventArgs.Empty);
			}
		}
	}
}
