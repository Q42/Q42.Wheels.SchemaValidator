using System.IO;
using System.Xml;
using System.Xml.Xsl;

namespace Q42.Wheels.SchemaValidator
{
  public static class Transformer
  {

    public static XslCompiledTransform GetXslCompiledTransform(string input)
    {
      XslCompiledTransform xslTransform = new XslCompiledTransform();
      XmlDocument xslDoc = new XmlDocument();
      if (input.StartsWith("<"))
        xslDoc.LoadXml(input);
      else
        xslDoc.Load(input);
      xslTransform.Load(xslDoc.CreateNavigator());
      return xslTransform;
    }

    public static XmlDocument Transform(XmlDocument xml, XslCompiledTransform xsl, XsltArgumentList args)
    {
      XmlDocument doc = new XmlDocument();

      using (MemoryStream stream = new MemoryStream())
      {
        XmlWriter writer = XmlWriter.Create(stream);

        if (args == null)
          xsl.Transform(xml, writer);
        else
          xsl.Transform(xml, args, writer);

        writer.Close();
        stream.Seek(0, SeekOrigin.Begin);

        doc.Load(XmlReader.Create(stream));
      }
      return doc;
    }
  
  }
}