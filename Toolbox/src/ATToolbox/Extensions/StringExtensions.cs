using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace AT.Toolbox.Extensions
{
  public static class StringExtensions
  {
    public static bool ContainsAnyOf(this string str, string rw)
    {
      foreach( char c in rw )
      {
        if (str.Contains(c))
          return true;
      }

      return false;
    }

    public static StringToken[] SimpleTokenize(this string Str )
    {
      StringToken current_token = new StringToken( );
      List<StringToken> ret_val = new List<StringToken>( );

      foreach( char c in Str )
      {
        if( current_token.Category != char.GetUnicodeCategory( c ) )
        {
          if( !string.IsNullOrEmpty( current_token.Value ) )
            ret_val.Add( current_token );

          current_token = new StringToken( );
          current_token.Category = char.GetUnicodeCategory( c );
          current_token.Value = new string( c, 1 );
        }
        else
          current_token.Value += c.ToString( );
      }

      if( !string.IsNullOrEmpty( current_token.Value ) )
        ret_val.Add( current_token );

      return ret_val.ToArray();
    }
  }

  public class StringToken
  {
    public StringToken()
    {
      Category = default( UnicodeCategory );
      Value = "";
    }

    public string Value { get; set; }
    public UnicodeCategory Category { get; set; }
  }
}
