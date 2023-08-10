using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace Maple.Utilities
{
  public class ResourceHelpers
  {
    public static Stream GetResourceStream(string resourcePath)
    {
      return Assembly.GetExecutingAssembly().GetManifestResourceStream(resourcePath);
    }
  }
}
