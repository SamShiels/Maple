using System;
using System.Collections.Generic;
using System.Text;

namespace Maple.Texturing
{
  public class RenderTexture : Texture
  {
    public int Width { get; set; }
    public int Height { get; set; }

    public RenderTexture() : base(null)
    {

    }
  }
}
