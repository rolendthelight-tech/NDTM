using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.ComponentModel;
using ERMS.Core.Common;
using System.Drawing;

namespace MapInterfaces
{
  public class BackgroundGeographyLoader : IDisposable, IMapControl
  {
    private volatile bool m_disposed;
    private readonly ManualResetEvent m_signal = new ManualResetEvent(false);

    private readonly GeographyLoader m_loader;
    private readonly ISynchronizeInvoke m_invoker;
    private readonly IMapControl m_control;

    private readonly List<RectangleF> m_last_bounds = new List<RectangleF>();
    private readonly Queue<DeferredInvoker> m_pending_actions = new Queue<DeferredInvoker>();

    public BackgroundGeographyLoader(IGeographyService service, ISynchronizeInvoke invoker, IMapControl control)
    {
      if (service == null)
        throw new ArgumentNullException("service");

      if (invoker == null)
        throw new ArgumentNullException("invoker");

      if (control == null)
        throw new ArgumentNullException("control");

      m_loader = new GeographyLoader(service, this);
      m_invoker = invoker;
      m_control = control;

      this.InvokeTimeout = new TimeSpan(0, 1, 0);

      Thread th = new Thread(this.Run);
      th.SetApartmentState(ApartmentState.STA);
      th.Name = "Geometry loader";
      th.Start();
    }

    private void Run()
    {
      while (m_signal.WaitOne(Timeout.Infinite))
      {
        if (m_disposed)
          break;

        m_signal.Reset();

        lock (m_pending_actions)
        {
          while (m_pending_actions.Count > 0)
            m_pending_actions.Dequeue().Execute();
        }

        RectangleF? rect = null;

        lock (m_last_bounds)
        {
          if (m_last_bounds.Count == 0)
            continue;

          float x1 = m_last_bounds.Min(ev => ev.X);
          float x2 = m_last_bounds.Max(ev => ev.Right);
          float y1 = m_last_bounds.Min(ev => ev.Y);
          float y2 = m_last_bounds.Max(ev => ev.Bottom);

          rect = new RectangleF(x1, y1, x2 - x1, y2 - y1);
          //rect = m_last_bounds.LastOrDefault();

          m_last_bounds.Clear();
        }

        if (rect != null)
        {
          m_loader.Skip = false;
          m_loader.GetObjectsByRect(rect.Value, "");

          if (!m_loader.Skip)
          {
            // Запомнить прямоугольник, чтобы больше полностью его не грузить
          }
        }
      }
    }

    public void ViewChanged(RectangleF bounds)
    {
      lock (m_last_bounds)
      {
        m_loader.Skip = true;

        m_last_bounds.Add(bounds);
        m_signal.Set();
      }
    }

    public Dictionary<string, MapLayer> GetLayers()
    {
      return m_loader.GetLayerTables();
    }

    #region Invoke

    public TimeSpan InvokeTimeout { get; set; }

    public TResult Invoke<TResult>(Func<GeographyLoader, TResult> handler)
    {
      using (var invoker = new FunctionDeferredInvoker<TResult>(handler, m_loader))
      {
        lock (m_pending_actions)
        {
          m_pending_actions.Enqueue(invoker);
          m_signal.Set();
        }

        if (!invoker.Signal.WaitOne(this.InvokeTimeout))
          throw new TimeoutException();

        return invoker.Result;
      }
    }

    public void Invoke(Action<GeographyLoader> handler)
    {
      using (var invoker = new ActionDeferredInvoker(handler, m_loader))
      {
        lock (m_pending_actions)
        {
          m_pending_actions.Enqueue(invoker);
          m_signal.Set();
        }

        if (!invoker.Signal.WaitOne(this.InvokeTimeout))
          throw new TimeoutException();
      }
    }

    public void BeginInvoke(Action<GeographyLoader> handler)
    {
      using (var invoker = new ActionDeferredInvoker(handler, m_loader))
      {
        lock (m_pending_actions)
        {
          m_pending_actions.Enqueue(invoker);
          m_signal.Set();
        }
      }
    }

    private abstract class DeferredInvoker : IDisposable
    {
      private readonly GeographyLoader m_loader;
      private readonly ManualResetEvent m_signal = new ManualResetEvent(false);

      public DeferredInvoker(GeographyLoader loader)
      {
        if (loader == null)
          throw new ArgumentNullException("loader");

        m_loader = loader;
      }

      public abstract void Execute();

      protected GeographyLoader Loader
      {
        get { return m_loader; }
      }

      public ManualResetEvent Signal
      {
        get { return m_signal; }
      }

      #region IDisposable Members

      public void Dispose()
      {
        m_signal.Close();
      }

      #endregion
    }

    private class FunctionDeferredInvoker<TResult> : DeferredInvoker
    {
      private readonly Func<GeographyLoader, TResult> m_function;

      public FunctionDeferredInvoker(Func<GeographyLoader, TResult> function, GeographyLoader loader)
        : base(loader)
      {
        if (function == null)
          throw new ArgumentNullException("function");

        m_function = function;
      }

      public TResult Result { get; private set; }

      public override void Execute()
      {
        this.Result = m_function(this.Loader);
      }
    }

    private class ActionDeferredInvoker : DeferredInvoker
    {
      private readonly Action<GeographyLoader> m_action;
      
      public ActionDeferredInvoker(Action<GeographyLoader> action, GeographyLoader loader)
        : base(loader)
      {
        if (action == null)
          throw new ArgumentNullException("action");

        m_action = action;
      }

      public override void Execute()
      {
        m_action(this.Loader);
      }
    }

    #endregion

    #region IDisposable Members

    public void Dispose()
    {
      m_disposed = true;
      m_loader.Skip = true;
      m_signal.Set();
    }

    #endregion

    #region IMapControl Members

    HashSet<string> IMapControl.GetVisibleLayers()
    {
      if (m_invoker.InvokeRequired)
        return (HashSet<string>)m_invoker.Invoke(new Func<HashSet<string>>(m_control.GetVisibleLayers), null);
      else
        return m_control.GetVisibleLayers();
    }

    int IMapControl.MaxIDCount
    {
      get
      {
        if (m_invoker.InvokeRequired)
          return (int)m_invoker.Invoke(new Func<int>(() => m_control.MaxIDCount), null);
        else
          return m_control.MaxIDCount;
      }
    }

    Dictionary<string, HashSet<int>> IMapControl.GetEntityFilter()
    {
      if (m_invoker.InvokeRequired)
        return (Dictionary<string, HashSet<int>>)m_invoker.Invoke(new Func<Dictionary<string, HashSet<int>>>(m_control.GetEntityFilter), null);
      else
        return m_control.GetEntityFilter();
    }

    HashSet<int> IMapControl.GetLoadedData(string layerName)
    {
      if (m_invoker.InvokeRequired)
        return (HashSet<int>)m_invoker.Invoke(new Func<string, HashSet<int>>(m_control.GetLoadedData),
          new object[] { layerName });
      else
        return m_control.GetLoadedData(layerName);
    }

    void IMapControl.InsertFeature(MappableObject mo)
    {
      if (m_invoker.InvokeRequired)
        m_invoker.Invoke(new Action<MappableObject>(m_control.InsertFeature), new object[] { mo });
      else
        m_control.InsertFeature(mo);
    }

    void IMapControl.DeleteFeature(MappableObject mo)
    {
      if (m_invoker.InvokeRequired)
        m_invoker.Invoke(new Action<MappableObject>(m_control.DeleteFeature), new object[] { mo });
      else
        m_control.DeleteFeature(mo);
    }

    void IMapControl.InsertFeatureRange(HashSet<MappableObject> objects)
    {
      if (m_invoker.InvokeRequired)
        m_invoker.Invoke(new Action<HashSet<MappableObject>>(m_control.InsertFeatureRange),
          new object[] { objects });
      else
        m_control.InsertFeatureRange(objects);
    }

    void IMapControl.DisplayToolTip(ToolTipEntry toolTip)
    {
      if (m_invoker.InvokeRequired)
        m_invoker.BeginInvoke(new Action<ToolTipEntry>(m_control.DisplayToolTip),
          new object[] { toolTip });
      else
        m_control.DisplayToolTip(toolTip);
    }

    #endregion
  }
}
