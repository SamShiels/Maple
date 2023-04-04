using Firefly.Texturing.Settings;
using System;
using System.Collections.Generic;
using System.Text;

namespace Firefly.Texturing
{
  abstract public class TextureBase
  {
    public static int NextId = 0;

    public int Id { protected set; get; }
    public uint DirtyId { protected set; get; } = 0;

    private TextureWrapMode wrapS;
    public TextureWrapMode WrapS
    {
      get { return wrapS; }
      set
      {
        wrapS = value;
        DirtyId++;
      }
    }

    private TextureWrapMode wrapT;
    public TextureWrapMode WrapT
    {
      get { return wrapT; }
      set
      {
        wrapT = value;
        DirtyId++;
      }
    }

    private TextureMinFilter minificationFilter;
    public TextureMinFilter MinificationFilter
    {
      get { return minificationFilter; }
      set
      {
        minificationFilter = value;
        DirtyId++;
      }
    }
    private TextureMagFilter magnificationFilter;
    public TextureMagFilter MagnificationFilter
    {
      get { return magnificationFilter; }
      set
      {
        magnificationFilter = value;
        DirtyId++;
      }
    }
    public bool UseMipMaps { get; set; }

    public TextureBase()
    {
      DirtyId = 0;

      WrapS = TextureWrapMode.Repeat;
      WrapT = TextureWrapMode.Repeat;
      MinificationFilter = TextureMinFilter.LinearMipmapLinear;
      MagnificationFilter = TextureMagFilter.Linear;
      UseMipMaps = true;
      Id = NextId;
      NextId++;
    }
  }
}
