using System;
using AT.Toolbox.MSSQL.DAL.RecordBinding.Wcf;

namespace AT.Toolbox.MSSQL.ORM.DAL.Wcf
{
  public class FileLoadServiceProxy : ExceptionHandlingProxyBase<IFileLoadService>, IFileLoadService
  {
    public FileLoadServiceProxy(string hostName, int basePort) :
      //base("", string.Format("net.tcp://{0}:{1}/FileLoadService", hostName, basePort + 3))
      base("", hostName, basePort)
    {
    }

    public void CheckAvailable()
    {
      this.Invoke("CheckAvailable");
    }

    public byte[] ReadFile(Guid file_guid, int offset, int bytes_to_read)
    {
      return Invoke<byte[]>("ReadFile", file_guid, offset, bytes_to_read);
    }

//    public void WriteFile(Guid file_guid, int offset, byte[] data)
//    {
//    }

//    public int StartDownload(Guid FileGuid)
//    {
//      return Invoke<int>("StartDownload", FileGuid);
//    }
//
//    public byte[] ReadByteArray(int bytes_to_read)
//    {
//      return Invoke<byte[]>("ReadByteArray", bytes_to_read);
//    }
//
//    public void FinishDownload()
//    {
//      Invoke("FinishDownload");
//    }
//
//    public void StartUpload(Guid FileGuid)
//    {
//      Invoke("StartUpload", FileGuid);
//    }
//
//    public void WriteByteArray(byte[] data)
//    {
//      Invoke("WriteByteArray", data);
//    }
//
//    public void FinishUpload()
//    {
//      Invoke("FinishUpload");
//    }
  }
}