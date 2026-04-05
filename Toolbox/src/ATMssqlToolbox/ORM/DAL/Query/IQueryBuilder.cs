using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace AT.Toolbox.MSSQL.DAL.RecordBinding
{
  /// <summary>
  /// Интерфейс построителя запроса для паттернов Active Record + Query Object
  /// </summary>
  public interface IQueryBuilder
  {
    string MainTable { get; set; }
    void BuildProjection(Projection prj);
    void BeginFilter();
    void StartBlock();
    void EndBlock();
    void Negate();
    void Exists();
    void BuildSeparator(bool conjunction);
    void BuildExpression(ExpressionFilter filter);
    void BuildIsNull(NullFilter filter);
    void BuildIn(InFilter inFilter);
    void BuildJoiner(Joiner joiner);
    void BuildGroupBy(List<string> groups);
    void BuildOrderBy(SortFieldCollection ordering);
    void BuildCustomText(string customText);
    object GetResult();
  }
}
