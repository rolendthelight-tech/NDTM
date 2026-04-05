using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Deployment.Application;
using System.Linq;
using System.Web;
using JetBrains.Annotations;
using Toolbox.Application;
using Toolbox.Extensions;

namespace Toolbox.GUI.Deployment.ClickOnce.Start
{
	public class ClickOnceCommandLineArgs
	{
		/// <summary>
		/// Определяет является ли экземпляр приложения приложением ClickOnce
		/// </summary>
		/// <returns>Экземпляр приложения является приложением ClickOnce</returns>
		public static bool IsClickOnce()
		{
			return ApplicationDeployment.IsNetworkDeployed;
		}

		/// <summary>
		/// Ключи запуска
		/// </summary>
		/// <returns>Массив ключей запуска</returns>
		[NotNull]
		public static string[] ArgsKey()
		{
			var activation_uri = ApplicationDeployment.CurrentDeployment.ActivationUri;
			if (activation_uri != null)
			{
				string queryString = activation_uri.Query;
				NameValueCollection nameValueTable = HttpUtility.ParseQueryString(queryString);
				var args = new string[nameValueTable.Count];
				int i = 0;
				foreach (var kv in nameValueTable)
				{
					args[i++] = kv == null ? string.Empty : kv.ToString();
				}
				return args;
			}
			else
			{
				return ActivationDataKeys() ?? ArrayExtensions.Empty<string>();
			}
		}

		/// <summary>
		/// Ключи со значениями и аргументы запуска (порядок ключей не сохраняется, порядок значений сохраняется, порядок аргументов сохраняется, аргументы не обязательно в конце)
		/// </summary>
		/// <returns>Массив ключей со значениями (разделены знаком "=") и аргументов запуска</returns>
		[NotNull]
		[Obsolete("Не следует использовать этот метод для использования результата в классе CommandLineArgs. Вместо этого следует использовать метод Args()")]
		public static string[] ArgsKeyValue()
		{
			var activation_uri = ApplicationDeployment.CurrentDeployment.ActivationUri;
			if (activation_uri != null)
			{
				string queryString = activation_uri.Query;
				NameValueCollection nameValueTable = HttpUtility.ParseQueryString(queryString);
				var args = new List<string>();
				foreach (var name in nameValueTable)
				{
					var key = name == null ? null : name.ToString();
					var values = nameValueTable.GetValues(key);

					if (values == null)
					{
						// TODO: Писать в лог
					}
					else
					{
						if (name == null)
							args.AddRange(values);
						else
						{
							if (values.Length == 0)
								args.Add(key ?? string.Empty);
							else
							{
								object name1 = name;
								args.AddRange(values.Select(val => string.Format("{0}={1}", name1, val)));
							}
						}
					}
				}
				return args.ToArray();
			}
			else
			{
				return ActivationData() ?? ArrayExtensions.Empty<string>();
			}
		}

		/// <summary>
		/// Возвращает аргументы командной строки
		/// </summary>
		/// <returns>Аргументы командной строки</returns>
		[NotNull]
		public static CommandLineArgs Args()
		{
			var activation_uri = ApplicationDeployment.CurrentDeployment.ActivationUri;
			if (activation_uri != null)
			{
				string queryString = activation_uri.Query;
				NameValueCollection nameValueTable = HttpUtility.ParseQueryString(queryString);
				return new CommandLineArgs(ToLookup(nameValueTable));
			}
			else
			{
				return new CommandLineArgs(ToLookup(ActivationData()));
			}
		}

		[NotNull]
		private static ILookup<string,string> ToLookup([NotNull] NameValueCollection nameValueTable)
		{
			if (nameValueTable == null) throw new ArgumentNullException("nameValueTable");

			var args = new Dictionary<string, IEnumerable<string>>();
			foreach (var name in nameValueTable)
			{
				var key = name == null ? string.Empty : name.ToString();
				args.Add(key, nameValueTable.GetValues(key));
			}
			var keys = args
				.SelectMany(kv => kv.Value.Select(value => new {Key = kv.Key, Value = value}))
				.ToLookup(kv => kv.Key, kv => kv.Value);
			return keys;
		}

		[NotNull]
		private static ILookup<string, string> ToLookup([NotNull] params string[] parameters)
		{
			if (parameters == null) throw new ArgumentNullException("parameters");

			return parameters.ToLookup(p => string.Empty, p => p);
		}

		[NotNull]
		[Obsolete("Этот метод работает плохо и коряво. Не используйте его.", true)]
		public static string[] Args1()
		{
			string[] activationData = AppDomain.CurrentDomain.SetupInformation.ActivationArguments.ActivationData; // Берётся параметр, предназначенный для активации приложения и используется для получения параметров запуска
			return activationData == null || activationData.Length < 1
			       	? ArrayExtensions.Empty<string>()
			       	: activationData[0].Split(new char[] {','});
		}

		[NotNull]
		private static string[] ActivationData()
		{
			var cd = AppDomain.CurrentDomain;
			var si = cd.SetupInformation;
			var aa = si.ActivationArguments;
			var ad = aa.ActivationData;

			if (ad != null && ad.Length >= 1)
			{
				var ad1 = ad[0]; // Используется только первый параметр, параметры Url не разбираются, так как это не Url

				var ai = aa.ApplicationIdentity;
				var cb = ai.CodeBase;

				if (ad1 == cb)
					return ArrayExtensions.Empty<string>(); // Параметр предназначен для активации
				else
					return new string[] {ad1};
			}
			else
			{
				return ArrayExtensions.Empty<string>();
			}
		}

		[NotNull]
		private static string[] ActivationDataKeys()
		{
			var cd = AppDomain.CurrentDomain;
			var si = cd.SetupInformation;
			var aa = si.ActivationArguments;
			var ad = aa.ActivationData;

			if (ad != null && ad.Length >= 1)
			{
				var ad1 = ad[0]; // Используется только первый параметр, параметры Url не разбираются, так как это не Url

				var ai = aa.ApplicationIdentity;
				var cb = ai.CodeBase;

				if (ad1 == cb)
					return ArrayExtensions.Empty<string>(); // Параметр предназначен для активации
				else
					return new string[] {string.Empty};
			}
			else
			{
				return ArrayExtensions.Empty<string>();
			}
		}
	}
}
