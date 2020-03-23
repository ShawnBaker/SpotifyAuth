// Copyright © 2020 Shawn Baker using the MIT License.
using Android.Webkit;

[assembly: Xamarin.Forms.Dependency(typeof(Example.Droid.DeleteCookies))]

namespace Example.Droid
{
	public class DeleteCookies : IDeleteCookies
	{
		public void DeleteAll()
		{
			CookieManager.Instance.RemoveAllCookies(null);
		}
	}
}
