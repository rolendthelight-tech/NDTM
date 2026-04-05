using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Toolbox.Common;
using Toolbox.Threading;

namespace Toolbox.Application.Services
{
  /// <summary>
  /// Сервис для получения логина и пароля.
  /// </summary>
  public interface ILoginService
  {
    /// <summary>
    /// Получение логина и пароля.
    /// </summary>
    /// <param name="connectionIdentity">Идентификатор подключения</param>
    /// <param name="crd">Логин и пароль</param>
    /// <returns><code>true</code>, если логин и пароль получены. Иначе, <code>false</code></returns>
    bool TryGetCredentials([NotNull] string connectionIdentity, [CanBeNull] out UserCredentials crd);

    /// <summary>
    /// Запоминание логина и пароля
    /// </summary>
    /// <param name="connectionIdentity">Идентификатор подключения</param>
    /// <param name="crd">Логин и пароль</param>
    void SaveCredentials([NotNull] string connectionIdentity, [NotNull] UserCredentials crd);

    /// <summary>
    /// Удаление логина и пароля из кэша
    /// </summary>
    /// <param name="connectionIdentity">Идентификатор подключения</param>
    void RemoveCredentials([NotNull] string connectionIdentity);

    /// <summary>
    /// Очистка кэша логинов и паролей
    /// </summary>
    void RemoveAllCredentials();
  }

  public sealed class UserCredentials
  {
    public string Login { get; set; }
    public string Password { get; set; }
    public bool SaveCredentials { get; set; }

    public override string ToString()
    {
      if (string.IsNullOrEmpty(this.Login))
        return base.ToString();

      return this.Login;
    }
  }

  public interface ILoginServiceView : ISynchronizeProvider
  {
    bool ShowAuthenticationForm([CanBeNull] out UserCredentials crd);
  }

  public class LoginService : ILoginService
  {
	  [NotNull] private readonly ILoginServiceView m_view;
	  [NotNull] private readonly Dictionary<string, UserCredentials> m_cache = new Dictionary<string, UserCredentials>();
	  [NotNull] private readonly LockSource m_lock = new LockSource();

    public LoginService([NotNull] ILoginServiceView view)
    {
      if (view == null)
        throw new ArgumentNullException("view");

      m_view = view;
    }

    public bool TryGetCredentials([NotNull] string connectionIdentity, [CanBeNull] out UserCredentials crd)
    {
      if (connectionIdentity == null) throw new ArgumentNullException("connectionIdentity");
      if (string.IsNullOrEmpty(connectionIdentity)) throw new ArgumentException("Empty", "connectionIdentity");

      using (m_lock.GetReadLock())
      {
        if (m_cache.TryGetValue(connectionIdentity, out crd))
          return true;
      }

      var wrapper = new SynchronizeOperationWrapper(m_view.Invoker);
      UserCredentials cpy = null;

      if (wrapper.Invoke(() => m_view.ShowAuthenticationForm(out cpy)))
      {
        crd = cpy;
        return crd != null;
      }
      else
        return false;
    }

    public void SaveCredentials([NotNull] string connectionIdentity, [NotNull] UserCredentials crd)
    {
      if (connectionIdentity == null) throw new ArgumentNullException("connectionIdentity");
      if (string.IsNullOrEmpty(connectionIdentity)) throw new ArgumentException("Empty", "connectionIdentity");

      if (crd == null)
        throw new ArgumentNullException("crd");

      using (m_lock.GetWriteLock())
      {
        crd.SaveCredentials = false;
        m_cache[connectionIdentity] = crd;
      }
    }

    public void RemoveCredentials([NotNull] string connectionIdentity)
    {
      if (connectionIdentity == null) throw new ArgumentNullException("connectionIdentity");
      if (string.IsNullOrEmpty(connectionIdentity)) throw new ArgumentException("Empty", "connectionIdentity");

      using (m_lock.GetWriteLock())
      {
        m_cache.Remove(connectionIdentity);
      }
    }

    public void RemoveAllCredentials()
    {
      using (m_lock.GetWriteLock())
      {
        m_cache.Clear();
      }
    }
  }

  internal class LoginServiceViewStub : SynchronizeProviderStub, ILoginServiceView
  {
    public bool ShowAuthenticationForm([CanBeNull] out UserCredentials crd)
    {
      crd = null;
      return false;
    }
  }
}
