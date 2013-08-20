using System;
using System.Text;

namespace Q42.Wheels.SchemaValidator
{
  /// <summary>
  /// Class for storing schema error information
  /// </summary>
  public class SchemaValidationException : ApplicationException
  {
    private SchemaError[] errors;
    public SchemaValidationException(string s, SchemaError[] errors)
      : base(s)
    {
      this.errors = errors;
    }

    /// <summary>
    /// Also show the errors in the log
    /// </summary>
    public override string Message
    {
      get
      {
        StringBuilder msg = new StringBuilder();
        msg.Append(base.Message);

        if (errors != null && errors.Length > 0)
        {
          msg.Append(Environment.NewLine + "The following errors were found:");
          foreach (SchemaError e in errors)
            msg.Append(Environment.NewLine + e.ToString());
        }

        return msg.ToString();
      }
    }
  }
}
