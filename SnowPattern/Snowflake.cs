using System.Windows;
using System.Windows.Controls;

namespace SnowPattern
{
	public enum SnowFlakeState
	{
		None,
		Set,
		HintSet,
	}

	class Snowflake : Button
	{
		private SnowFlakeState prev = SnowFlakeState.None;

		public Snowflake()
        {
			DefaultStyleKey = typeof( Snowflake );
        }

        /// <summary>
        /// Updates the visual state of the space based on the initial binding values.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            UpdateSpaceState(false);
        }

		public SnowFlakeState FlakeState
		{
			get
			{
				return (SnowFlakeState)GetValue( FlakeStateProperty );
			}
			set
			{
				prev = (SnowFlakeState)GetValue( FlakeStateProperty );
				SetValue( FlakeStateProperty, value );
			}
		}

		public static readonly DependencyProperty FlakeStateProperty = DependencyProperty.Register( "SnowFlakeState", typeof( SnowFlakeState ), typeof( Button ), new PropertyMetadata( SnowFlakeState.None, SpaceStateChanged ) );

		private static void SpaceStateChanged( DependencyObject d,	DependencyPropertyChangedEventArgs e )
		{
			( d as Snowflake ).UpdateSpaceState( true );
		}

		/// <summary>
		/// Updates the visual state of the space, optionally using animated transitions.
		/// </summary>
		/// <param name="useTransitions">true to use transitions; otherwise, false.</param>
		private void UpdateSpaceState( bool useTransitions )
		{
			if ( FlakeState == SnowFlakeState.None )
			{
				VisualStateManager.GoToState( this, "None", useTransitions );
			}
			else if ( FlakeState == SnowFlakeState.Set)
			{
				VisualStateManager.GoToState( this, "FlakeSet", useTransitions );
			}
			else if ( FlakeState == SnowFlakeState.HintSet )
			{
				if ( prev == SnowFlakeState.Set )
				{
					VisualStateManager.GoToState( this, "HintSet", useTransitions );
				}
				else
				{
					VisualStateManager.GoToState( this, "HintNone", useTransitions );
				}
			}
		}
	}
}
