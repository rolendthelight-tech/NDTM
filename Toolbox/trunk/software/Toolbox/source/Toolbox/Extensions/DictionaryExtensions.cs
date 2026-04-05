using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using Toolbox.Tasks;
using PureAttribute = System.Diagnostics.Contracts.PureAttribute;

namespace Toolbox.Extensions
{
	public static class DictionaryExtensions
	{
		[NotNull]
		[Pure]
		public static ISuccessResult<TValue> TryGetValue<TKey, TValue>([NotNull] this IDictionary<TKey, TValue> dictionary, TKey key)
		{
			if (dictionary == null) throw new ArgumentNullException("dictionary");

			TValue value;
			if (dictionary.TryGetValue(key: key, value: out value))
				return SuccessResult<TValue>.Create(value);
			else
				return FailResult<TValue>.Create();
		}

		/// <summary>
		/// Удаляет указанный ключ из словаря.
		/// </summary>
		/// <typeparam name="TKey">Тип ключа словаря.</typeparam>
		/// <typeparam name="TValue">Тип значений словаря.</typeparam>
		/// <param name="dictionary">Словарь.</param>
		/// <param name="key">Подлежащий удалению ключ.</param>
		public static void CheckedRemove<TKey, TValue>([NotNull] this IDictionary<TKey, TValue> dictionary, TKey key)
		{
			if (dictionary == null) throw new ArgumentNullException("dictionary");

			if (!dictionary.Remove(key))
				throw new ArgumentException("Такого ключа не существует в словаре", "key");
		}

    /// <summary>
    /// Получение значения из словаря или добавление нового, если оно отсутствовало в словаре
    /// </summary>
    /// <typeparam name="TKey">Тип ключа словаря.</typeparam>
    /// <typeparam name="TValue">Тип значений словаря.</typeparam>
    /// <param name="dictionary">Словарь.</param>
    /// <param name="key">Ключ для получения значения</param>
    /// <returns></returns>
    public static TValue GetOrCreate<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key)
      where TValue : new()
    {
      TValue val;

      if (!dictionary.TryGetValue(key, out val))
      {
        val = new TValue();
        dictionary.Add(key, val);
      }

      return val;
    }
	}
}
