using System;
using JetBrains.Annotations;

namespace Toolbox.Security.Cryptography
{
  /// <summary>
  /// Помечает свойство или поле как шифруемое
  /// </summary>
  [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
  public sealed class EncryptedAttribute : Attribute { }

  /// <summary>
  /// Класс для шифрования строк через DPAPI.
  /// DPAPI — Data Protection API, встроенный в Windows набор функций, позволяющий шифровать хранимую информацию 320-битным ключом, уникальным для пользователя.
  /// Эти функции представлены классом System.Security.Cryptography.ProtectedData.
  /// </summary>
  public class UserSecurityProvider
  {
    private static readonly log4net.ILog _log = log4net.LogManager.GetLogger(typeof(UserSecurityProvider));
    static byte[] _salt = System.Text.Encoding.Unicode.GetBytes("{4CA6E5B6-332A-49a1-AF5B-2EBC2F53E7FE}");

    /// <summary>
    /// Установка "случайного" контекста шифрования (соль)
    /// </summary>
    /// <param name="salt"></param>
    public void SetSalt([NotNull] string salt)
    {
	    if (salt == null) throw new ArgumentNullException("salt");

			_salt = System.Text.Encoding.Unicode.GetBytes(salt);
    }

	  /// <summary>
    /// Шифрует строку уникальным для пользователя ключом        
    /// </summary>        
    /// <param name="input">Строка для шифрования</param>
    /// <returns>Зашифрованная строка в base64</returns>
    public static string EncryptString([NotNull] string input)
    {
		  if (input == null) throw new ArgumentNullException("input");

			byte[] encrypted_data = System.Security.Cryptography.ProtectedData.Protect(
          System.Text.Encoding.Unicode.GetBytes(input),
          _salt,
          System.Security.Cryptography.DataProtectionScope.CurrentUser);

      return Convert.ToBase64String(encrypted_data);
    }

    /// <summary>
    /// Шифрует строку уникальным для пользователя ключом        
    /// </summary>
    /// <param name="input">Строка для шифрования</param>
    /// <param name="salt">Соль</param>
    /// <returns>Зашифрованная строка в base64</returns>
    public static string EncryptString([NotNull] string input, [NotNull] string salt)
    {
	    if (input == null) throw new ArgumentNullException("input");
	    if (salt == null) throw new ArgumentNullException("salt");

			byte[] encrypted_data = System.Security.Cryptography.ProtectedData.Protect(
          System.Text.Encoding.Unicode.GetBytes(input),
          System.Text.Encoding.Unicode.GetBytes(salt),
          System.Security.Cryptography.DataProtectionScope.CurrentUser);

      return Convert.ToBase64String(encrypted_data);
    }

    /// <summary>
    /// Расшифрует строку, зашифрованную ключом пользователя
    /// </summary>
    /// <param name="encryptedData">Зашифрованная строка в base64</param>
    /// <returns>Расшифровання строка</returns>
    public static string DecryptString([NotNull] string encryptedData)
    {
	    if (encryptedData == null) throw new ArgumentNullException("encryptedData");

			return DecryptString(encryptedData, System.Text.Encoding.Unicode.GetString(_salt));
    }

	  /// <summary>
    /// Расшифрует строку, зашифрованную ключом пользователя
    /// </summary>
    /// <param name="encryptedData">Зашифрованная строка в base64</param>
    /// <param name="salt">Соль</param>
    /// <returns>Расшифровання строка</returns>
    public static string DecryptString([NotNull] string encryptedData, [NotNull] string salt)
    {
	    if (encryptedData == null) throw new ArgumentNullException("encryptedData");
	    if (salt == null) throw new ArgumentNullException("salt");

	    try
      {
        byte[] decrypted_data = System.Security.Cryptography.ProtectedData.Unprotect(
            Convert.FromBase64String(encryptedData),
            System.Text.Encoding.Unicode.GetBytes(salt),
            System.Security.Cryptography.DataProtectionScope.CurrentUser);

        return System.Text.Encoding.Unicode.GetString(decrypted_data);
      }
      catch (Exception ex)
      {
        _log.Error("DecryptString(): Невозможно дешифровать строку", ex);
        return "";
      }
    }
  }
}
