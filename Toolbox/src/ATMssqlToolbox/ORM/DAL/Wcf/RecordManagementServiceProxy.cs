using System;
using System.Collections.Generic;
using System.Data;

namespace AT.Toolbox.MSSQL.DAL.RecordBinding.Wcf
{
  public class RecordManagementServiceProxy : ExceptionHandlingProxyBase<IRecordManagementService>, IRecordManagementService
  {
    public RecordManagementServiceProxy(string host_name, int base_port) :
      base("", host_name, base_port)
    {
    }

    //    public RecordManagementServiceProxy(string host_name, int base_port) :
    //      base("", string.Format("net.tcp://{0}:{1}/RecordManagement", host_name, base_port + 1))
    //    {
    //    }

    //public SecurityControlProxy() :
    //  this("")
    //{
    //}

    //public SecurityControlProxy(string endpointConfigurationName) :
    //  base(endpointConfigurationName)
    //{
    //}

    //public SecurityControlProxy(string endpointConfigurationName, string remoteAddress) :
    //  base(endpointConfigurationName, remoteAddress)
    //{
    //}

    //public SecurityControlProxy(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) :
    //  base(binding, remoteAddress)
    //{
    //}

    //    public bool Enabled
    //    {
    //      get { return Invoke<bool>("get_Enabled"); }
    //    }
    //
    //    public bool IsUserRegistered
    //    {
    //      get { return Invoke<bool>("get_IsUserRegistered"); }
    //    }
    //
    //    public bool IsUserAdmin
    //    {
    //      get { return Invoke<bool>("get_IsUserAdmin"); }
    //    }
    //
    //    public QueryObject UpdateQuery(string tableName, QueryObject source)
    //    {
    //      return Invoke<QueryObject>("UpdateQuery", tableName, source);
    //    }
    //
    //    public void SavePermission(int roleID, string tableName, string action, bool? allow)
    //    {
    //      Invoke("SavePermission", roleID, tableName, action, allow);
    //    }
    //
    //    IList<Permission> ISecurityControl.GetDocTypePermissions(string tableName, string userLogin)
    //    {
    //      return Invoke<Permission[]>("GetDocTypePermissions", tableName, userLogin);
    //    }
    //
    //    IList<TablePermission> ISecurityControl.GetTableLevelPermissions()
    //    {
    //      return Invoke<TablePermission[]>("GetTableLevelPermissions");
    //    }
    //
    //    public bool Allowed(string documentType, string[] actions)
    //    {
    //      return Invoke<bool>("Allowed", documentType, actions);
    //    }
    //
    //    public DataSet GetPermissionsByTable(string tableName)
    //    {
    //      return Invoke<DataSet>("GetPermissionsByTable", tableName);
    //    }
    //
    //    public void UpdatePermissionsByTable(DataSet dataSet)
    //    {
    //      Invoke("UpdatePermissionsByTable", dataSet);
    //    }
    //
    //    //    public void SavePermission(string role, Type documentType, string[] actions, bool allow)
    //    //    {
    //    //      throw new NotImplementedException();
    //    //    }
    //
    //    public DataSet GetPermissionsByRole(int roleId)
    //    {
    //      return Invoke<DataSet>("GetPermissionsByRole", roleId);
    //    }
    public void SendClientRequisites(string userName, string computerName, string applicationName)
    {
      Invoke("SendClientRequisites", userName, computerName, applicationName);
    }

    public bool CheckActuality(string tableName, DateTime lastChange)
    {
      return Invoke<bool>("CheckActuality", tableName, lastChange);
    }

    public BindingResult GetResult(Package package, BindingAction action)
    {
      return Invoke<BindingResult>("GetResult", package, action);
    }

    public byte[] ExecuteReaderCommand(CommandType cmd, string commandText, Dictionary<string, object> parameters)
    {
      return Invoke<byte[]>("ExecuteReaderCommand", cmd, commandText, parameters);
    }

    public byte[] InitReader(string tableName, byte[] packedQuery)
    {
      return Invoke<byte[]>("InitReader", tableName, packedQuery);
    }

    public byte[] GetReadyObjects(string tableName, byte[] packedQuery)
    {
      return Invoke<byte[]>("GetReadyObjects", tableName, packedQuery);
    }

    public BindingResult Insert(string tableName, Dictionary<string, object> fieldValues, string[] outputFieldNames)
    {
      return Invoke<BindingResult>("Insert", tableName, fieldValues, outputFieldNames);
    }

    public BindingResult Update(string tableName, Dictionary<string, object> fieldValues, Dictionary<string, object> keyFieldValues)
    {
      return Invoke<BindingResult>("Update", tableName, fieldValues, keyFieldValues);
    }

    public BindingResult Delete(string tableName, Dictionary<string, object> keyFieldValues)
    {
      return Invoke<BindingResult>("Delete", tableName, keyFieldValues);
    }

    public BindingResult UpdateRecordset(string tableName, byte[] packedQuery, Dictionary<string, object> parameters)
    {
      return Invoke<BindingResult>("UpdateRecordset", tableName, packedQuery, parameters);
    }

    public BindingResult DeleteRecordSet(string tableName, byte[] packedQuery)
    {
      return Invoke<BindingResult>("DeleteRecordSet", tableName, packedQuery);
    }
  }
}