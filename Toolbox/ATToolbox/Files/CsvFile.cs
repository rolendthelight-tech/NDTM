using System;
using System.Collections.Generic;
using System.IO;
using System.Text;


namespace AT.Toolbox.Files
{
  public class CsvFile
  {
    private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(CsvFile).Name);
    
    public string UsedEncoding { get; set; }

    public string Separator { get; set; }

    public void ReadLines(string file_name, out List<string[]> table)
    {
      table = new List<string[]>( );

      Encoding enc = Encoding.Unicode;

      if( !string.IsNullOrEmpty( UsedEncoding ) )
        enc = Encoding.GetEncoding( UsedEncoding );

      FileStream fs = null;

      try
      {
        fs = new FileStream( file_name, FileMode.Open, FileAccess.Read, FileShare.ReadWrite );

        using( StreamReader reader = new StreamReader( fs, enc ) )
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
      catch( Exception ex )
      {
        Log.Error( "Failed to load file", ex );
      }
      finally
      {
        if( null != fs )
          fs.Close( );
      }
    }

    public void WriteFile(string file_name, List<string[]> table)
    {
      Encoding enc = Encoding.Unicode;

      if( !string.IsNullOrEmpty( UsedEncoding ) )
        enc = Encoding.GetEncoding( UsedEncoding );

      FileStream fs = null;

      try
      {
        if( File.Exists( file_name ) )
          fs = new FileStream( file_name, FileMode.Truncate, FileAccess.Write, FileShare.None );
        else
          fs = new FileStream( file_name, FileMode.Create, FileAccess.Write, FileShare.None );

        using( StreamWriter writer = new StreamWriter( fs, enc ) )
        {
          foreach( string[] strings in table )
            writer.WriteLine( string.Join( Separator, strings ) );
        }
      }
      catch( Exception ex )
      {
        Log.Error( "Failed to save file", ex );
      }
      finally
      {
        if( null != fs )
          fs.Close( );
      }
    }
  }
}