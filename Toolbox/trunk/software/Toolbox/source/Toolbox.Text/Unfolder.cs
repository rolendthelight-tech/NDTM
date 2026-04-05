using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Toolbox.Text
{
  public class Unfolder
  {
    public static bool Unfold(string foldedString, out List<string> unfoldedStrings)
    {
      var openIndexes = new List<int>();
      var closeIndexes = new List<int>();

      for (int i = 0; i < foldedString.Length; i++)
      {
        if (foldedString[i] == '{')
          openIndexes.Add(i);
        else if (foldedString[i] == '}')
          closeIndexes.Add(i);
      }

      if (openIndexes.Count != closeIndexes.Count)
        throw new Exception();

      var tokens = new List<Token>();

      if (openIndexes.Count == 0)
        tokens.Add(new InvariantToken(foldedString));
      for (int i = 0; i < openIndexes.Count; i++)
      {
        if (openIndexes[i] > closeIndexes[i])
          throw new Exception();

        if (i == 0 && openIndexes[i] > 0)
          tokens.Add(new InvariantToken(foldedString.Substring(0, openIndexes[i])));

        tokens.Add(new VariantToken(foldedString.Substring(openIndexes[i] + 1,
          (closeIndexes[i] - openIndexes[i] - 1))));

        if (closeIndexes[i] < foldedString.Length - 1)
        {
          int startIdx = closeIndexes[i] + 1;
          int length = foldedString.Length - startIdx;

          if (i < openIndexes.Count - 1)
            length = openIndexes[i + 1] - startIdx;

          tokens.Add(new InvariantToken(foldedString.Substring(startIdx, length)));
        }
      }

      unfoldedStrings = new List<string>();

      var processed = new List<List<string>>();
      processed.Add(new List<string>());

      for (int i = 0; i < tokens.Count; i++)
      {
        var bifurcations = new List<List<string>>();
        var variantes = tokens[i].GetVariantes(';');

        if (variantes.Length == 0)
          continue;

        // Во все формирующиеся цепочки добавляем первый вариант
        foreach (var chain in processed)
          chain.Add(variantes[0]);

        // Если есть другие варианты, для каждого из них
        // требуется создать копии всех формирующихся цепочек
        for (int j = 1; j < variantes.Length; j++)
        {
          foreach (var oldChain in processed)
          {
            var newChain = new List<string>();

            for (int k = 0; k < oldChain.Count - 1; k++)
              newChain.Add(oldChain[k]);

            newChain.Add(variantes[j]);

            bifurcations.Add(newChain);
          }
        }

        // Копии цепочек создаём через специальную временную переменную,
        // чтобы в ходе обработки одного набора вариантов не делать
        // лишние копии, когда в общий список уже добавлены копии для предыдущего варианта
        processed.AddRange(bifurcations);
      }

      foreach (var line in processed)
      {
        var str = string.Join(" ", line.ToArray());
        str = Regex.Replace(str, @"\s+", " ");
        str = Regex.Replace(str, @"# ", "");
        unfoldedStrings.Add(str);
      }
      return true;
    }

    private abstract class Token
    {
      private readonly string _value;

      public Token(string value)
      {
        if (string.IsNullOrEmpty(value))
          throw new ArgumentNullException("value");

        _value = value;
      }

      protected string Value
      {
        get { return _value; }
      }

      public abstract string[] GetVariantes(char separator);

      public override string ToString()
      {
        return _value;
      }
    }

    private sealed class InvariantToken : Token
    {
      public InvariantToken(string value) : base(value) { }

      public override string[] GetVariantes(char separator)
      {
        return new[] { Value };
      }
    }

    private sealed class VariantToken : Token
    {
      public VariantToken(string value) : base(value) { }

      public override string[] GetVariantes(char separator)
      {
        return Value.Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries);
      }
    }
  }
}
