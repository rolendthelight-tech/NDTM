namespace Toolbox.Tasks
{
	/// <summary>
	/// Интерфейс результата.
	/// </summary>
	/// <typeparam name="TResult">Тип возвращаемого результата.</typeparam>
	public interface IResultable<out TResult>
	{
		/// <summary>
		/// Результат.
		/// </summary>
		TResult Result { get; }
	}
}