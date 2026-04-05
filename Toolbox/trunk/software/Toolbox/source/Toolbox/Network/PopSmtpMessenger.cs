using System.Runtime.Serialization;
using Toolbox.Application;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;

namespace Toolbox.Network
{
  [Serializable]
  public class MailException : Exception
  {
    public MailException( string s )
      : base( s ) { }
  }

  public class EmailMessage
  {
    public string From;

    public string To;

    public string Cc;

    public string Subj;

    public string Date;

    public string MimeVer;

    public List<string> Boundaries = new List<string>( );

    public Dictionary<string, string> Contents = new Dictionary<string, string>( );
  }
}