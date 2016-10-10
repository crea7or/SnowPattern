using System;
using System.Linq;
using SnowPatternLNG.Resources;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Phone.Controls;
using System.Threading;
using System.ComponentModel;


namespace SnowPattern
{
	public sealed partial class GamePage : PhoneApplicationPage
	{
		private Snowflake[ , ] snowFlakes = new Snowflake[ 5, 7 ];
		private Snowflake[ , ] snowFlakesSamples = new Snowflake[ 5, 7 ];
		private DateTime gameStartTime = DateTime.Now;
		private bool accessHit = false;
		private bool hitsShown = false;

		public GamePage()
		{
			this.InitializeComponent();
			this.Loaded += GamePageLoaded;
			ShowGameStatistics();
			var timer = new DispatcherTimer();
			timer.Tick += timer_Tick; ;
			timer.Interval = new TimeSpan( 0, 0, 0, 1 );
			timer.Start();

		}

		void timer_Tick(object sender, object e)
		{
			ShowGameStatistics();
		}

		void GamePageLoaded( object sender, EventArgs e )
		{
			// Game Board
			var rows = Enumerable.Range( 0, 5 ).ToArray();
			var columns = Enumerable.Range( 0, 7 ).ToArray();

			foreach ( var row in rows )
			{
				foreach ( var column in columns )
				{
					var snowFlakeButton = new Snowflake();

					snowFlakeButton.VerticalAlignment = VerticalAlignment.Stretch;
					snowFlakeButton.HorizontalAlignment = HorizontalAlignment.Stretch;
					snowFlakeButton.Click += Click;

					if ( App.Current.Game.boardPlayer[ row, column ] == 0 )
					{
						snowFlakeButton.FlakeState = SnowFlakeState.None;
					}
					else
					{
						snowFlakeButton.FlakeState = SnowFlakeState.Set;
					}

					snowFlakes[ row, column ] = snowFlakeButton;

					Grid.SetRow( snowFlakeButton, row );
					Grid.SetColumn( snowFlakeButton, column );
					BoardGrid.Children.Add( snowFlakeButton );
				}
			}
			// Game Board
			SampleBoardLoaded();
			// Sample Board
		}

		void SampleBoardLoaded()
		{
			var rows = Enumerable.Range( 0, 5 ).ToArray();
			var columns = Enumerable.Range( 0, 7 ).ToArray();

			foreach ( var row in rows )
			{
				foreach ( var column in columns )
				{
					var snowFlakeButton = new Snowflake();
					snowFlakeButton.VerticalAlignment = VerticalAlignment.Stretch;
					snowFlakeButton.HorizontalAlignment = HorizontalAlignment.Stretch;
					if ( App.Current.Game.boardSample[ row, column ] == 0 )
					{
						snowFlakeButton.FlakeState = SnowFlakeState.None;
					}
					else
					{
						snowFlakeButton.FlakeState = SnowFlakeState.Set;
					}
					snowFlakesSamples[ row, column ] = snowFlakeButton;
					Grid.SetRow( snowFlakeButton, row );
					Grid.SetColumn( snowFlakeButton, column );
					SampleGrid.Children.Add( snowFlakeButton );
				}
			}
		}

		private void ChangeBoardState()
		{
			for ( int row = 0; row < 5; row++ )
			{
				for ( int col = 0; col < 7; col++ )
				{
					if ( App.Current.Game.boardPlayer[ row, col ] == 0 && snowFlakes[ row, col ].FlakeState == SnowFlakeState.Set )
					{
						snowFlakes[ row, col ].FlakeState = SnowFlakeState.None;
					}

					if ( App.Current.Game.boardPlayer[ row, col ] == 1 && snowFlakes[ row, col ].FlakeState == SnowFlakeState.None )
					{
						snowFlakes[ row, col ].FlakeState = SnowFlakeState.Set;
					}
				}
			}
		}

		private void ChangeSampleState()
		{
			for ( int row = 0; row < 5; row++ )
			{
				for ( int col = 0; col < 7; col++ )
				{
					if ( App.Current.Game.boardSample[ row, col ] == 0 ) //&& snowFlakesSamples[ row, col ].FlakeState == SnowFlakeState.Set )
					{
						snowFlakesSamples[ row, col ].FlakeState = SnowFlakeState.None;
					}

					if ( App.Current.Game.boardSample[ row, col ] == 1 ) //&& snowFlakesSamples[ row, col ].FlakeState == SnowFlakeState.None )
					{
						snowFlakesSamples[ row, col ].FlakeState = SnowFlakeState.Set;
					}
				}
			}
		}

		private void GridRightTapped( object sender, EventArgs e )
		{
			MessageBoxResult mbResult = MessageBox.Show( AppResources.GameQuestionResetBoard, AppResources.GameQuestionTitle, MessageBoxButton.OKCancel );
			if ( mbResult == MessageBoxResult.OK )
			{
				for ( int row = 0; row < 5; row++ )
				{
					for ( int col = 0; col < 7; col++ )
					{
						App.Current.Game.boardPlayer[ row, col ] = 0;
						snowFlakes[ row, col ].FlakeState = SnowFlakeState.None;
					}
				}
			}
		}

		private void Click( object sender, RoutedEventArgs e )
		{
			if ( accessHit == false )
			{
				accessHit = true;
				int columnIndex = Grid.GetColumn( sender as FrameworkElement );
				int rowIndex = Grid.GetRow( sender as FrameworkElement );
				App.Current.Game.StepToBoard( rowIndex, columnIndex, App.Current.Game.boardPlayer );
				ChangeBoardState();

				var timer = new DispatcherTimer();
				timer.Interval = TimeSpan.FromMilliseconds( 300 );
				timer.Tick += ( s, en ) => { timer.Stop(); OnGameCheckComplete(); };
				timer.Start();
			}
		}

		private void OnGameCheckComplete()
		{
			if ( App.Current.Game.IsWin() )
			{
				string winMessage;
				if ( App.Current.Game.SubLevel < 8 )
				{
					winMessage = String.Format( AppResources.GameLevelWon, App.Current.Game.SubLevel, App.Current.Game.Level );
				}
				else
				{
					winMessage = String.Format( AppResources.GameNextLevel, App.Current.Game.Level );
					if ( App.Current.Game.Level == 8 )
					{
						winMessage = AppResources.GameWon;
					}
				}

				GameBoardMessage.Visibility = Visibility.Visible;
				GameMessage.Text = winMessage;

				var timer = new DispatcherTimer();
				timer.Interval = TimeSpan.FromSeconds( 3 );
				timer.Tick += ( s, en ) => { timer.Stop(); OnGameComplete(); };
				timer.Start();
			}
			else
			{
				accessHit = false;
			}
		}

		private void OnGameComplete()
		{
			if ( App.Current.Game.SubLevel == 8 && App.Current.Game.Level == 8 )
			{
				App.Current.Game.SubLevel = 1;
				App.Current.Game.Level = 1;
				App.Current.Game.ActiveGame = false;
			}
			else
			{
				GameBoardMessage.Visibility = Visibility.Collapsed;

				accessHit = false;
				if ( App.Current.Game.SubLevel < 8 )
				{
					App.Current.Game.SubLevel++;
				}
				else
				{
					App.Current.Game.SubLevel = 1;
					App.Current.Game.Level++;
				}
				App.Current.Game.GenerateNewBoard();
				ChangeBoardState();
				ChangeSampleState();
				ShowGameStatistics();
				hitsShown = false;
			}
		}

		private void ShowGameStatistics()
		{
			GameStatGame.Text = App.Current.Game.SubLevel.ToString();
			GameStatLevel.Text = App.Current.Game.Level.ToString();

			TimeSpan gameTime = DateTime.Now - gameStartTime;
			gameStartTime = DateTime.Now;

			App.Current.Game.GameTime += gameTime;
			App.Current.Game.GameTotalTime += gameTime;

			TimeSpan gameTimeSpan = App.Current.Game.GameTime;
			TimeSpan gameTotalTimeSpan = App.Current.Game.GameTotalTime;

			string timeStr = String.Format( AppResources.GameTimeFormat, gameTimeSpan.Days, gameTimeSpan.Hours, gameTimeSpan.Minutes, gameTimeSpan.Seconds );
			GameStatTime.Text = timeStr;

			timeStr = String.Format( AppResources.GameTimeFormat, gameTotalTimeSpan.Days, gameTotalTimeSpan.Hours, gameTotalTimeSpan.Minutes, gameTotalTimeSpan.Seconds );
			GameStatTotalTime.Text = timeStr;
		}


		private void HintButtonClick( object sender, RoutedEventArgs e )
		{
			if ( hitsShown == false )
			{
				hitsShown = true;
				for ( int row = 0; row < 5; row++ )
				{
					for ( int col = 0; col < 7; col++ )
					{
						if ( App.Current.Game.boardSteps[ row, col ] == 1 )
						{
							snowFlakesSamples[ row, col ].FlakeState = SnowFlakeState.HintSet;
						}
					}
				}
			}
		}

		protected override void OnNavigatedTo( System.Windows.Navigation.NavigationEventArgs e )
		{
			base.OnNavigatedTo( e );
		}

	}
}
