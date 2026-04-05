namespace Toolbox.Tasks
{
	/// <summary>
	/// Интерфейс успешности операции.
	/// </summary>
	public interface ISuccessable
	{
		/// <summary>
		/// Успешность.
		/// </summary>
		bool Success { get; }
	}
}