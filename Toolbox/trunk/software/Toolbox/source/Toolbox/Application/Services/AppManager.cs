using System;
using Toolbox.Common;
using Toolbox.Configuration;

namespace Toolbox.Application.Services
{
  public static class AppManager
  {
    private static IAssemblyClassifier _asm_classifier = new AssemblyClassifier(new AssemblyTreeBuilder());
    private static IAppInstance _app_instance = new AppInstance(new ProcessAppInstanceView(), null);
    private static INotificator _notificator = new Notificator(new NotificationViewStub());
    private static IConfigurator _configurator = new DataContractConfigurator(new ConfigFile("Settings.config"));
    private static ILoginService _login_service = new LoginService(new LoginServiceViewStub());
    private static IEventFactory _event_factory = new EventFactory();
    private static ITaskManager _task_manager = new TaskManager(new TaskViewStub());

    /// <summary>
    /// Классификатор сборок по типу
    /// </summary>
    public static IAssemblyClassifier AssemblyClassifier
    {
      get { return _asm_classifier; }
      set
      {
        if (value == null)
          throw new ArgumentNullException("value");

        _asm_classifier = value;
      }
    }

    /// <summary>
    /// Экземпляр приложения
    /// </summary>
    public static IAppInstance Instance
    {
      get { return _app_instance; }
      set
      {
        if (value == null)
          throw new ArgumentNullException("value");

        _app_instance = value;
      }
    }

    /// <summary>
    /// Сервис оповещения пользователя сообщениями
    /// </summary>
    public static INotificator Notificator
    {
      get { return _notificator; }
      set
      {
        if (value == null)
          throw new ArgumentNullException("value");

        if (ReferenceEquals(_notificator, value))
          return;

        _notificator.Dispose();
        _notificator = value;
      }
    }

    /// <summary>
    /// Хранилище настроек приложения
    /// </summary>
    public static IConfigurator Configurator
    {
      get { return _configurator; }
      set
      {
        if (value == null)
          throw new ArgumentNullException("value");

        _configurator = value;
      }
    }

    /// <summary>
    /// Менеджер задач
    /// </summary>
    public static ITaskManager TaskManager
    {
      get { return _task_manager; }
      set
      {
        if (value == null)
          throw new ArgumentNullException("value");

        _task_manager = value;
      }
    }

    /// <summary>
    /// Компонент для аутентификации пользователя
    /// </summary>
    public static ILoginService LoginService
    {
      get { return _login_service; }
      set
      {
        if (value == null)
          throw new ArgumentNullException("value");

        _login_service = value;
      }
    }

    /// <summary>
    /// Точка доступа к службе подписки
    /// </summary>
    public static IEventFactory EventFactory
    {
      get { return _event_factory; }
      set
      {
        if (value == null)
          throw new ArgumentNullException("value");

        _event_factory = value;
      }
    }
  }
}
