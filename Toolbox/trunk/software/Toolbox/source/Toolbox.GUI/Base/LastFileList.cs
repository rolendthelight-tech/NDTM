using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using Toolbox.Configuration;
using System.Runtime.Serialization;
using System.Drawing;
using Toolbox.Application.Services;
using Toolbox.Extensions;
using System.IO;

namespace Toolbox.GUI.Base
{
  public class LastFileList
  {
	  [NotNull] private readonly ILastFileView m_view;

    public LastFileList([NotNull] ILastFileView view)
    {
      if (view == null)
        throw new ArgumentNullException("view");

      m_view = view;
    }

    public void Init()
    {
      var section = AppManager.Configurator.GetSection<LastFilesSection>();

      foreach (var kv in section.Files.OrderByDescending(f => f.Value).ToArray())
      {
        if (m_view.Width > 0 && m_view.Font != null)
          m_view.AddMenuItem(PathExtensions.TruncatePath(kv.Key, m_view.Font, m_view.Width), kv.Key, false);
        else
          m_view.AddMenuItem(kv.Key, kv.Key, false);
      }

      if (section.Files.Count == 0)
        m_view.Hide();
    }

		public void Update([NotNull] [PathReference] string fileName)
    {
	    if (fileName == null) throw new ArgumentNullException("fileName");

	    var section = AppManager.Configurator.GetSection<LastFilesSection>();
      var path = Path.GetFullPath(fileName);

      section.Files[path] = DateTime.Now;

      m_view.Clear();

      foreach (var kv in section.Files.OrderByDescending(f => f.Value).ToArray())
      {
        if (m_view.Width > 0 && m_view.Font != null)
          m_view.AddMenuItem(PathExtensions.TruncatePath(kv.Key, m_view.Font, m_view.Width), kv.Key, false);
        else
          m_view.AddMenuItem(kv.Key, kv.Key, false);
      }

      m_view.Show();
    }
  }

  public class FileNameEventArgs : EventArgs
  {
	  [NotNull] private readonly string m_file_name;

		public FileNameEventArgs([NotNull] [PathReference] string fileName)
    {
      if (fileName == null) throw new ArgumentNullException("fileName");
      if (string.IsNullOrEmpty(fileName)) throw new ArgumentException("Empty", "fileName");

      //if (!File.Exists(fileName))
      //  throw new FileNotFoundException();

      m_file_name = fileName;
    }

	  [NotNull]
	  public string FileName
    {
      get { return m_file_name; }
    }
  }

  [DataContract]
  public class LastFilesSection : ConfigurationSection
  {
    private Dictionary<string, DateTime> m_files;

    [DataMember]
    public Dictionary<string, DateTime> Files
    {
      get { return m_files ?? (m_files = new Dictionary<string, DateTime>()); }
    }
  }

  public interface ILastFileView
  {
		void AddMenuItem(string caption, [NotNull] [PathReference] string fileName, bool atStart);

    void Hide();

    void Show();

    void Clear();

	  [CanBeNull]
	  Font Font { get; }

    int Width { get; }
  }
}
