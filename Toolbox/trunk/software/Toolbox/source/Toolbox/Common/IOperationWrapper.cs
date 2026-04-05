using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using JetBrains.Annotations;
using Toolbox.Extensions;

namespace Toolbox.Common
{
  /// <summary>
  /// Универсальный декоратор для выполнения произвольных операций
  /// </summary>
  public interface IOperationWrapper
  {
    /// <summary>
    /// Выполнение операции, возвращающей значение
    /// </summary>
    /// <typeparam name="TType">Тип возвращаемого значения</typeparam>
    /// <param name="action">Операция</param>
    /// <returns>Значение, которое вернула операция</returns>
    TType Invoke<TType>([NotNull] Func<TType> action);

    /// <summary>
    /// Выполнение операции, не возвращающей значения
    /// </summary>
    /// <param name="action">Операция</param>
    void Invoke([NotNull] Action action);
  }

  public class EmptyOperationWrapper : IOperationWrapper
  {
    #region IOperationWrapper Members

    public TType Invoke<TType>(Func<TType> action)
    {
      return action != null ? action() : default(TType);
    }

    public void Invoke(Action action)
    {
      if (action != null)
        action();
    }

    #endregion
  }

  /// <summary>
  /// Декоратор для прокидывания операций в поток пользовательского интерфейса
  /// через ISynchronizeInvoke
  /// </summary>
  public class SynchronizeOperationWrapper : IOperationWrapper
  {
	  [NotNull] private readonly ISynchronizeInvoke m_invoker;

    public SynchronizeOperationWrapper([NotNull] ISynchronizeInvoke invoker)
    {
      if (invoker == null)
        throw new ArgumentNullException("invoker");

      m_invoker = invoker;
    }

    /// <summary>
    /// Вызывать методы, не возвращающие значения, через BeginInvoke, а не Invoke
    /// </summary>
    public bool CallProceduresAsync { get; set; }

    #region IOperationWrapper Members

    public TType Invoke<TType>(Func<TType> action)
    {
      if (action == null)
        return default(TType);

      if (m_invoker.InvokeRequired)
        return (TType)m_invoker.Invoke(action, ArrayExtensions.Empty<object>());
      else
        return action();
    }

    public void Invoke(Action action)
    {
      if (action == null)
        return;

      if (m_invoker.InvokeRequired)
      {
        if (this.CallProceduresAsync)
          m_invoker.BeginInvoke(action, ArrayExtensions.Empty<object>());
        else
          m_invoker.Invoke(action, ArrayExtensions.Empty<object>());
      }
      else
        action();
    }

    #endregion
  }
}
