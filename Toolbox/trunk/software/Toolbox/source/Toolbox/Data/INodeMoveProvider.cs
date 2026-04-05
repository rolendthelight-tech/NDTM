using System;
using System.Collections;
using System.ComponentModel;

namespace Toolbox.Data
{
  /// <summary>
  /// Определяет операции, соответствующие перетаскиванию узла
  /// </summary>
  public interface INodeMoveProvider
  {
    /// <summary>
    /// Проверяет допустимость перетаскивания
    /// </summary>
    /// <param name="source">Перетаскиваемый узел</param>
    /// <param name="destination">Узел, на который перетаскивается</param>
    /// <param name="list">Коллекция, связанная с узлом, на который перетаскивается</param>
    /// <returns><code>true</code>, если перетаскивание допустимо</returns>
    bool Check(object source, object destination, IList list);

    /// <summary>
    /// Выполняет над моделью операцию, соответствующую перетаскиванию
    /// </summary>
    /// <param name="source">Перетаскиваемый узел</param>
    /// <param name="destination">Узел, на который перетаскивается</param>
    /// <param name="list">Коллекция, связанная с узлом, на который перетаскивается</param>
    /// <returns>Новый узел, возникший после перетаскивания</returns>
    object Perform(object source, object destination, IList list);
  }

  [AttributeUsage(AttributeTargets.Property)]
  public sealed class NoBindAttribute : Attribute { }
}
