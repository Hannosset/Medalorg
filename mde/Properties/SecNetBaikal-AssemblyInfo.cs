using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;

[assembly: NeutralResourcesLanguageAttribute( "en" )]
[assembly: AssemblyCompany( "SecNetBaikal" )]
[assembly: AssemblyCopyright( "MIT License 2023" )]
[assembly: AssemblyTrademark( "Baikal Freeware" )]
[assembly: System.CLSCompliant( false )]

[assembly: AssemblyCulture( "" )]

#if DEBUG
[assembly: AssemblyConfiguration( "Debug" )]
#else
[assembly: AssemblyConfiguration( "Release" )]
#endif

// Setting ComVisible to false makes the types in this assembly not visible
// to COM components.  If you need to access a type in this assembly from
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible( false )]
