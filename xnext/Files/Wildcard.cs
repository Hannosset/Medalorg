using System.Globalization;

namespace xnext.Files
{
	public static class Wildcard
	{
		#region Wilcard comparison

		public static bool Match( string[] extensions , string fileName )
		{
			if( extensions is null )
				throw new System.ArgumentNullException( nameof( extensions ) );
			if( fileName is null )
				throw new System.ArgumentNullException( nameof( fileName ) );
			foreach( string aWc in extensions )
				if( Match( aWc , fileName ) )
					return true;
			return false;
		}

		/// <summary>
		/// Verify the wild card match
		/// </summary>
		/// <param name="extension"> wildcard </param>
		/// <param name="fileName">  string to match </param>
		/// <returns> true if match. By default the wc has a virtual * append to it </returns>
		public static bool Match( string extension , string fileName )
		{
			if( extension is null )
				throw new System.ArgumentNullException( nameof( extension ) );
			if( fileName is null )
				throw new System.ArgumentNullException( nameof( fileName ) );

			bool ans = false;
			string wc = extension.ToUpper( CultureInfo.CurrentCulture );
			string s = fileName.ToUpper( CultureInfo.CurrentCulture );

			int i = 0, j = 0;

			try
			{
				for( i = 0, j = 0 ; i < wc.Length && j < s.Length ; )
				{
					if( wc[i] == '*' )
					{
						if( ++i == wc.Length )
							return true;
						// Skip ** or *?, ...
						while( wc[i] == '*' || wc[i] == '?' )
							i += 1;
						if( wc[i] == '.' && s.Contains( "." ) )
							j = s.LastIndexOf( '.' );
						while( j < s.Length && s[j] != wc[i] )
							j += 1;
					}
					else if( s[j] != wc[i] )
						return false;
					i += 1;
					j += 1;
				}
				while( i < wc.Length && wc[i] == '*' )
					i += 1;
			}
			catch( System.Exception )
			{
			}
			finally
			{
				ans = i == wc.Length && j == s.Length;
			}
			return ans;
		}

		#endregion Wilcard comparison
	}
}
