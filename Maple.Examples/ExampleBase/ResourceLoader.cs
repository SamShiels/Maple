using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace ExampleBase
{
  public class ResourceLoader
  {
    private Assembly assembly;

    public ResourceLoader()
    {
      assembly = Assembly.GetExecutingAssembly();
    }

    public Stream GetResourceStream(string path)
    {
      return assembly.GetManifestResourceStream($"ExampleBase.Resources.{path}");
    }
  }
}
