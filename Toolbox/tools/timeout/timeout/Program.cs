using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace timeout
{
	internal class Program
	{
		public static int Main(string[] args)
		{
			try
			{
				if (args.Length < 2)
				{
					Console.Error.WriteLine("Синтаксис: timeout задержка_в_миллисекундах команда [аргумент]...");
					return 1;
				}
				else
				{
					int timeout;
					if (!int.TryParse(args[0], out timeout))
					{
						Console.Error.WriteLine("Не удалость распознать число");
						return 2;
					}
					else
					{
						Process process = null;
						try
						{
							ProcessStartInfo process_start_info;
							if (args.Length < 3)
							{
								process_start_info = new ProcessStartInfo(args[1]);
							}
							else
							{
								string process_args;
								process_args = "\"" + args[2] + "\"";
								for (int i = 3; i < args.Length; ++i)
									process_args += "\t\"" + args[i] + "\"";

								process_start_info = new ProcessStartInfo(args[1], process_args);
							}
							process_start_info.CreateNoWindow = false;
							process_start_info.UseShellExecute = false;

							Console.CancelKeyPress +=
								(object sender, ConsoleCancelEventArgs e) =>
									{
										try
										{
											if (process != null)
											{
												Console.Error.WriteLine("Прерывание: {0}", DateTime.Now);
												process.Kill();
												process = null;
											}
										}
										catch(Exception ex)
										{
											Console.Error.WriteLine("Ошибка при прерывании процесса: {0}", ex.ToString());
										}
									};

							process = Process.Start(process_start_info);
							Console.WriteLine("Старт: {0}", process.StartTime);
							if (process.WaitForExit(timeout))
							{
								Console.WriteLine("Завершение: {0} с кодом возврата {1}", process.ExitTime, process.ExitCode);
								Console.WriteLine("Команда выполнилась за {0}", process.ExitTime - process.StartTime);
								var code = process.ExitCode;
								process = null;
								return code;
							}
							else
							{
								Console.WriteLine("Принудительное завершение: {0}", DateTime.Now);
								process.Kill();
								process = null;
								return 2;
							}
						}
						finally
						{
							if (process != null)
							{
								Console.WriteLine("Завершение из-за ошибки: {0}", DateTime.Now);
								process.Kill();
								process = null;
							}
						}
					}
				}
			}
			catch(Exception ex)
			{
				Console.Error.WriteLine("Ошибка в программе: {0}", ex.ToString());
				return 3;
			}
		}
	}
}
