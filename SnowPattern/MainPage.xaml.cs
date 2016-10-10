using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using SnowPatternLNG.Resources;
using Microsoft.Phone.Tasks;
using System.Windows.Media.Imaging;

namespace SnowPattern
{
	public partial class MainPage : PhoneApplicationPage
	{
		// Constructor
		public MainPage()
		{
			this.InitializeComponent();

			SecureWordAdv.Source = new BitmapImage( new Uri( AppResources.AdvImageWP, UriKind.Relative ) );
		}

		protected override void OnNavigatedTo( System.Windows.Navigation.NavigationEventArgs e )
		{
			if ( App.Current.Game.ActiveGame )
			{
				ResumeGame.Visibility = Visibility.Visible;
			}
			base.OnNavigatedTo( e );
		}

		private void NewGameButtonClick( object sender, EventArgs e )
		{
			App.Current.Game.StartNewGame();
			NavigationService.Navigate( new Uri( "/GamePage.xaml", UriKind.Relative ) );
		}

		private void ResumeGameClick( object sender, EventArgs e )
		{
			NavigationService.Navigate( new Uri( "/GamePage.xaml", UriKind.Relative ) );
		}

		private void HyperlinkButtonClick( object sender, EventArgs e )
		{
		//	await Launcher.LaunchUriAsync( new Uri(  ) );
			System.Uri urlToOpen = new System.Uri( AppResources.AboutLink );
			var task = new Microsoft.Phone.Tasks.WebBrowserTask
			{
				Uri = urlToOpen
			};
			task.Show();
		}

		private void HelpClick( object sender, EventArgs e )
		{
			NavigationService.Navigate( new Uri( "/HelpPage.xaml", UriKind.Relative ) );
		}

		private void SecureWordAdvClick( object sender, RoutedEventArgs e )
		{
			MarketplaceDetailTask marketplaceDetailTask = new MarketplaceDetailTask();
			marketplaceDetailTask.ContentType = MarketplaceContentType.Applications;
			marketplaceDetailTask.ContentIdentifier = "d636c98c-e6b4-4877-afb4-205fc513bc8c";
			marketplaceDetailTask.Show();
		}
    }
}