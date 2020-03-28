// Copyright © 2020 Shawn Baker using the MIT License.
using Xamarin.Forms;

namespace InternalWebView
{
	public partial class App : Application
	{
		public App()
		{
			InitializeComponent();

			MainPage = new NavigationPage(new MainPage());
		}

		protected override void OnStart()
		{
		}

		protected override void OnSleep()
		{
		}

		protected override void OnResume()
		{
		}
	}
}
