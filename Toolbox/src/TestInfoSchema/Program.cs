using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using AT.Toolbox;
using System.Data.Linq.SqlClient;
using System.Data.Linq;
using System.Reflection;
using System.Linq.Expressions;
using System.Data.Linq.Mapping;

namespace TestInfoSchema
{
  static class Program
  {
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
      /*using (SqlProvider prov = new SqlProvider())
      {
        using (DataContext dc = new DataContext(@"Data Source=.\SQLEXPRESS;AttachDbFilename=;Initial Catalog=Holidays;Integrated Security=True;User ID=;Password="))
        {
          IQueryable qo = from e in dc.GetTable<DatabaseEntry>()
                          join s in dc.GetTable<ServerEntry>()
                          on e.Name equals s.DbFilter
                          where e.Used
                          //&& dc.GetTable<ServerEntry>().Where(s => e.Name.StartsWith(s.DbFilter)).Count() > 0
                          select e.Name;

          var cmd = dc.GetCommand(qo);
          cmd.ToString();

          foreach (MethodInfo mi in typeof(SqlProvider).GetMethods(BindingFlags.Public
            | BindingFlags.NonPublic | BindingFlags.Instance))
          {
            if (mi.Name == "BuildQuery")
            {
              var parms = mi.GetParameters();

              if (parms.Length == 2 && parms[0].ParameterType == typeof(Expression))
              {
                object second = Activator.CreateInstance(parms[1].ParameterType);

                // Смотреть в сторону System.Data.Linq.SqlClient.QueryConverter
                Array arr = (Array)mi.Invoke(prov, new object[] { qo.Expression, second });

                foreach (var q in arr)
                {
                  Console.WriteLine(q);
                }
              }
            }
          }
        }
      }*/
      AppWrapper.Run<MainForm>(true);
    }
  }

  /*[Table]
  public class DatabaseEntry
  {
    [Column]
    public bool Used {get; set;}

    [Column]
    public string Name { get; set; }
  }

  [Table]
  public class ServerEntry
  {
    readonly List<DatabaseEntry> m_bases = new List<DatabaseEntry>();

    [Column]
    public string DbFilter { get; set; }

    public List<DatabaseEntry> Bases
    {
      get { return m_bases; }
    } 
  }*/
}
