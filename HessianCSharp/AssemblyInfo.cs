using System.Reflection;
using System.Runtime.CompilerServices;

// Allgemeine Informationen ?er eine Assembly werden ?er folgende Attribute 
// gesteuert. Ändern Sie diese Attributswerte, um die Informationen zu modifizieren,
// die mit einer Assembly verkn?ft sind.

[assembly: AssemblyTitle("")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("")]
[assembly: AssemblyCopyright("")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Versionsinformationen f? eine Assembly bestehen aus folgenden vier Werten:
//      Hauptversion
//      Nebenversion 
//      Buildnummer
//      Revision

// Sie k?nen alle Werte oder die standardmäßige Revision und Buildnummer 
// mit '*' angeben:

[assembly: AssemblyVersion("1.3.*")]

// Um die Assembly zu signieren, m?sen Sie einen Schl?sel angeben. Weitere Informationen 
// ?er die Assemblysignierung finden Sie in der Microsoft .NET Framework-Dokumentation.

// Mit den folgenden Attributen k?nen Sie festlegen, welcher Schl?sel f? die Signierung verwendet wird. 

// Hinweise: 
//   (*) Wenn kein Schl?sel angegeben ist, wird die Assembly nicht signiert.
//   (*) KeyName verweist auf einen Schl?sel, der im CSP (Crypto Service
//       Provider) auf Ihrem Computer installiert wurde. KeyFile verweist auf eine Datei, die einen
//       Schl?sel enth?t.
//   (*) Wenn die Werte f? KeyFile und KeyName angegeben werden, 
//       werden folgende Vorg?ge ausgef?rt:
//       (1) Wenn KeyName im CSP gefunden wird, wird dieser Schl?sel verwendet.
//       (2) Wenn KeyName nicht vorhanden ist und KeyFile vorhanden ist, 
//           wird der Schl?sel in KeyFile im CSP installiert und verwendet.
//   (*) Um eine KeyFile zu erstellen, k?nen Sie das Programm sn.exe (Strong Name) verwenden.
//       Wenn KeyFile angegeben wird, muss der Pfad von KeyFile
//       relativ zum Projektausgabeverzeichnis sein:
//       %Project Directory%\obj\<configuration>. Wenn sich KeyFile z.B.
//       im Projektverzeichnis befindet, geben Sie das AssemblyKeyFile-Attribut 
//       wie folgt an: [assembly: AssemblyKeyFile("..\\..\\mykey.snk")]
//   (*) Das verz?ern der Signierung ist eine erweiterte Option. Weitere Informationen finden Sie in der
//       Microsoft .NET Framework-Dokumentation.

[assembly: AssemblyDelaySign(false)]
[assembly: AssemblyKeyFile("")]
[assembly: AssemblyKeyName("")]
