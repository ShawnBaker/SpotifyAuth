// Copyright © 2020 Shawn Baker using the MIT License.
namespace ExternalBrowser.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();

            LoadApplication(new ExternalBrowser.App());
        }
    }
}
