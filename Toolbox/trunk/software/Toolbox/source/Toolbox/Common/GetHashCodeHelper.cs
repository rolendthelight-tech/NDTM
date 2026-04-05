using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using JetBrains.Annotations;
using PureAttribute = System.Diagnostics.Contracts.PureAttribute;

namespace Toolbox.Common
{
  public static class GetHashCodeHelper
  {
    private const int Factor = 373;

		[Pure]
    public static int CombineHashCodes<T>(int start, int insteadNull, [NotNull] params T[] objects)
    {
	    if (objects == null) throw new ArgumentNullException("objects");

			return CombineHashCodesCustom<T>(start, insteadNull, o => o.GetHashCode(), objects);
    }

		[Pure]
		public static int CombineHashCodesCustom<T>(int start, int insteadNull, [NotNull] Func<T, int> lambda, [NotNull] params T[] objects)
    {
	    if (lambda == null) throw new ArgumentNullException("lambda");
		  if (objects == null) throw new ArgumentNullException("objects");

		  int hash = start;
      for (long i = 0L; i < objects.LongLength; ++i)
      {
        var object_ = objects[i];
        hash = unchecked(hash * Factor) ^ (object_ == null ? insteadNull : lambda(object_));
      }
      return hash;
    }

		[Pure]
		public static int CombineHashCodesEnum<T>([NotNull] IEnumerable<T> objects, int start, int insteadNull)
    {
	    if (objects == null) throw new ArgumentNullException("objects");

			return CombineHashCodesCustomEnum<T>(objects, start, insteadNull, o => o.GetHashCode());
    }

		[Pure]
		public static int CombineHashCodesCustomEnum<T>([NotNull] IEnumerable<T> objects, int start, int insteadNull, [NotNull] Func<T, int> lambda)
    {
		  if (objects == null) throw new ArgumentNullException("objects");
		  if (lambda == null) throw new ArgumentNullException("lambda");

		  int hash = start;
      foreach (var object_ in objects)
      {
        hash = unchecked(hash * Factor) ^ (object_ == null ? insteadNull : lambda(object_));
      }
      return hash;
    }

		[Pure]
		public static int GetHashCode(this object[] objects, int start, int insteadNull)
    {
      return CombineHashCodes(start, insteadNull, objects);
    }

		[Pure]
		public static int GetHashCode(this IEnumerable<object> objects, int start, int insteadNull)
    {
      return CombineHashCodesEnum(objects, start, insteadNull);
    }
  }

  public static class GetEqualsHelper
  {
		[Pure]
		public static bool Equals([NotNull] object[] objectsA, [NotNull] object[] objectsB)
    {
	    if (objectsA == null) throw new ArgumentNullException("objectsA");
	    if (objectsB == null) throw new ArgumentNullException("objectsB");

			if (objectsA.LongLength != objectsB.LongLength)
        return false;
      for (long i = 0L; i < objectsA.LongLength; ++i)
      {
        if (!object.Equals(objectsA[i], objectsB[i]))
          return false;
      }
      return true;
    }

	  [Pure]
	  public static bool Equals([NotNull] IEnumerable<object> objectsA, [NotNull] IEnumerable<object> objectsB)
	  {
		  if (objectsA == null) throw new ArgumentNullException("objectsA");
		  if (objectsB == null) throw new ArgumentNullException("objectsB");

		  return objectsA.SequenceEqual(objectsB);
	  }
  }
}
