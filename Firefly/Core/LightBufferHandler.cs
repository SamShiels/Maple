using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics.OpenGL4;

namespace Firefly.Core
{
  public class LightBufferHandler
  {
    private int bufferHandler;

    public LightBufferHandler()
    {
      bufferHandler = GL.GenBuffer();

    }
  }
}
