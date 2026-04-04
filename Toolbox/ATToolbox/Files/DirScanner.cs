namespace AT.Toolbox.Files
{
  using System;
  using System.Collections.Generic;
  using System.IO;
  using System.Text.RegularExpressions;


  public class FSUtils
  {
    private static readonly log4net.ILog Log = log4net.LogManager.GetLogger(typeof(FSUtils).Name);

    /// <summary>
    /// Получение полного дерева подкаталогов указаного кактлога.
    /// </summary>
    /// <param name="Root">Каталог</param>
    /// <returns>Список подкаталогов</returns>
    public static List<string> GetAllSubdirs( string Root )
    {
      List<string> children = new List<string>( );

      try
      {
        List<string> dirs = new List<string>( );

        dirs.AddRange( Directory.GetDirectories( Root, @"*.*" ) );

        children.AddRange( dirs );

        foreach( string str in dirs )
          children.AddRange( GetAllSubdirs( str ) );
      }
      catch( Exception ex )
      {
        Log.Error( String.Format("GetAllSubdirs({0}): exception", Root ), ex );
        return new List<string>( );
      }

      return children;
    }

    /// <summary>
    /// Поиск файлов в каталоге по регулярному выражению
    /// </summary>
    /// <param name="Root">Каталог</param>
    /// <param name="Mask">Регулярное выражение для поиска</param>
    /// <param name="IncludeSubdirs">Просматривать подкаталоги или нет</param>
    /// <returns></returns>
    public static List<string> FindFiles( string Root, string Mask, bool IncludeSubdirs )
    {
      try
      {
        if( string.IsNullOrEmpty( Root ) )
          throw new ArgumentException( "No root specified", "Root" );

        List<string> ret_val = new List<string>( );
        List<string> dirs = new List<string>( );

        dirs.Add( Root );

        if( IncludeSubdirs )
          dirs.AddRange( GetAllSubdirs( Root ) );

        foreach( string str in dirs )
        {
          if( string.IsNullOrEmpty( str ) )
            continue;

          List<string> files = new List<string>( );
          files.AddRange( Directory.GetFiles( str ) );

          foreach( string file in files )
          {
            if( !string.IsNullOrEmpty( Mask ) )
            {
              if( Regex.IsMatch( Path.GetFileName( file ), Mask, RegexOptions.IgnoreCase ) )
                ret_val.Add( file );
            }
            else
              ret_val.Add( file );
          }
        }

        return ret_val;
      }
      catch( Exception ex )
      {
        Log.Error( String.Format( "FindFiles({0},{1},{2}): exception", Root, Mask, IncludeSubdirs ), ex );
        return new List<string>( );
      }
    }

    /// <summary>
    /// Получение полного размера каталога со всеми подкаталогами
    /// </summary>
    /// <param name="Root"></param>
    /// <returns></returns>
    public static long DirSize( string Root )
    {
      DirectoryInfo d = new DirectoryInfo( Root );
      long Size = 0;

      FileInfo[] fis = d.GetFiles( );

      foreach( FileInfo fi in fis )
        Size += fi.Length;

      DirectoryInfo[] dis = d.GetDirectories( );

      foreach( DirectoryInfo di in dis )
        Size += DirSize( di.Name );

      return Size;
    }

    public static bool CopyFolder( string Source, string Destination )
    {
      List<string> subidrs = new List<string>( Directory.GetDirectories( Source, @"*.*" ) );
      List<string> files = FindFiles( Source, "", false );

      if( !Directory.Exists( Destination ) )
        Directory.CreateDirectory( Destination );

      foreach( string s in files )
      {
        string s2 = Path.GetFileName( s );
        File.Copy( s, Path.Combine( Destination, s2 ), true );
      }

      foreach( string s in subidrs )
      {
        string dest = Path.GetFileName( s );
        CopyFolder( s, Path.Combine( Destination, dest ) );
      }

      return true;
    }

    public static bool CopyFolderContents( string Source, string Destination )
    {
      List<string> subidrs = new List<string>( Directory.GetDirectories( Source, @"*.*" ) );
      List<string> files = FindFiles( Source, "", false );

      foreach( string s in files )
      {
        string s2 = Path.GetFileName( s );
        File.Copy( s, Path.Combine( Destination, s2 ), true );
      }

      foreach( string s in subidrs )
      {
        string dest = Path.GetFileName( s );
        CopyFolder( s, Path.Combine( Destination, dest ) );
      }

      return true;
    }
  }
}