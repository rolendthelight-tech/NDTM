namespace Toolbox.Text
{
  public class Edition
  {
    /// <summary>
    /// Индекс символа "эталонной" строки, который должен быть, в соответствии с типом EditionType,
    /// удален, заменен или ПОСЛЕ которого должен быть вставлен символ тестовой строки; т.о. может быть
    /// равен -1, если вставка символа должна быть осуществлена в начало тестовой строки
    /// </summary>
    public int Index;

    /// <summary>
    /// Тип редактирования
    /// </summary>
    public EditionType EditionType;

    /// <summary>
    /// Позиция элемента в строке [символ]
    /// </summary>
    public int Position;

    /// <summary>
    /// Длина элемента [символ]
    /// </summary>
    public int Length;


    public string From { get; set; }

    public int FromIndex { get; set; }

    public string To { get; set; }

    public int ToIndex { get; set; }


    public override string ToString()
    {
      return string.Format("{0}, {1} From=\"{2}\", To=\"{3}\"", Index, EditionType, From, To);
    }
  };
}