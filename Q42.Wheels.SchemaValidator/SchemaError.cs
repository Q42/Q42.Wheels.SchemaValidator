namespace Q42.Wheels.SchemaValidator
{
  /// <summary>
  /// Class for storing schema error information
  /// </summary>
  public class SchemaError
  {
    public readonly int LineNumber;
    public readonly int LinePosition;
    public readonly string Message;
    public readonly string SourceUri;
    public bool XmlSyntaxError;

    public SchemaError(int lineNumber, int linePosition, string message, string sourceUri, bool xmlSyntaxError)
    {
      LineNumber = lineNumber;
      LinePosition = linePosition;
      Message = message;
      SourceUri = sourceUri;
      XmlSyntaxError = xmlSyntaxError;
    }

    public override string ToString()
    {
        return string.Format("{0}[line {1}:Position {2}] {3}", XmlSyntaxError ? "XmlSyntaxError" : "", LineNumber, LinePosition, Message);
    }
  }
}
