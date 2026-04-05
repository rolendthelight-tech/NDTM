using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using log4net;

namespace AT.Toolbox
{
  public class ApplicationLoadingTask
  {
    private static readonly ILog _log = LogManager.GetLogger("ApplicationLoadingTask");

    private readonly ILoadingQueue m_queue;
    private readonly DependencyContainer m_container;

    public ApplicationLoadingTask(ILoadingQueue queue, DependencyContainer container)
    {
      if (queue == null)
        throw new ArgumentNullException("queue");

      if (container == null)
        throw new ArgumentNullException("container");

      m_queue = queue;
      m_container = container;
    }

    /// <summary>
    /// Аргументы командной строки
    /// </summary>
    public string[] CommandArgs { get; set; }

    public ApplicationLoadingResult Run(ISynchronizeInvoke invoker, BackgroundWorker worker)
    {
      var ret = new ApplicationLoadingResult();

      var items = new TopologicalSort<Type>(m_queue.GetLoaders()).Sort();

      if (items.Count > 0)
      {
        var context = new LoadingContext(m_container, worker, invoker);

        context.CommandArgs = this.CommandArgs;

        ret.Buffer = context.Buffer;

        foreach (IApplicationLoader item in items)
        {
          if (worker.WorkerReportsProgress)
            worker.ReportProgress(0, "");
          
          try
          {
            var loaded = item.Load(context);

            ret[item.Key] = loaded ? new EmptyApplicationLoader(item.Key) : item;

            if (!loaded)
              ret.Success = false;
          }
          catch (Exception ex)
          {
            context.Buffer.Add(ex);
            _log.Error("Run(): exception", ex);
            ret[item.Key] = item;
            ret.Success = false;
          }
        }
      }

      return ret;
    }
  }

  public sealed class ApplicationLoadingResult : ILoadingQueue
  {
    private readonly Dictionary<Type, IApplicationLoader> m_loaders = new Dictionary<Type,IApplicationLoader>();
    
    public ApplicationLoadingResult()
    {
      this.Success = true;
      this.Buffer = new InfoBuffer();
    }

    public IApplicationLoader this[Type key]
    {
      get { return m_loaders[key]; }
      internal set { m_loaders[key] = value; }
    }
    
    public bool Success { get; internal set; }

    public InfoBuffer Buffer { get; internal set; }

    public IApplicationLoader[] GetLoaders()
    {
      return m_loaders.Values.ToArray();
    }
  }
}
