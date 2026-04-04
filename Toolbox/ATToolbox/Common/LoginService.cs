using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AT.Toolbox
{
  public interface ILoginService
  {
    bool GetLoginAndPassword(string endpointConfigurationName, string hostName, int basePort, out UserCredentials crd);
  }

  internal class LoginServiceStub : ILoginService
  {
    public bool GetLoginAndPassword(string endpointConfigurationName, string hostName, int basePort, out UserCredentials crd)
    {
      throw new NotImplementedException();
    }
  }

  public class UserCredentials
  {
    public string Login { get; set; }
    public string Password { get; set; }
  }

  public interface ILoginServiceView : ISynchronizeProvider
  {
    bool ShowAuthentificationForm(out UserCredentials crd);
  }
}
