using Toolbox.Common;

namespace Toolbox.Text
{
  public enum EditionType
  {
    [DisplayName2("Удаление")]
    Delete,
    [DisplayName2("Вставка")]
    Insert,
    [DisplayName2("Замена")]
    Replace,
    [DisplayName2("Совпадение")]
    Coincidence
  }
}