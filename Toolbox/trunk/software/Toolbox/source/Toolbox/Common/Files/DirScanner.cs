using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using Toolbox.Extensions;

namespace Toolbox.Common.Files
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field)]
	[Obsolete("Переименован в FileBrowseAttribute", true)]
	public class PathFilterAttribute : Attribute
	{
		public PathFilterAttribute(string filter)
		{
			throw new NotImplementedException("PathFilterAttribute заменён на FileBrowseAttribute");
		}
	}

	/// <summary>
	/// Помечает свойство как редактируемое с помощью диалога выбора файла с фильтром по имени.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field)]
  public class FileBrowseAttribute : Attribute
  {
		public FileBrowseAttribute(string filter)
    {
      this.Filter = filter;
    }

    /// <summary>
    /// Фильтр по имени файла
    /// </summary>
    public string Filter { get; private set; }
  }

  /// <summary>
  /// Помечает свойство как редактируемое с помощью диалога выбора директории.
  /// </summary>
  [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
  public class FolderBrowseAttribute : Attribute { }

  public class FSUtils
  {
	  [NotNull] private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(typeof(FSUtils));

    /// <summary>
    /// Получение полного дерева подкаталогов указанного каталога.
    /// </summary>
    /// <param name="Root">Каталог</param>
    /// <returns>Список подкаталогов</returns>
		public static List<string> GetAllSubdirs([NotNull] [PathReference] string Root)
    {
	    if (Root == null) throw new ArgumentNullException("Root");

			var children = new List<string>( );

      try
      {
        var dirs = new List<string>( );

        dirs.AddRange( Directory.GetDirectories( Root, @"*" ) );

        children.AddRange( dirs );

        foreach( string str in dirs )
          children.AddRange( GetAllSubdirs( str ) );
      }
      catch( Exception ex )
      {
				_log.ErrorFormat(ex, "GetAllSubdirs({0}): exception", Root);
        return new List<string>( );
      }

      return children;
    }

    /// <summary>
    /// Поиск файлов в каталоге по регулярному выражению
    /// </summary>
    /// <param name="Root">Каталог</param>
    /// <param name="Mask">Регулярное выражение для поиска</param>
    /// <param name="IncludeSubdirs">Просматривать подкаталоги</param>
    /// <returns></returns>
		public static List<string> FindFiles([NotNull] [PathReference] string Root, [CanBeNull] string Mask, bool IncludeSubdirs)
    {
	    if (Root == null) throw new ArgumentNullException("Root");
			if (string.IsNullOrEmpty(Root)) throw new ArgumentException("No root specified", "Root");

			try
      {
        var ret_val = new List<string>( );
        var dirs = new List<string>( );

        dirs.Add( Root );

        if( IncludeSubdirs )
          dirs.AddRange( GetAllSubdirs( Root ) );

        foreach( string str in dirs )
        {
          if( string.IsNullOrEmpty( str ) )
            continue;

          var files = new List<string>( );
          files.AddRange( Directory.GetFiles( str ) );

          foreach( string file in files )
          {
            if( !string.IsNullOrEmpty( Mask ) )
            {
							if (Regex.IsMatch(Path.GetFileName(file), Mask, RegexOptions.IgnoreCase | RegexOptions.Singleline))
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
				_log.ErrorFormat(ex, "FindFiles({0},{1},{2}): exception", Root, Mask, IncludeSubdirs);
        return new List<string>( );
      }
    }

    /// <summary>
    /// Получение полного размера каталога со всеми подкаталогами
    /// </summary>
    /// <param name="root"></param>
    /// <returns></returns>
		public static long DirSize([NotNull] [PathReference] string root)
    {
	    if (root == null) throw new ArgumentNullException("root");

			var d = new DirectoryInfo( root );
      long Size = 0;

      FileInfo[] fis = d.GetFiles( );

      foreach( FileInfo fi in fis )
        Size += fi.Length;

      DirectoryInfo[] dis = d.GetDirectories( );

      foreach( DirectoryInfo di in dis )
        Size += DirSize( di.Name );

      return Size;
    }

		public static bool CopyFolder([NotNull] [PathReference] string Source, [NotNull] [PathReference] string Destination)
    {
	    if (Source == null) throw new ArgumentNullException("Source");
	    if (Destination == null) throw new ArgumentNullException("Destination");

			var subidrs = new List<string>( Directory.GetDirectories( Source, @"*" ) );
      var files = FindFiles( Source, "", false );

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

		public static bool CopyFolderContents([NotNull] [PathReference] string Source, [NotNull] [PathReference] string Destination)
    {
	    if (Source == null) throw new ArgumentNullException("Source");
	    if (Destination == null) throw new ArgumentNullException("Destination");

			var subidrs = new List<string>( Directory.GetDirectories( Source, @"*" ) );
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