// Copyright © 2020 Shawn Baker using the MIT License.
using Windows.UI.Xaml.Controls;

[assembly: Xamarin.Forms.Dependency(typeof(Example.UWP.DeleteCookies))]

namespace Example.UWP
{
	public class DeleteCookies : IDeleteCookies
	{
		public void DeleteAll()
		{
			WebView.ClearTemporaryWebDataAsync();
		}
	}
}
