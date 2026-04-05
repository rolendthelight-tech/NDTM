using System.ComponentModel;
using System.Drawing;
using System;
using System.Windows.Forms;

namespace AT.Toolbox.Misc
{
  /// <summary>
  /// Интерфейс для объекта -- задачи, предназначеной для выполнения в фоновом режиме с помощью BackgroundWorkerForm
  /// </summary>
  public interface IBackgroundWork : INotifyPropertyChanged
  {
    /// <summary>
    /// Закрывать ли форму при завершении
    /// </summary>
    bool CloseOnFinish { get; }

    /// <summary>
    /// Бегающая полоска вместо прогресса операции
    /// </summary>
    bool IsMarquee { get; }

    /// <summary>
    /// Можно ли прервать задание
    /// </summary>
    bool CanCancel { get; }

    /// <summary>
    /// Иконка
    /// </summary>
    Bitmap Icon { get; }

    /// <summary>
    /// Название задачи
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Вес задачи -- требуется только для последовательного выполнения нескольких задач
    /// </summary>
    float Weight { get; }

    /// <summary>
    /// Реализация задачи для выполнения. Тебуется поддержа BackgroundWorker.CancellationPending для отмены
    /// </summary>
    /// <param name="worker">Для оповещения задачи об отмене и вызывающей прогаммы о прогрессе выполнения</param>
    void Run(BackgroundWorker worker);
  }
}