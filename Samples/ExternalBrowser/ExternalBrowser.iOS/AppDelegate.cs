// Copyright © 2020 Shawn Baker using the MIT License.
using Foundation;
using UIKit;
using FrozenNorth.SpotifyAuth;

namespace ExternalBrowser.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            Xamarin.Forms.Forms.Init();
            LoadApplication(new App());

            return base.FinishedLaunching(app, options);
        }

        public override bool OpenUrl(UIApplication application, NSUrl url, string sourceApplication, NSObject annotation)
        {
            // get the authorization response URI
            System.Uri uri = new System.Uri(url.AbsoluteString);

            // set the authorization code
            Auth.SetCodeAsync(uri);
            return true;
        }
    }
}
