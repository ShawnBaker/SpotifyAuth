// Copyright © 2020 Shawn Baker using the MIT License.
using Foundation;

[assembly: Xamarin.Forms.Dependency(typeof(Example.iOS.DeleteCookies))]

namespace Example.iOS
{
	public class DeleteCookies : IDeleteCookies
	{
		public void DeleteAll()
		{
			NSHttpCookieStorage cookieStorage = NSHttpCookieStorage.SharedStorage;
			foreach (var cookie in cookieStorage.Cookies)
			{
				cookieStorage.DeleteCookie(cookie);
			}
		}
	}
}
