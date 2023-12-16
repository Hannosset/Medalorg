using System;
using System.Runtime.InteropServices;
using System.Text;

namespace xnext.Native
{
	public static partial class NativeMethods
	{
		[System.Runtime.InteropServices.DllImport( "kernel32.dll" , SetLastError = true , EntryPoint = "GetStdHandle" )]
		private static extern IntPtr Win32GetStdHandle( int nStdHandle );

		public static IntPtr GetStandardHandle( int handle ) => Win32GetStdHandle( handle );

		/// <summary>
		/// Gets a string value of the specified key in the specified section of a Windows Initialization file.
		/// </summary>
		/// <param name="sectionName">   
		/// The name of the section containing the key name. If this parameter is NULL, the GetPrivateProfileString function copies all section names in the file to the supplied buffer.
		/// </param>
		/// <param name="keyName">       
		/// The name of the key whose associated string is to be retrieved. If this parameter is NULL, all key names in the section specified by the sectionName parameter are copied to
		/// the buffer specified by the ReturnedString parameter.
		/// </param>
		/// <param name="DefaultString"> 
		/// A default string. If the KeyName key cannot be found in the initialization file, GetPrivateProfileString copies the default string to the ReturnedString buffer. If this
		/// parameter is NULL, the default is an empty string, "".
		/// </param>
		/// <param name="ReturnedString"> A pointer to the buffer that receives the retrieved string. </param>
		/// <param name="Size">           The size of the buffer pointed to by the ReturnedString parameter, in characters. </param>
		/// <param name="FileName">       The full path to the initialization file. </param>
		/// <returns> </returns>
		[DllImport( "kernel32.dll" , CharSet = CharSet.Unicode , EntryPoint = "GetPrivateProfileString" )]
		private static extern int Win32GetPrivateProfileString( string sectionName , string keyName , string DefaultString , StringBuilder ReturnedString , int Size , string FileName );

		public static int GetPrivateProfileString( string sectionName , string keyName , string defaultString , StringBuilder returnedString , int size , string fileName )
			=> Win32GetPrivateProfileString( sectionName , keyName , defaultString , returnedString , size , fileName );

		/// <summary>
		/// Writes a string value of the specified key in the specified section of a Windows Initialization file.
		/// </summary>
		/// <param name="sectionName">
		/// The name of the section containing the key name. If this parameter is NULL, the GetPrivateProfileString function copies all section names in the file to the supplied buffer.
		/// </param>
		/// <param name="KeyName">    
		/// The name of the key whose associated string is to be retrieved. If this parameter is NULL, all key names in the section specified by the sectionName parameter are copied to
		/// the buffer specified by the ReturnedString parameter.
		/// </param>
		/// <param name="FileName">    The full path to the initialization file. </param>
		/// <param name="KeyValue">    A null-terminated string to be written to the file. If this parameter is NULL, the key pointed to by the KeyName parameter is deleted. </param>
		[DllImport( "kernel32.dll" , CharSet = CharSet.Unicode , EntryPoint = "WritePrivateProfileString" )]
		[return: MarshalAs( UnmanagedType.Bool )]
		private static extern bool Win32WritePrivateProfileString( string sectionName , string KeyName , string KeyValue , string FileName );

		public static bool WritePrivateProfileString( string sectionName , string keyName , string keyValue , string fileName )
			=> Win32WritePrivateProfileString( sectionName , keyName , keyValue , fileName );

		[DllImport( "kernel32.dll" , CallingConvention = CallingConvention.Cdecl )]
		private static extern int RtlCompareMemory( byte[] b1 , byte[] b2 , long count );
		public static bool CompareMemory( byte[] b1 , byte[] b2 , long count ) => RtlCompareMemory( b1 , b2 , count ) == count;
		public static bool CompareMemory( byte[] b1 , byte[] b2 )
		{
			if( b1 is null )
				throw new ArgumentNullException( nameof( b1 ) );
			if( b2 is null )
				throw new ArgumentNullException( nameof( b2 ) );
			return b1.Length == b2.Length && RtlCompareMemory( b1 , b2 , b1.Length ) == b1.Length;
		}
	}
}
