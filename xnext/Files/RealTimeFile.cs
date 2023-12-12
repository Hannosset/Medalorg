namespace xnet.Files
{
	public class RealTimeFile : BufferedFile
	{
		#region CONSTRUCTOR

		/// <summary>
		/// Initializes a new instance of the <see cref="RealTimeFile"/> class.
		/// </summary>
		public RealTimeFile() : base()
		{
		}

		#endregion CONSTRUCTOR

		/// <summary>
		/// Writes the line.
		/// </summary>
		/// <param name="data"> The data. </param>
		public override void WriteLine( string data )
		{
			base.WriteLine( data );
			Flush();
		}
	}
}
