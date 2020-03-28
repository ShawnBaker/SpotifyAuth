// Copyright © 2020 Shawn Baker using the MIT License.
namespace InternalWebView.UWP
{
    public sealed partial class MainPage
    {
        public MainPage()
        {
            InitializeComponent();

            LoadApplication(new InternalWebView.App());
        }
    }
}
