using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;

namespace xnet.Files
{
	public class BufferedFile : IDisposable
	{
		#region LOCAL VARIABLES

		private static readonly object MutexFlag = new object();
		private FileStream DataFile;
		private BufferedStream DataFileBuffer;
		private Thread thFlush;
		#endregion LOCAL VARIABLES

		#region PUBLIC PROPERTIES
		public bool CanWrite => !disposedValue && DataFile != null && DataFile.CanWrite;
		public bool CanRead => !disposedValue && DataFile != null && DataFile.CanRead;
		public string FileName { get; private set; }
		#endregion PUBLIC PROPERTIES

		#region CONSTRUCTOR
		/// <summary>
		/// Initializes a new instance of the <see cref="BufferedFile"/> class.
		/// </summary>
		public BufferedFile()
		{
		}
		public BufferedFile( string fileName ) => Open( fileName );
		public BufferedFile( string fileName , int bufferSize ) => Open( fileName , bufferSize );
		#endregion CONSTRUCTOR

		/// <summary>
		/// Opens the specified filename.
		/// </summary>
		/// <param name="fileName"> The filename. </param>
		public BufferedFile Open( string fileName ) => Open( fileName , 4096 );
		public BufferedFile Open( string fileName , int bufferSize )
		{
			if( fileName is null )
				throw new ArgumentNullException( nameof( fileName ) );

			Close();

			FileName = fileName;

			if( !new FileInfo( fileName ).Directory.Exists )
				Directory.CreateDirectory( new FileInfo( fileName ).DirectoryName );

			lock( MutexFlag )
			{
#if DEBUG
				DataFile = new FileStream( fileName , FileMode.Append , FileAccess.Write , FileShare.ReadWrite );
#else
				DataFile = new FileStream( fileName , FileMode.Append , FileAccess.Write , FileShare.ReadWrite );
#endif
				DataFileBuffer = new BufferedStream( DataFile , bufferSize );
			}

			thFlush = new Thread( new ThreadStart( _Flush ) ) { IsBackground = true , Priority = ThreadPriority.Lowest , Name = "BufferedFile.Flush" };
			thFlush.Start();

			return this;
		}
		/// <summary>
		/// Flushes this instance.
		/// </summary>
		public void Flush() => Flush( 0 );
		/// <summary>
		/// Flushes this instance.
		/// </summary>
		/// <param name="sleep"> The sleep. </param>
		public void Flush( int sleep )
		{
			try
			{
				if( DataFileBuffer != null )
					DataFileBuffer?.Flush();

				if( DataFile != null )
					DataFile?.Flush();
			}
			catch( System.Exception )
			{
			}
			finally
			{
				if( sleep > 0 && CanWrite && DataFileBuffer != null )
					for( int i = 0 ; !disposedValue && CanWrite && DataFileBuffer != null && i < sleep / 100 ; i++ )
						Thread.Sleep( 100 );
			}
		}
		private void _Flush()
		{
			while( !disposedValue && DataFileBuffer != null && DataFile != null )
				if( CanWrite && DataFileBuffer != null )
					try
					{
						if( DataFileBuffer != null )
							DataFileBuffer?.Flush();

						if( DataFile != null )
							DataFile?.Flush();
					}
					catch( System.Exception )
					{
					}
					finally
					{
						if( CanWrite && DataFileBuffer != null )
							for( int i = 0 ; !disposedValue && CanWrite && DataFileBuffer != null && i < 60 ; i++ )
								Thread.Sleep( 1000 );
					}
		}
		/// <summary>
		/// Writes the line.
		/// </summary>
		/// <param name="data"> The data. </param>
		/// <exception cref="ArgumentNullException"> data </exception>
		public virtual void WriteLine( string data )
		{
			if( data is null )
				throw new ArgumentNullException( nameof( data ) );

			lock( MutexFlag )
			{
				if( DataFileBuffer != null )
				{
					try
					{
						if( data.IndexOf( '>' ) > -1 && (data.Substring( 0 , data.IndexOf( '>' ) ).Contains( DateTime.UtcNow.ToString( "HH:mm" , CultureInfo.CurrentCulture ) ) || data.Substring( 0 , data.IndexOf( '>' ) ).Contains( DateTime.Now.ToString( "HH:mm" , CultureInfo.CurrentCulture ) )) )
						{
							byte[] str = Encoding.ASCII.GetBytes( $"{data}\n" );
							DataFileBuffer.Write( str , 0 , str.Length );
						}
						else
						{
							byte[] str = Encoding.ASCII.GetBytes( $"{DateTime.UtcNow:HH:mm:ss.fff},{data}\n" );
							DataFileBuffer.Write( str , 0 , str.Length );
						}
					}
					catch { }
				}
			}
		}
		/// <summary>
		/// Writes the line.
		/// </summary>
		/// <param name="data"> The data. </param>
		public virtual void DumpLine( string data )
		{
			if( data is null )
				throw new ArgumentNullException( nameof( data ) );

			lock( MutexFlag )
			{
				if( DataFileBuffer != null )
				{
					byte[] str = Encoding.ASCII.GetBytes( $"{data}\n" );
					DataFileBuffer.Write( str , 0 , str.Length );
				}
			}
		}

		public virtual void Write( byte[] buffer )
		{
			if( buffer is null )
				throw new ArgumentNullException( nameof( buffer ) );

			lock( MutexFlag )
				if( DataFileBuffer != null )
					DataFileBuffer.Write( buffer , 0 , buffer.Length );
		}

		public void WriteByte( byte value )
		{
			lock( MutexFlag )
				if( DataFileBuffer != null )
					DataFileBuffer.WriteByte( value );
		}

		/// <summary>
		/// Closes this instance.
		/// </summary>
		/// <returns> BufferedFile. </returns>
		public void Close()
		{
			lock( MutexFlag )
			{
				Flush();

				try
				{
					if( DataFileBuffer != null )
						DataFileBuffer.Close();
				}
				catch( System.Exception )
				{
				}
				finally
				{
					DataFileBuffer = null;
				}
				try
				{
					if( DataFile != null )
						DataFile.Close();
				}
				catch( System.Exception )
				{
				}
				finally
				{
					DataFile = null;
				}
			}
		}

		#region IDisposable Support

		private bool disposedValue; // To detect redundant calls

		protected virtual void Dispose( bool disposing )
		{
			if( !disposedValue )
			{
				if( disposing )
				{
					disposedValue = true;
					Close();
					// TODO: dispose managed state (managed objects).
				}

				// TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
				// TODO: set large fields to null.
			}
		}

		// TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources. ~BufferedFile() { // Do not change this code. Put cleanup code in
		// Dispose(bool disposing) above. Dispose(false); }

		// This code added to correctly implement the disposable pattern.
		public void Dispose()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose( true );
			// TODO: uncomment the following line if the finalizer is overridden above.
			GC.SuppressFinalize( this );
		}

		#endregion IDisposable Support
	}
}