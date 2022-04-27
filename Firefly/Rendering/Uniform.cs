using System;
using System.Collections.Generic;
using System.Text;

namespace Firefly.Rendering
{
  public class Uniform
  {
    public string name { get; private set; }
    public object data { get; set; }
    public Type dataType { get; private set; }

    public Uniform(string name, object data)
    {
      this.name = name;
      UpdateData(data);
      dataType = data.GetType();
    }

    public void UpdateData(object data)
    {
      this.data = data;
    }
  }
}
