using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Schema;

namespace Q42.Wheels.SchemaValidator
{
  public class SchemaValidator
  {
    private readonly XmlReaderSettings settings;
    private readonly List<SchemaError> validationErrors;
    private string topLevelElement;

    // creation
    private SchemaValidator(XmlReaderSettings settings)
    {
      this.settings = settings;
      validationErrors = new List<SchemaError>();
    }

    public static SchemaValidator Create(string schemaUri)
    {
      return Create(null, schemaUri);
    }

    public static SchemaValidator Create(string targetNamespace, string schemaUri)
    {
      if (!File.Exists(schemaUri))
        throw new FileNotFoundException("Schema not found from uri " + schemaUri);

      try
      {
        var set = new XmlSchemaSet();
        set.Add(targetNamespace, schemaUri);

        var settings = new XmlReaderSettings { Schemas = set, ValidationType = ValidationType.Schema };

        var validator = new SchemaValidator(settings);
        settings.ValidationEventHandler += validator.GenericValidationEventHandler;
        return validator;
      }
      catch (Exception ex)
      {
        throw new ArgumentException("Unable to load " + schemaUri, ex);
      }
    }

    public void SetTopLevelElement(string elementName)
    {
      topLevelElement = elementName;
    }

    /// <summary>
    /// Validates the document.
    /// </summary>
    /// <remarks>
    /// NOTE: you should load the xmlDocument with
    /// <code>XmlDocument.PreserveWhitespace = true</code>
    /// This will prevent validation problems with insignificant whitespace.
    /// For example an element with simpleContent type = xs:string with restriction 
    /// on length or regex, allowing an empty string:
    /// This case will be affected if the content is like: <code>&lt;tag&gt;&lt;/tag&gt;</code>
    /// (no content only open and close tags).
    /// 
    /// We need this because we want to XmlDocument.Save to a Memory stream to get nice line-number positions.
    /// </remarks>
    /// <param name="xmlDocument"></param>
    /// <returns></returns>
    public SchemaError[] Validate(XmlDocument xmlDocument)
    {
      using (MemoryStream stream = new MemoryStream())
      {
        xmlDocument.Save(stream);
        stream.Position = 0;
        return Validate(stream);
      }
    }

    public SchemaError[] Validate(Stream xmlStream)
    {
      try
      {
        using (var xmlReader = XmlReader.Create(xmlStream, settings))
        {
          return DoValidation(xmlReader);
        }
      }
      catch (XmlException ex)
      {
        return new[] { GetXmlSyntaxError(ex) };
      }
    }

    public SchemaError[] Validate(TextReader reader)
    {
      try
      {
        using (var xmlReader = XmlReader.Create(reader, settings))
        {
          return DoValidation(xmlReader);
        }
      }
      catch (XmlException ex)
      {
        return new[] { GetXmlSyntaxError(ex) };
      }
    }

    public SchemaError[] Validate(string xmlUri)
    {
      try
      {
        using (var xmlReader = XmlReader.Create(xmlUri, settings))
        {
          return DoValidation(xmlReader);
        }
      }
      catch (XmlException ex)
      {
        return new[] { GetXmlSyntaxError(ex) };
      }
    }

    private SchemaError[] DoValidation(XmlReader reader)
    {
      bool seenFirstElement = false;
      while (reader.Read())
      {
        if (!seenFirstElement && reader.NodeType == XmlNodeType.Element)
        {
          seenFirstElement = true;
          if (topLevelElement != null && reader.LocalName != topLevelElement)
          {
            var lineInfo = reader as IXmlLineInfo;
            if (lineInfo != null && lineInfo.HasLineInfo())
              validationErrors.Add(new SchemaError(lineInfo.LineNumber, lineInfo.LinePosition, "First element should be: " + topLevelElement, "", false));
            else
              validationErrors.Add(new SchemaError(1, 1, "First element should be: " + topLevelElement, "", false));
          }
        }
      }
      SchemaError[] errors = validationErrors.ToArray();
      validationErrors.Clear();
      return errors;
    }

    // error collector
    private void GenericValidationEventHandler(object sender, ValidationEventArgs args)
    {
      validationErrors.Add(new SchemaError(args.Exception.LineNumber, args.Exception.LinePosition, args.Exception.Message, args.Exception.SourceUri, false));
    }

    private SchemaError GetXmlSyntaxError(XmlException ex)
    {
      return new SchemaError(ex.LineNumber, ex.LinePosition, ex.Message, "", true);
    }

  }
}
