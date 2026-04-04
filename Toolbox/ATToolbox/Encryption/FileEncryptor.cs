using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AT.Toolbox.Encryption
{
  using System.IO;
  using System.Security.Cryptography;


  public static class FileEncryptor
  {
    const int key_offset = 128;
    const int block_size = 16;
    const int random_blocks = 1;

    private static RijndaelManaged PrepareRijndael(string KeyFileName)
    {
      RijndaelManaged aes = new RijndaelManaged();

      FileStream fs = new FileStream(KeyFileName, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);

      byte[] b = new byte[aes.Key.Length];
      fs.Seek(key_offset, SeekOrigin.Begin);
      fs.Read(b, 0, aes.Key.Length);

      aes.Key = b;

      return aes;
    }

    public static void EncryptFile( string FileName, string KeyFileName )
    {
      RijndaelManaged aes = PrepareRijndael(KeyFileName);
      ICryptoTransform t = aes.CreateEncryptor( );

      string outFile = "test.enc";

      using (FileStream outFs = new FileStream(outFile, FileMode.Create))
      {
        using( CryptoStream outStreamEncrypted = new CryptoStream( outFs, t, CryptoStreamMode.Write ) )
        {
          using (FileStream inFs = new FileStream(FileName, FileMode.Open))
          {
            // By encrypting a chunk at
            // a time, you can save memory
            // and accommodate large files.
            int count = 0;
            
            byte[] data = new byte[block_size];

            Random rnd = new Random(DateTime.Now.Millisecond);

            for (int i = 0; i < random_blocks; i++)
            {
              rnd.NextBytes(data);
              outStreamEncrypted.Write(data, 0, block_size);
            }
            
            do
            {
              count = inFs.Read(data, 0, block_size);
              outStreamEncrypted.Write(data, 0, count);
            }
            while (count > 0);
            
            inFs.Close( );
          }
          outStreamEncrypted.FlushFinalBlock( );
          outStreamEncrypted.Close( );
        }
        outFs.Close( );
      }

      File.Copy(outFile, FileName, true);
      File.Delete(outFile);
    }

    public static void DecryptFile(string FileName, string KeyFileName)
    {
      RijndaelManaged aes = PrepareRijndael(KeyFileName);
      ICryptoTransform t = aes.CreateDecryptor( );

      string outFile = "test.dec";

      using (FileStream inFs = new FileStream( FileName, FileMode.Open ))
      {
        using( FileStream outFs = new FileStream( outFile, FileMode.Create ) )
        {
          int count = 0;
          
          byte[] data = new byte[block_size];
          byte[] data2 = new byte[block_size];

          using (CryptoStream outStreamDecrypted = new CryptoStream(outFs, t, CryptoStreamMode.Write))
          {
            for (int i = 0; i < random_blocks; i++)
            {
              count = inFs.Read(data, 0, block_size);
              outStreamDecrypted.Write(data, 0, count);
            }

            bool first = true;

            outFs.Seek(0, SeekOrigin.Begin);
            do
            {
              count = inFs.Read(data, 0, block_size);
              outStreamDecrypted.Write(data, 0, count);

              if( first )
              {
                outFs.Seek(0, SeekOrigin.Begin);
                first = false;
              }
            }
            while (count > 0);

            outStreamDecrypted.FlushFinalBlock();
            outStreamDecrypted.Close();
          }

          outFs.Close();
        }
        
        inFs.Close( );
      }

      File.Copy(outFile, FileName, true);
      File.Delete( outFile );
    }
  }
}
