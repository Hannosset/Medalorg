using mui.Context.Protocol;

using System;
using System.IO;
using System.Runtime.Caching;
using System.Xml;
using System.Xml.Serialization;

using xnext.Diagnostics;

namespace mui.Context
{
	/// <summary>
	/// What: List of the country code
	///  Why: Need the country code mnemonic to download the subtitle
	/// </summary>
	internal sealed class HandleCountryCode
	{
		#region ACCESSORS
		/// <summary>
		/// What: List of the available data of the singleton
		///  Why: Access the country code mnemonic and the country name for the end user to chose
		/// </summary>
		internal CountryCode[] Details { get; private set; }
		#endregion ACCESSORS

		#region SINGLETON
		/// <summary>
		/// What: Singleton instance of the object
		///  Why: Allow all application's methods to access the singleton
		/// </summary>
		internal static HandleCountryCode Info { get; private set; } = new HandleCountryCode();
		#endregion SINGLETON

		#region PUBLIC METHODS
		/// <summary>
		/// What: Load the country code from the xml file and initialize the country code if not existing.
		///  Why: Allow to specifically populate the singleton instance when initializing the application.
		/// </summary>
		internal static void LoadFromFile()
		{
			if( !File.Exists( Filename ) )
			{
				LogTrace.Label();
				Info.Details = new CountryCode[]
				{
					new CountryCode { Code = "en" , Label = "English" } ,
					new CountryCode { Code = "ru" , Label = "Russian" } ,
					new CountryCode { Code = "ch" , Label = "Chinese" } ,
					new CountryCode { Code = "fa" , Label = "Persian" } ,
					new CountryCode { Code = "be" , Label = "Belarusian" } ,
					new CountryCode { Code = "ar" , Label = "Arabic" } ,
					new CountryCode { Code = "ka" , Label = "Kazakh" } ,
					new CountryCode { Code = "ug" , Label = "Uighur; Uyghur" } ,
					new CountryCode { Code = "tu" , Label = "Turkish" } ,
					new CountryCode { Code = "th" , Label = "Thai" } ,

					new CountryCode { Code = "aa" , Label = "Afar" } ,
					new CountryCode { Code = "ab" , Label = "Abkhazian" } ,
					new CountryCode { Code = "ae" , Label = "Avestan" } ,
					new CountryCode { Code = "af" , Label = "Afrikaans" } ,
					new CountryCode { Code = "ak" , Label = "Akan" } ,
					new CountryCode { Code = "am" , Label = "Amharic" } ,
					new CountryCode { Code = "an" , Label = "Aragonese" } ,
					new CountryCode { Code = "as" , Label = "Assamese" } ,
					new CountryCode { Code = "av" , Label = "Avaric" } ,
					new CountryCode { Code = "ay" , Label = "Aymara" } ,
					new CountryCode { Code = "az" , Label = "Azerbaijani" } ,
					new CountryCode { Code = "ba" , Label = "Bashkir" } ,
					new CountryCode { Code = "bg" , Label = "Bulgarian" } ,
					new CountryCode { Code = "bh" , Label = "Bihari languages" } ,
					new CountryCode { Code = "bi" , Label = "Bislama" } ,
					new CountryCode { Code = "bm" , Label = "Bambara" } ,
					new CountryCode { Code = "bn" , Label = "Bengali" } ,
					new CountryCode { Code = "bo" , Label = "Tibetan" } ,
					new CountryCode { Code = "br" , Label = "Breton" } ,
					new CountryCode { Code = "bs" , Label = "Bosnian" } ,
					new CountryCode { Code = "ca" , Label = "Catalan; Valencian" } ,
					new CountryCode { Code = "ce" , Label = "Chechen" } ,
					new CountryCode { Code = "ch" , Label = "Chamorro" } ,
					new CountryCode { Code = "co" , Label = "Corsican" } ,
					new CountryCode { Code = "cr" , Label = "Cree" } ,
					new CountryCode { Code = "cs" , Label = "Czech" } ,
					new CountryCode { Code = "cu" , Label = "Church Slavic" } ,
					new CountryCode { Code = "cv" , Label = "Chuvash" } ,
					new CountryCode { Code = "cy" , Label = "Welsh" } ,
					new CountryCode { Code = "da" , Label = "Danish" } ,
					new CountryCode { Code = "de" , Label = "German" } ,
					new CountryCode { Code = "dv" , Label = "Divehi; Dhivehi; Maldivian" } ,
					new CountryCode { Code = "dz" , Label = "Dzongkha" } ,
					new CountryCode { Code = "ee" , Label = "Ewe" } ,
					new CountryCode { Code = "el" , Label = "Greek, Modern (1453-)" } ,
					new CountryCode { Code = "eo" , Label = "Esperanto" } ,
					new CountryCode { Code = "es" , Label = "Spanish; Castilian" } ,
					new CountryCode { Code = "et" , Label = "Estonian" } ,
					new CountryCode { Code = "eu" , Label = "Basque" } ,
					new CountryCode { Code = "eu" , Label = "Basque" } ,
					new CountryCode { Code = "ff" , Label = "Fulah" } ,
					new CountryCode { Code = "fi" , Label = "Finnish" } ,
					new CountryCode { Code = "fj" , Label = "Fijian" } ,
					new CountryCode { Code = "fo" , Label = "Faroese" } ,
					new CountryCode { Code = "fr" , Label = "French" } ,
					new CountryCode { Code = "fy" , Label = "Western Frisian" } ,
					new CountryCode { Code = "ga" , Label = "Irish" } ,
					new CountryCode { Code = "gd" , Label = "Gaelic; Scottish Gaelic" } ,
					new CountryCode { Code = "gl" , Label = "Galician" } ,
					new CountryCode { Code = "gn" , Label = "Guarani" } ,
					new CountryCode { Code = "gu" , Label = "Gujarati" } ,
					new CountryCode { Code = "gv" , Label = "Manx" } ,
					new CountryCode { Code = "ha" , Label = "Hausa" } ,
					new CountryCode { Code = "he" , Label = "Hebrew" } ,
					new CountryCode { Code = "hi" , Label = "Hindi" } ,
					new CountryCode { Code = "ho" , Label = "Hiri Motu" } ,
					new CountryCode { Code = "hr" , Label = "Croatian" } ,
					new CountryCode { Code = "ht" , Label = "Haitian; Haitian Creole" } ,
					new CountryCode { Code = "hu" , Label = "Hungarian" } ,
					new CountryCode { Code = "hy" , Label = "Armenian" } ,
					new CountryCode { Code = "hy" , Label = "Armenian" } ,
					new CountryCode { Code = "hz" , Label = "Herero" } ,
					new CountryCode { Code = "ia" , Label = "Interlingua" } ,
					new CountryCode { Code = "id" , Label = "Indonesian" } ,
					new CountryCode { Code = "ie" , Label = "Interlingue; Occidental" } ,
					new CountryCode { Code = "ig" , Label = "Igbo" } ,
					new CountryCode { Code = "ii" , Label = "Sichuan Yi; Nuosu" } ,
					new CountryCode { Code = "ik" , Label = "Inupiaq" } ,
					new CountryCode { Code = "io" , Label = "Ido" } ,
					new CountryCode { Code = "is" , Label = "Icelandic" } ,
					new CountryCode { Code = "it" , Label = "Italian" } ,
					new CountryCode { Code = "iu" , Label = "Inuktitut" } ,
					new CountryCode { Code = "ja" , Label = "Japanese" } ,
					new CountryCode { Code = "jv" , Label = "Javanese" } ,
					new CountryCode { Code = "ka" , Label = "Georgian" } ,
					new CountryCode { Code = "ka" , Label = "Georgian" } ,
					new CountryCode { Code = "kg" , Label = "Kongo" } ,
					new CountryCode { Code = "ki" , Label = "Kikuyu; Gikuyu" } ,
					new CountryCode { Code = "kl" , Label = "Kalaallisut; Greenlandic" } ,
					new CountryCode { Code = "km" , Label = "Central Khmer" } ,
					new CountryCode { Code = "kn" , Label = "Kannada" } ,
					new CountryCode { Code = "ko" , Label = "Korean" } ,
					new CountryCode { Code = "kr" , Label = "Kanuri" } ,
					new CountryCode { Code = "ks" , Label = "Kashmiri" } ,
					new CountryCode { Code = "ku" , Label = "Kurdish" } ,
					new CountryCode { Code = "kv" , Label = "Komi" } ,
					new CountryCode { Code = "kw" , Label = "Cornish" } ,
					new CountryCode { Code = "ky" , Label = "Kirghiz; Kyrgyz" } ,
					new CountryCode { Code = "la" , Label = "Latin" } ,
					new CountryCode { Code = "lb" , Label = "Luxembourgish" } ,
					new CountryCode { Code = "lg" , Label = "Ganda" } ,
					new CountryCode { Code = "li" , Label = "Limburger" } ,
					new CountryCode { Code = "ln" , Label = "Lingala" } ,
					new CountryCode { Code = "lo" , Label = "Lao" } ,
					new CountryCode { Code = "lt" , Label = "Lithuanian" } ,
					new CountryCode { Code = "lu" , Label = "Luba-Katanga" } ,
					new CountryCode { Code = "lv" , Label = "Latvian" } ,
					new CountryCode { Code = "mg" , Label = "Malagasy" } ,
					new CountryCode { Code = "mh" , Label = "Marshallese" } ,
					new CountryCode { Code = "mi" , Label = "Maori" } ,
					new CountryCode { Code = "mk" , Label = "Macedonian" } ,
					new CountryCode { Code = "ml" , Label = "Malayalam" } ,
					new CountryCode { Code = "mn" , Label = "Mongolian" } ,
					new CountryCode { Code = "mr" , Label = "Marathi" } ,
					new CountryCode { Code = "ms" , Label = "Malay" } ,
					new CountryCode { Code = "mt" , Label = "Maltese" } ,
					new CountryCode { Code = "my" , Label = "Burmese" } ,
					new CountryCode { Code = "na" , Label = "Nauru" } ,
					new CountryCode { Code = "nb" , Label = "Bokmål, Norwegian" } ,
					new CountryCode { Code = "nd" , Label = "Ndebele, North" } ,
					new CountryCode { Code = "ne" , Label = "Nepali" } ,
					new CountryCode { Code = "ng" , Label = "Ndonga" } ,
					new CountryCode { Code = "nl" , Label = "Dutch; Flemish" } ,
					new CountryCode { Code = "nn" , Label = "Norwegian Nynorsk;" } ,
					new CountryCode { Code = "no" , Label = "Norwegian" } ,
					new CountryCode { Code = "nr" , Label = "Ndebele, South" } ,
					new CountryCode { Code = "nv" , Label = "Navajo; Navaho" } ,
					new CountryCode { Code = "ny" , Label = "Chichewa; Chewa; Nyanja" } ,
					new CountryCode { Code = "oc" , Label = "Occitan (post 1500)" } ,
					new CountryCode { Code = "oj" , Label = "Ojibwa" } ,
					new CountryCode { Code = "om" , Label = "Oromo" } ,
					new CountryCode { Code = "or" , Label = "Oriya" } ,
					new CountryCode { Code = "os" , Label = "Ossetian; Ossetic" } ,
					new CountryCode { Code = "pa" , Label = "Panjabi; Punjabi" } ,
					new CountryCode { Code = "pi" , Label = "Pali" } ,
					new CountryCode { Code = "pl" , Label = "Polish" } ,
					new CountryCode { Code = "ps" , Label = "Pushto; Pashto" } ,
					new CountryCode { Code = "pt" , Label = "Portuguese" } ,
					new CountryCode { Code = "qu" , Label = "Quechua" } ,
					new CountryCode { Code = "rm" , Label = "Romansh" } ,
					new CountryCode { Code = "rn" , Label = "Rundi" } ,
					new CountryCode { Code = "ro" , Label = "Romanian; Moldovan" } ,
					new CountryCode { Code = "rw" , Label = "Kinyarwanda" } ,
					new CountryCode { Code = "sa" , Label = "Sanskrit" } ,
					new CountryCode { Code = "sc" , Label = "Sardinian" } ,
					new CountryCode { Code = "sd" , Label = "Sindhi" } ,
					new CountryCode { Code = "se" , Label = "Northern Sami" } ,
					new CountryCode { Code = "sg" , Label = "Sango" } ,
					new CountryCode { Code = "si" , Label = "Sinhala; Sinhalese" } ,
					new CountryCode { Code = "sk" , Label = "Slovak" } ,
					new CountryCode { Code = "sl" , Label = "Slovenian" } ,
					new CountryCode { Code = "sm" , Label = "Samoan" } ,
					new CountryCode { Code = "sn" , Label = "Shona" } ,
					new CountryCode { Code = "so" , Label = "Somali" } ,
					new CountryCode { Code = "sq" , Label = "Albanian" } ,
					new CountryCode { Code = "sr" , Label = "Serbian" } ,
					new CountryCode { Code = "ss" , Label = "Swati" } ,
					new CountryCode { Code = "st" , Label = "Sotho, Southern" } ,
					new CountryCode { Code = "su" , Label = "Sundanese" } ,
					new CountryCode { Code = "sv" , Label = "Swedish" } ,
					new CountryCode { Code = "sw" , Label = "Swahili" } ,
					new CountryCode { Code = "ta" , Label = "Tamil" } ,
					new CountryCode { Code = "te" , Label = "Telugu" } ,
					new CountryCode { Code = "tg" , Label = "Tajik" } ,
					new CountryCode { Code = "ti" , Label = "Tigrinya" } ,
					new CountryCode { Code = "tk" , Label = "Turkmen" } ,
					new CountryCode { Code = "tl" , Label = "Tagalog" } ,
					new CountryCode { Code = "tn" , Label = "Tswana" } ,
					new CountryCode { Code = "to" , Label = "Tonga (Tonga Islands)" } ,
					new CountryCode { Code = "ts" , Label = "Tsonga" } ,
					new CountryCode { Code = "tt" , Label = "Tatar" } ,
					new CountryCode { Code = "tw" , Label = "Twi" } ,
					new CountryCode { Code = "ty" , Label = "Tahitian" } ,
					new CountryCode { Code = "uk" , Label = "Ukrainian" } ,
					new CountryCode { Code = "ur" , Label = "Urdu" } ,
					new CountryCode { Code = "uz" , Label = "Uzbek" } ,
					new CountryCode { Code = "ve" , Label = "Venda" } ,
					new CountryCode { Code = "vi" , Label = "Vietnamese" } ,
					new CountryCode { Code = "vo" , Label = "Volapük" } ,
					new CountryCode { Code = "wa" , Label = "Walloon" } ,
					new CountryCode { Code = "wo" , Label = "Wolof" } ,
					new CountryCode { Code = "xh" , Label = "Xhosa" } ,
					new CountryCode { Code = "yi" , Label = "Yiddish" } ,
					new CountryCode { Code = "yo" , Label = "Yoruba" } ,
					new CountryCode { Code = "za" , Label = "Zhuang; Chuang" } ,
					new CountryCode { Code = "zu" , Label = "Zulu" }
				};
				Info.Serialize();
			}
			Info.Details = Deserialize();
		}
		#endregion PUBLIC METHODS

		#region SERIALIZATION
		/// <summary>
		/// What: Name of the singleton
		///  Why: The name of the object is also identifying the object serialization on the support
		/// </summary>
		private const string Name = "CountryCode";
		/// <summary>
		/// What: Filename used to load/save the content of the singleton
		///  Why: one filename access allowing a centralization of the filename management
		/// </summary>
		private static string Filename
		{
			get
			{
				FileInfo fi = new FileInfo( Path.Combine( MemoryCache.Default["DataPath"] as string , Name + ".xml" ) );
				if( !fi.Directory.Exists )
					fi.Directory.Create();
				return fi.FullName;
			}
		}
		/// <summary>
		/// What: Serialization of the singleton
		///  Why: allow to save on the file system the content of the singleton - allowing manual alteration or simple recovery by deleting the file.
		/// </summary>
		private void Serialize( string filename = null )
		{
			if( string.IsNullOrEmpty( filename ) )
				filename = Filename;

			LogTrace.Label( filename );

			lock( Info )
				try
				{
					using( FileStream fs = new FileStream( filename , FileMode.Create , FileAccess.Write , FileShare.None ) )
						new XmlSerializer( typeof( CountryCode[] ) ).Serialize( fs , Details );
				}
				catch( System.Exception ex )
				{
					Logger.TraceException( ex , "The new media will not be saved" , $"Confirm the Data Path in the configuration file is correct and confirm read/write access to the path and the file ({filename} )" );
				}
		}
		/// <summary>
		/// What: Initialize the singleton with the data set from the hard disk
		///  Why: initialize the singleton object with the updated data from the support.
		/// </summary>
		private static CountryCode[] Deserialize( string filename = null )
		{
			if( string.IsNullOrEmpty( filename ) )
				filename = Filename;
			try
			{
				if( File.Exists( filename ) )
				{
					LogTrace.Label( filename );
					using( FileStream fs = new FileStream( filename , FileMode.Open , FileAccess.Read , FileShare.Read ) )
					using( XmlReader reader = XmlReader.Create( fs , new XmlReaderSettings() { XmlResolver = null } ) )
						return new XmlSerializer( typeof( CountryCode[] ) ).Deserialize( reader ) as CountryCode[];
				}
			}
			catch( System.Exception ex )
			{
				Logger.TraceException( ex , "The new media will not be loaded" , $"Confirm the Data Path in the configuration file is correct and confirm read access to the path and the file ({filename} )" );
			}
			return Array.Empty<CountryCode>();
		}
	}
}
#endregion SERIALIZATION