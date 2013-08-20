using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Q42.Wheels.SchemaValidator;

namespace SchemaValidatorExample
{
  public static class Program
  {
    public static void Usage()
    {
      Console.WriteLine("Usage:");
      Console.WriteLine("SchemaValidator.exe [-v] path-to-xsd path-to-xml");
    }

    public static void Main(string[] args)
    {
      List<string> argsList = new List<string>(args);
      bool quiet = argsList.Any(IsQuietFlag);

      if (quiet)
      {
        argsList.RemoveAll(IsQuietFlag);
      }

      if (argsList.Count != 2)
      {
        Usage();
        return;
      }

      try
      {
        var xsdFile = argsList[0];
        var xmlFile = argsList[1];

        var validator = SchemaValidator.Create(xsdFile);

        SchemaError[] errors;
        using (var stream = File.OpenRead(xmlFile))
        {
          errors = validator.Validate(stream);
        }

        if (!quiet)
        {
          foreach (var error in errors)
          {
            Console.WriteLine("Line {0} column {1}: {2}", error.LineNumber, error.LinePosition, error.Message);
          }
        }

        System.Environment.Exit(errors.Length);
      }
      catch (Exception e)
      {
        Console.Error.WriteLine(e.GetType().Name);
        Console.Error.WriteLine(e.Message);
        Console.Error.WriteLine(e.StackTrace);
        System.Environment.Exit(255);
      }
    }

    private static bool IsQuietFlag(string s)
    {
      return s == "-q" || s == "--quiet";
    }
  }
}
