Q42.Wheels.SchemaValidator
==========================

A "wheel" that allows you to validate an xml document against a schema.

The `SchemaValidator` uses .NET's `XmlReaderSettings` to setup an `XmlReader` to capture schema errors.
XML syntax errors are also captured by the `SchemaValidator`.

Usage
=====
```C#
SchemaValidator validator = SchemaValidator.Create(pathToXsd);
using (var reader = new StringReader(xml))
{
  SchemaError[] errors = validator.Validate(reader);
}
```
