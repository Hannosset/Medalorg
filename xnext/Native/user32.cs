using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Runtime.InteropServices;

namespace xnet.Native
{
	/// <summary>
	/// Class User32.
	/// Cfr: http://www.pinvoke.net/index.aspx
	/// </summary>
	public static partial class NativeMethods
	{
		#region TYPES

		[StructLayout( LayoutKind.Sequential )]
		internal struct POINT : IEquatable<POINT>
		{
			/// <summary>
			/// Gets or sets the x.
			/// </summary>
			/// <value> The x. </value>
			[SuppressMessage( "Microsoft.Performance" , "CA1811:AvoidUncalledPrivateCode" , Justification = "Native call - do not jam" )]
			public int X { get; set; }
			/// <summary>
			/// Gets or sets the y.
			/// </summary>
			/// <value> The y. </value>
			[SuppressMessage( "Microsoft.Performance" , "CA1811:AvoidUncalledPrivateCode" , Justification = "Native call - do not jam" )]
			public int Y { get; set; }

			#region CONSTRUCTOR
			/// <summary>
			/// Initializes a new instance of the <see cref="POINT"/> struct.
			/// </summary>
			/// <param name="abcissa">  The x. </param>
			/// <param name="ordinate"> The y. </param>
			[SuppressMessage( "Microsoft.Performance" , "CA1811:AvoidUncalledPrivateCode" , Justification = "Native call - do not jam" )]
			internal POINT( int abcissa , int ordinate )
			{
				X = abcissa;
				Y = ordinate;
			}

			#endregion CONSTRUCTOR

			public override int GetHashCode()
			{
				return X ^ Y;
			}

			public override bool Equals( object obj )
			{
				if( !(obj is POINT) )
					return false;

				return Equals( (POINT)obj );
			}

			public bool Equals( POINT other )
			{
				if( X != other.X )
					return false;

				return Y == other.Y;
			}

			public static bool operator ==( POINT point1 , POINT point2 )
			{
				return point1.Equals( point2 );
			}

			public static bool operator !=( POINT point1 , POINT point2 )
			{
				return !point1.Equals( point2 );
			}
		}

		#endregion TYPES

		/// <summary>
		/// What: Gets the cursor position.
		/// Why: Get the cursor position from the desktop
		/// </summary>
		/// <param name="lpPoint"> The lp point. </param>
		/// <returns> <c> true </c> if XXXX, <c> false </c> otherwise. </returns>
		[DllImport( "user32.dll" , SetLastError = true , EntryPoint = "GetCursorPos" )]
		[return: MarshalAs( UnmanagedType.Bool )]
		private static extern bool Win32GetCursorPos( out POINT lpPoint );

		public static Tuple<int , int> GetCursorPosition()
		{
			if( Win32GetCursorPos( out POINT point ) )
				return new Tuple<int , int>( point.X , point.Y );
			return new Tuple<int , int>( 0 , 0 );
		}

		/// <summary>
		/// The GetDesktopWindow function returns a handle to the desktop window. The desktop window covers the entire screen. The desktop window is the area on top of which other
		/// windows are painted.
		/// </summary>
		/// <returns> IntPtr. </returns>
		[DllImport( "user32.dll" , CharSet = CharSet.Unicode , ExactSpelling = true , EntryPoint = "GetDesktopWindow" )]
		private static extern IntPtr Win32GetDesktopWindow();

		public static IntPtr GetDesktopWindow() => Win32GetDesktopWindow();

		[DllImport( "user32.dll" , SetLastError = true , ExactSpelling = true , EntryPoint = "BringWindowToTop" )]
		private static extern bool Win32BringWindowToTop( IntPtr hWnd );

		public static bool BringWindowToTop( IntPtr windowHandle ) => Win32BringWindowToTop( windowHandle );

		[DllImport( "User32.dll" , EntryPoint = "SetForegroundWindow" )]
		private static extern int Win32SetForegroundWindow( int hWnd );

		public static bool SetForegroundWindow( IntPtr windowHandle ) => Win32SetForegroundWindow( windowHandle.ToInt32() ) != 0;

		public static bool SetForegroundWindow()
		{
			foreach( Process process in Process.GetProcessesByName( Process.GetCurrentProcess().ProcessName ) )
				if( process.Id == Process.GetCurrentProcess().Id )
					return SetForegroundWindow( process.MainWindowHandle );
			return false;
		}

		public static void ShowToFront( IntPtr window )
		{
			ShowWindow( window , SW_SHOWNORMAL );
			SetForegroundWindow( window );
		}

		[DllImport( "user32" , CharSet = CharSet.Unicode , EntryPoint = "RegisterWindowMessage" )]
		private static extern int Win32RegisterWindowMessage( string message );

		public static int RegisterWindowMessage( string message ) => Win32RegisterWindowMessage( message );

		public static int RegisterWindowMessage( string format , params object[] args )
		{
			string message = string.Format( CultureInfo.CurrentCulture , format , args );
			return Win32RegisterWindowMessage( message );
		}

		public const int HWND_BROADCAST = 0xffff;
		public const int SW_SHOWNORMAL = 1;
		[DllImport( "user32" , EntryPoint = "PostMessage" )]
		private static extern bool Win32PostMessage( IntPtr hwnd , int msg , IntPtr wparam , IntPtr lparam );

		public static bool PostMessage( IntPtr handle , int messageId , IntPtr wordParameter , IntPtr longParameter ) => Win32PostMessage( handle , messageId , wordParameter , longParameter );

		[DllImportAttribute( "user32.dll" , EntryPoint = "ShowWindow" )]
		private static extern bool Win32ShowWindow( IntPtr hWnd , int nCmdShow );

		public static bool ShowWindow( IntPtr handle , int showType ) => Win32ShowWindow( handle , showType );
	}
}
