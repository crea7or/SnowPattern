using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace SnowPattern
{
	public class GameCore
	{
		public byte[,] boardSample = new byte[5, 7];
		public byte[,] boardSteps = new byte[5,7];
		public byte[,] boardPlayer = new byte[5,7];

		// Settings
		public int Level = 1;
		public int SubLevel = 1;
		public bool ActiveGame = false;
		public TimeSpan GameTime = new TimeSpan( 0 );
		public TimeSpan GameTotalTime = new TimeSpan( 0 );
		private string filename = "snowflakes.game";

		public GameCore()
		{

		}

		public void LoadData()
		{
			BinaryReader bReader = null;
			byte[] arrayOfBytes = new byte[ 5 * 7 ];

			try
			{
				using ( var appLocalStore = IsolatedStorageFile.GetUserStoreForApplication() )
				{
					bReader = new BinaryReader( appLocalStore.OpenFile( filename, FileMode.Open, FileAccess.Read ) );

					arrayOfBytes = bReader.ReadBytes( 5 * 7 );
					ArraySingleToDouble( arrayOfBytes, boardSample );

					arrayOfBytes = bReader.ReadBytes( 5 * 7 );
					ArraySingleToDouble( arrayOfBytes, boardSteps );

					arrayOfBytes = bReader.ReadBytes( 5 * 7 );
					ArraySingleToDouble( arrayOfBytes, boardPlayer );
					
					Level = bReader.ReadInt32();
					SubLevel = bReader.ReadInt32();

					double timeVal = bReader.ReadDouble();
					GameTime = TimeSpan.FromSeconds( timeVal );
					
					timeVal = bReader.ReadDouble();
					GameTotalTime = TimeSpan.FromSeconds( timeVal );

					int boolVal = bReader.ReadInt32();
					if ( boolVal == 1)
					{
						ActiveGame = true;
					}
					else
					{
						ActiveGame = false;
					}
					bReader.Close();
				}
			}
			catch ( System.Exception /*ex*/ )
			{
				Array.Clear( boardSteps, 0, boardSteps.Length );
				Array.Clear( boardSample, 0, boardSample.Length );
				Array.Clear( boardPlayer, 0, boardPlayer.Length );
				Level = 1;
				SubLevel = 1;
				ActiveGame = false;
				GameTime = TimeSpan.FromSeconds( 0.0 );
				GameTotalTime = TimeSpan.FromSeconds( 0.0 );
			}
			finally
			{
				if ( bReader != null )
				{
					( (IDisposable)bReader ).Dispose();
				}
			}
		}

		public void SaveData()
		{
			BinaryWriter bWriter = null;
			byte[] arrayOfBytes = new byte[ 5 * 7 ];

			try
			{
				using ( var appLocalStore = IsolatedStorageFile.GetUserStoreForApplication() )
				{
					bWriter = new BinaryWriter( appLocalStore.OpenFile( filename, FileMode.OpenOrCreate, FileAccess.ReadWrite ) );

					ArrayDoubleToSingle( boardSample, arrayOfBytes );
					bWriter.Write( arrayOfBytes );

					ArrayDoubleToSingle( boardSteps, arrayOfBytes );
					bWriter.Write( arrayOfBytes );

					ArrayDoubleToSingle( boardPlayer, arrayOfBytes );
					bWriter.Write( arrayOfBytes );
										
					bWriter.Write( Level );
					bWriter.Write( SubLevel );

					bWriter.Write( GameTime.TotalSeconds );
					bWriter.Write( GameTotalTime.TotalSeconds );

					int boolVal = 0;
					if ( ActiveGame )
					{
						boolVal = 1;
					}
					bWriter.Write( boolVal );

					bWriter.Close();
				}
			}
			catch( System.Exception /*ex*/ )
			{
			}
			finally
			{
				if( bWriter != null )
				{
					((IDisposable)bWriter ).Dispose();
				}
			}
		}
		
		private void ArraySingleToDouble( byte[] singleArray, byte[ , ] doubleArray )
		{
			int row, column;
			for ( int count = 0; count < singleArray.Length; count++ )
			{
				column = (int)( count % 7 );
				row = (int)( count / 7 );
				doubleArray[ row, column ] = singleArray[ count ];
			}
		}

		private void ArrayDoubleToSingle( byte[ , ] doubleArray, byte[] singleArray )
		{
			int row, column;
			for ( int count = 0; count < singleArray.Length; count++ )
			{
				column = (int)( count % 7 );
				row = (int)( count / 7 );
				singleArray[ count ] = doubleArray[ row, column ];
			}
		}

		public void StartNewGame()
		{
			// Generate game board
			ActiveGame = true;
			GameTime = TimeSpan.FromSeconds( 0 );
			GameTotalTime = TimeSpan.FromSeconds( 0 );
			GenerateNewBoard();
		}

		public void GenerateNewBoard()
		{
			GameTime = TimeSpan.FromSeconds( 0 );
			while ( GenerateSampleInternal( Level + 2 ) == 0 ) { };
		}

		public int GenerateSampleInternal( int steps )
		{
			Array.Clear( boardSteps, 0, boardSteps.Length );
			Array.Clear( boardSample, 0, boardSample.Length );
			Array.Clear( boardPlayer, 0, boardPlayer.Length );
			
			// Get random
			byte[] randomBytes = new byte[ steps ];
			RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
			rng.GetBytes( randomBytes );


			int row, column;
			for ( int cnt = 0; cnt < randomBytes.Length; cnt++ )
			{
				if ( randomBytes[ cnt ] > 34 )
				{					
//					randomBytes[ cnt ] = (byte)( randomBytes[ cnt ] - ( ( randomBytes[ cnt ] / 35 ) * 35 ));
					randomBytes[ cnt ] = (byte)( randomBytes[ cnt ] % 35 );
				}

//				column = randomBytes[ cnt ] - ( (int)( randomBytes[ cnt ] / 7 ) * 7 );
				column = randomBytes[ cnt ] % 7;
				row = (int)( randomBytes[ cnt ] / 7 );

				if ( boardSteps[ row, column ] == 0 )
				{
					boardSteps[ row, column ] = 1;
					StepToBoard( row, column, boardSample );
				}
			}

			int result = 0;
			for ( int row1 = 0; row1 < 5; row1++ )
			{
				for ( int col1 = 0; col1 < 7; col1++ )
				{
					result += boardSample[ row1, col1 ];
				}				
			}
			return result;
		}

		public void StepToBoard( int row, int column, byte[ , ] arrayToSet )
		{
			// top row
			if ( row > 0 )
			{
				// left column
				if ( column > 0 )
				{
					ChangeCellState( row - 1, column - 1, arrayToSet );
				}
				// middle column
				ChangeCellState( row - 1, column,  arrayToSet );
				// left column
				if ( column < 6 )
				{
					ChangeCellState( row - 1, column + 1, arrayToSet );
				}
			}
			
			// middle row
			// left column
			if ( column > 0 )
			{
				ChangeCellState( row, column - 1, arrayToSet );
			}
			
			//
			// middle column is not affected
			//

			// left column
			if ( column < 6 )
			{
				ChangeCellState( row, column + 1, arrayToSet );
			}
						
			// bottom row
			if ( row < 4 )
			{
				// left column
				if ( column > 0 )
				{
					ChangeCellState( row + 1, column - 1, arrayToSet );
				}
				// middle column
				ChangeCellState( row + 1, column, arrayToSet );
				// left column
				if ( column < 6 )
				{
					ChangeCellState( row + 1, column + 1, arrayToSet );
				}
			}
		}

		public void ChangeCellState( int row, int column, byte[ , ] arrayToSet )
		{
			if ( arrayToSet[ row, column ] == 0 )
			{
				arrayToSet[ row, column ] = 1;
			}
			else
			{
				arrayToSet[ row, column ] = 0;
			}
		}

		public bool IsWin()
		{
			bool result = true;
			for ( int row = 0; row < 5; row++ )
			{
				for ( int col = 0; col < 7; col++ )
				{
					if ( boardPlayer[ row, col ] != boardSample[ row, col ] )
					{
						result = false;
						break;
					}
				}				
			}
			return result;
		}

		

	}
}
