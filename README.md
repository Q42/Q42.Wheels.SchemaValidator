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

Cmd
===
While this package is mainly meant to be integrated (via NuGet) into other software, 
I've added a commandline tool just as an example how to use it. However, the example works pretty well
and you could use it to validate files on disk with it:
```
SchemaValidator.exe [-q] path-to-xsd path-to-xml
```

The exit value for invalid XML syntax is always 1.
For schema validation errors (this implies that there are no XML syntax errors) the exit value
is equal to the number of validation errors found.

On other errors (file not found, etc.) the exit value is 255
