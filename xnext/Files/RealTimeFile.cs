namespace xnext.Files
{
	public class RealTimeFile : BufferedFile
	{
		#region CONSTRUCTOR
		public RealTimeFile() : base()
		{
		}
		#endregion CONSTRUCTOR

		public override void WriteLine( string data )
		{
			base.WriteLine( data );
			Flush();
		}
	}
}
