// Copyright © 2020 Shawn Baker using the MIT License.
using System;
using Android.App;
using Android.Content;
using Android.OS;
using FrozenNorth.SpotifyAuth;

namespace Example.Droid
{
    [Activity(Label = "Spotify Authorization")]
    [IntentFilter(new[] { Intent.ActionView }, DataScheme = "spotauth", DataHost = "ca.frozen.spotauth", DataPathPrefix = "/auth", Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable })]
    public class AppLinkActivity : Activity
    {
        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // get the authorization response URI
            Android.Net.Uri uriAndroid = Intent.Data;
            Uri uri = new Uri(uriAndroid.ToString());

            // set the authorization code
            await Auth.SetCodeAsync(uri);

            // return to the main activity
            Finish();
        }
    }
}
