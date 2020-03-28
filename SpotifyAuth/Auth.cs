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
	public static class Auth
	{
		// public properties
		public static string ClientId { get; private set; } = "";
		public static Scope Scope { get; private set; } = Scope.None;
		public static string AuthUrl { get; private set; } = "";
		public static string RedirectUrl { get; private set; } = "";
		public static string Error { get; private set; } = "";
		public static string Code { get; private set; } = "";
		public static string Guid { get; private set; } = "";
		public static string AccessToken => (tokens != null) ? tokens.AccessToken : "";
		public static string RefreshToken => (tokens != null) ? tokens.RefreshToken : "";
		public static string TokenType => (tokens != null) ? tokens.TokenType : "";
		public static string GrantedScope => (tokens != null) ? tokens.Scope : "";
		public static double ExpiresIn => (tokens != null) ? tokens.ExpiresIn : 0;
		public static bool IsAuthorized => !string.IsNullOrEmpty(Guid) && !string.IsNullOrEmpty(AccessToken);
		public static bool IsUnauthorized => string.IsNullOrEmpty(Guid);

		// public events
		public static event EventHandler Changed = null;

		// private variables
		private static Timer refreshTimer = null;
		private static Tokens tokens = null;

		/// <summary>
		/// Request an authorization code from the server.
		/// </summary>
		/// <param name="clientId">Client ID to use for the request.</param>
		/// <param name="scope">Scope to use for the request.</param>
		/// <param name="authUrl">URL of the authorization server.</param>
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

			// get the URL
			string url = AuthUrl +
							"?action=auth" +
							"&guid=" + Guid +
							"&client_id=" + ClientId +
							"&scope=" + Scope.GetFlagsDescription() +
							"&redirect_uri=" + RedirectUrl +
							"&platform=" + DeviceInfo.Platform +
							"&version=" + DeviceInfo.VersionString +
							"&idiom=" + DeviceInfo.Idiom;

			// perform authorization in the web view or in the browser
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
			tokens = await GetTokenAsync("token", Code);
			if (tokens != null)
			{
				StartRefreshTimer();
				Changed?.Invoke(ClientId, EventArgs.Empty);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Resets the state to unauthorized.
		/// </summary>
		public static void Reset()
		{
			StopRefreshTimer();

			ClientId = "";
			Scope = Scope.None;
			AuthUrl = "";
			RedirectUrl = "";
			Error = "";
			Code = "";
			Guid = "";
			tokens = null;
		}

		/// <summary>
		/// Refreshes the access token immediately.
		/// </summary>
		public static void Refresh()
		{
			if (IsAuthorized)
			{
				RefreshTimer_Elapsed(null, null);
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

				tokens = JsonConvert.DeserializeObject<Tokens>(await response.Content.ReadAsStringAsync());
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
		private static void StartRefreshTimer()
		{
			StopRefreshTimer();
			if (tokens != null)
			{
				refreshTimer = new Timer(tokens.ExpiresIn * 1000);
				refreshTimer.Elapsed += RefreshTimer_Elapsed;
				refreshTimer.AutoReset = false;
				refreshTimer.Start();
			}
		}

		/// <summary>
		/// Stop the refresh timer.
		/// </summary>
		private static void StopRefreshTimer()
		{
			if (refreshTimer != null)
			{
				refreshTimer.Stop();
				refreshTimer.Dispose();
				refreshTimer = null;
			}
		}

		/// <summary>
		/// Refresh the access token when the timer goes off.
		/// </summary>
		private static async void RefreshTimer_Elapsed(object sender, ElapsedEventArgs e)
		{
			StopRefreshTimer();
			tokens = await GetTokenAsync("refresh", tokens.RefreshToken);
			if (tokens != null)
			{
				StartRefreshTimer();
			}
			Changed?.Invoke(ClientId, EventArgs.Empty);
		}
	}
}
