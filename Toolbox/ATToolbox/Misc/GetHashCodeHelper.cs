using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace AT.Toolbox.Misc
{
  public static class GetHashCodeHelper
  {
    private const int Factor = 373;

    public static int CombineHashCodes<T>(int start, int insteadNull, params T[] objects)
    {
      return CombineHashCodesCustom<T>(start, insteadNull, o => o.GetHashCode(), objects);
    }

    public static int CombineHashCodesCustom<T>(int start, int insteadNull, Func<T, int> lambda, params T[] objects)
    {
      int hash = start;
      for (long i = 0L; i < objects.LongLength; ++i)
      {
        var object_ = objects[i];
        hash = unchecked(hash * Factor) ^ (object_ == null ? insteadNull : lambda(object_));
      }
      return hash;
    }

    public static int CombineHashCodesEnum<T>(IEnumerable<T> objects, int start, int insteadNull)
    {
      return CombineHashCodesCustomEnum<T>(objects, start, insteadNull, o => o.GetHashCode());
    }

    public static int CombineHashCodesCustomEnum<T>(IEnumerable<T> objects, int start, int insteadNull, Func<T, int> lambda)
    {
      int hash = start;
      foreach (var object_ in objects)
      {
        hash = unchecked(hash * Factor) ^ (object_ == null ? insteadNull : lambda(object_));
      }
      return hash;
    }

    public static int GetHashCode(this object[] objects, int start, int insteadNull)
    {
      return CombineHashCodes(start, insteadNull, objects);
    }

    public static int GetHashCode(this IEnumerable<object> objects, int start, int insteadNull)
    {
      return CombineHashCodesEnum(objects, start, insteadNull);
    }
  }

  public static class GetEqualsHelper
  {
    public static bool Equals(object[] objectsA, object[] objectsB)
    {
      if (objectsA.LongLength != objectsB.LongLength)
        return false;
      for (long i = 0L; i < objectsA.LongLength; ++i)
      {
        if (!object.Equals(objectsA[i], objectsB[i]))
          return false;
      }
      return true;
    }

    public static bool Equals(IEnumerable<object> objectsA, IEnumerable<object> objectsB)
    {
      IEnumerator<object> enumeratorA = null, enumeratorB = null;
      try
      {
        enumeratorA = objectsA.GetEnumerator();
        enumeratorB = objectsB.GetEnumerator();
        enumeratorA.Reset();
        enumeratorB.Reset();
        bool next_A = true, next_B = true;
        while (next_A || next_B)
        {
          next_A = enumeratorA.MoveNext();
          next_B = enumeratorB.MoveNext();
          if (next_A && next_B)
          {
            if (!object.Equals(enumeratorA.Current, enumeratorB.Current))
              return false;
          }
          else
            if (next_A || next_B)
              return false;
        }
        return true;
      }
      finally
      {
        if (enumeratorA != null)
          enumeratorA.Dispose();

        if (enumeratorB != null)
          enumeratorB.Dispose();
      }
    }
  }
}
