using System;
using System.Collections.Generic;
using System.Text;

namespace Firefly.Texturing
{
  public class RenderTexture : Texture
  {
    public uint Width { get; set; }
    public uint Height { get; set; }

    public RenderTexture() : base(null)
    {

    }
  }
}
