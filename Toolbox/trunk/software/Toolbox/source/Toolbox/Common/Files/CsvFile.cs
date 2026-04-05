using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using JetBrains.Annotations;

namespace Toolbox.Common.Files
{
  public class CsvFile
  {
	  [NotNull] private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(typeof(CsvFile));

    public string UsedEncoding { get; set; }

    public string Separator { get; set; }

		public void ReadLines([NotNull] [PathReference] string fileName, out List<string[]> table)
    {
	    if (fileName == null) throw new ArgumentNullException("fileName");

			table = new List<string[]>( );

      Encoding enc = Encoding.Unicode;

      if( !string.IsNullOrEmpty( UsedEncoding ) )
        enc = Encoding.GetEncoding( UsedEncoding );

	    try
      {
	      using (var fs = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read))
	      {
		      using( var reader = new StreamReader( fs, enc ) )
		      {
			      while( !reader.EndOfStream )
			      {
				      string line = reader.ReadLine( );

				      if( string.IsNullOrEmpty( line ) )
					      continue;

				      table.Add( line.Split( Separator.ToCharArray( ) ) );
			      }
		      }
	      }
      }
      catch( Exception ex )
      {
        _log.Error( "Failed to load file", ex );
      }
    }

		public void WriteFile([NotNull] [PathReference] string fileName, [NotNull] List<string[]> table)
    {
	    if (fileName == null) throw new ArgumentNullException("fileName");
	    if (table == null) throw new ArgumentNullException("table");

	    Encoding enc = Encoding.Unicode;

      if( !string.IsNullOrEmpty( UsedEncoding ) )
        enc = Encoding.GetEncoding( UsedEncoding );

	    try
	    {
		    using (var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None))
		    {
			    using( var writer = new StreamWriter( fs, enc ) )
			    {
				    foreach( string[] strings in table )
					    writer.WriteLine( string.Join( Separator, strings ) );
			    }
		    }
	    }
	    catch( Exception ex )
      {
        _log.Error( "Failed to save file", ex );
      }
    }
  }
}