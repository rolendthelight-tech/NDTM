using System;
using System.Collections.Generic;
using System.Text;
using AT.Toolbox.MSSQL.UI;

namespace AT.Toolbox.MSSQL.DAL.RecordBinding
{
  [DisplayNameRes("AggregateOperation", typeof(AggregateOperation))]
  public enum AggregateOperation : byte
  {
    [DisplayNameRes("None", typeof(AggregateOperation))]
    None,   // Прострая проекция
    [DisplayNameRes("Group", typeof(AggregateOperation))]
    Group,  // Группировка
    [DisplayNameRes("Count", typeof(AggregateOperation))]
    Count,  // Количество записей
    [DisplayNameRes("Max", typeof(AggregateOperation))]
    Max,    // Максимальное значение
    [DisplayNameRes("Min", typeof(AggregateOperation))]
    Min,    // Минимальное значение
    [DisplayNameRes("Sum", typeof(AggregateOperation))]
    Sum,    // Сумма
    [DisplayNameRes("Avg", typeof(AggregateOperation))]
    Avg,    // Среднее арифметическое
    [DisplayNameRes("Stdev", typeof(AggregateOperation))]
    Stdev,  // Стандартное отклонение
    [DisplayNameRes("Var", typeof(AggregateOperation))]
    Var     // Вариабельность
  }
}
