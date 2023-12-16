using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace xnext.Diagnostics
{
	/// <summary>Class RecordTrace. This class cannot be inherited.</summary>
	/// <remarks>
	/// The format of the record is as follows:
	/// <code>
	///0			1				2					3				4			5		6			7			8			9			10
	///UTC Date, UTC Time (HH:mm),local Time (HH:mm), TraceEventType, Context, Message, Consequence, Reaction, Module(:line) ,function, details
	/// </code>
	/// </remarks>
	public sealed class RecordIssue
	{
		#region PUBLIC PROPERTIES

		/// <summary>
		/// The consequence are an explanation of the behavioral consequence of such a critical error
		/// </summary>
		private Stack<string> Consequence = new Stack<string>();

		/// <summary>
		/// The reaction are the alternatives provided to the support team to resolve the issue
		/// </summary>
		private Stack<string> Reaction = new Stack<string>();

		/// <summary>
		/// Default Context description
		/// </summary>
		private Stack<string> Context = new Stack<string>();
		#endregion PUBLIC PROPERTIES

		#region PUBLIC METHODS
		/// <summary>
		/// Configure the default context, consequence and reaction for any incoming record step
		/// </summary>
		/// <param name="context"> </param>
		/// <param name="consequence"> </param>
		/// <param name="reaction"> </param>
		public void PushEnv( string context , string consequence , string reaction )
		{
			if( !string.IsNullOrEmpty( context ) )
				Context.Push( context );
			else
				Context.Push( Context.Peek() );

			if( !string.IsNullOrEmpty( consequence ) )
				Consequence.Push( consequence );
			else
				Consequence.Push( Consequence.Peek() );

			if( !string.IsNullOrEmpty( reaction ) )
				Reaction.Push( reaction );
			else
				Reaction.Push( Reaction.Peek() );
		}
		/// <summary>
		/// Pops the env.
		/// </summary>
		public void PopEnv()
		{
			Context.Pop();
			Consequence.Pop();
			Reaction.Pop();
		}
		/// <summary>
		/// This specific function is used to call when the consequence and the reaction are recorded.
		/// </summary>
		/// <param name="message"> </param>
		/// <param name="details"> </param>
		/// <returns> </returns>
		public string[] Step( string message , string details ) => Step( TraceEventType.Information , null , message , null , null , details );
		/// <summary>
		/// Steps the specified message.
		/// </summary>
		/// <param name="message"> The message. </param>
		/// <param name="consequence"> The consequence. </param>
		/// <param name="reaction"> The reaction. </param>
		/// <param name="details"> The details. </param>
		/// <returns> System.String[]. </returns>
		public string[] Step( string message , string consequence , string reaction , string details ) => Step( TraceEventType.Information , null , message , consequence , reaction , details );
		/// <summary>
		/// This specific function is used to call when the consequence and the reaction are recorded.
		/// </summary>
		/// <param name="type"> The type. </param>
		/// <param name="message"> The message. </param>
		/// <param name="details"> The details. </param>
		/// <returns> System.String[]. </returns>
		public string[] Step( TraceEventType type , string message , string details ) => Step( type , null , message , null , null , details );
		/// <summary>
		/// Steps the specified message.
		/// </summary>
		/// <param name="type"> The type. </param>
		/// <param name="message"> The message. </param>
		/// <param name="consequence"> The consequence. </param>
		/// <param name="reaction"> The reaction. </param>
		/// <param name="details"> The details. </param>
		/// <returns> System.String[]. </returns>
		public string[] Step( TraceEventType type , string message , string consequence , string reaction , string details ) => Step( type , null , message , consequence , reaction , details );
		/// <summary>
		/// Steps the specified ex.
		/// </summary>
		/// <param name="ex"> The ex. </param>
		/// <param name="consequence"> The consequence. </param>
		/// <param name="reaction"> The reaction. </param>
		/// <returns> System.String[]. </returns>
		/// <exception cref="ArgumentNullException"> ex </exception>
		public string[] Step( Exception ex , string consequence , string reaction )
		{
			if( ex is null )
				throw new ArgumentNullException( nameof( ex ) );
			return Step( TraceEventType.Critical , ex.Source , ex.Message , consequence , reaction , string.Empty );
		}
		/// <summary>
		/// Steps the specified type.
		/// </summary>
		/// <param name="type"> The type. </param>
		/// <param name="context"> The context. </param>
		/// <param name="message"> The message. </param>
		/// <param name="consequence"> The consequence. </param>
		/// <param name="reaction"> The reaction. </param>
		/// <param name="details"> The details. </param>
		/// <returns> System.String. </returns>
		public string[] Step( TraceEventType type , string context , string message , string consequence , string reaction , string details )
		{
			List<string> lst = new List<string>
			{
				$"{DateTime.Now:yyyy-MM-dd}",
				$"{DateTime.UtcNow:HH:mm}",
				$"{DateTime.Now:HH:mm}",
				type.ToString(),
				context is null ? "": (Context.Count > 0 ? Context.Peek() : string.Empty),
				message ,
				consequence is null ? (Consequence.Count > 0 ? Consequence.Peek() : string.Empty) : consequence,
				reaction is null ?   (Reaction.Count > 0 ? Reaction.Peek(): string.Empty): reaction
				,""
				, ""
				,details?.Replace( "\n" , "-/-" )
			};
			StackTrace stackTrace = new StackTrace( 1 , true );

			for( int i = 0 ; i < stackTrace.FrameCount ; i++ )
				if( !stackTrace.GetFrame( i ).GetMethod().ReflectedType.FullName.Contains( "Diagnostics" )
					&& !stackTrace.GetFrame( i ).GetMethod().ReflectedType.FullName.Contains( "LogTrace" ) )
				{
					lst[8] = $"{stackTrace.GetFrame( i ).GetFileName()}:{stackTrace.GetFrame( i ).GetFileLineNumber()}";
					lst[9] = stackTrace.GetFrame( i ).GetMethod().ReflectedType.FullName;
					if( string.IsNullOrEmpty( lst[4] ) && (i + 1) < stackTrace.FrameCount )
						if( string.IsNullOrEmpty( lst[4] ) )
							lst[4] = $"{stackTrace.GetFrame( i + 1 ).GetMethod().Name}";
						else
							lst[4] = $"{lst[4]} - {stackTrace.GetFrame( i + 1 ).GetMethod().Name}";
					break;
				}
			return lst.ToArray();
		}

		#endregion PUBLIC METHODS
	}
}