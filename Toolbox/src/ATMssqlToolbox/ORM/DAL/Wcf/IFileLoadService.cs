using System;
using System.ServiceModel;

namespace AT.Toolbox.MSSQL.ORM.DAL.Wcf
{
  [ServiceContract]
  public interface IFileLoadService : IDisposable
  {
//    [OperationContract]
//    bool FileExists(Guid FileGuid);
    [OperationContract]
    void CheckAvailable();

//    [OperationContract]
//    int StartDownload(Guid FileGuid);

    [OperationContract]
    byte[] ReadFile(Guid file_guid, int offset, int bytes_to_read);

//    [OperationContract]
//    void WriteFile(Guid file_guid, int offset, byte[] data);

//    [OperationContract]
//    int StartDownload(Guid FileGuid);
//
//    [OperationContract]
//    byte[] ReadByteArray(int bytes_to_read);
//
//    [OperationContract]
//    void FinishDownload();
//
//    [OperationContract]
//    void StartUpload(Guid FileGuid);
//
//    [OperationContract]
//    void WriteByteArray(byte[] data);
//
//    [OperationContract]
//    void FinishUpload();
  }
}