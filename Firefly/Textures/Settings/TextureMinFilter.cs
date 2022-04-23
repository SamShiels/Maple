using System;
using System.Collections.Generic;
using System.Text;

namespace Renderer.Textures
{
  //
  // Summary:
  //     Not used directly.
  public enum TextureMinFilter
  {
    //
    // Summary:
    //     Original was GL_NEAREST = 0x2600
    Nearest = 9728,
    //
    // Summary:
    //     Original was GL_LINEAR = 0x2601
    Linear = 9729,
    //
    // Summary:
    //     Original was GL_NEAREST_MIPMAP_NEAREST = 0x2700
    NearestMipmapNearest = 9984,
    //
    // Summary:
    //     Original was GL_LINEAR_MIPMAP_NEAREST = 0x2701
    LinearMipmapNearest = 9985,
    //
    // Summary:
    //     Original was GL_NEAREST_MIPMAP_LINEAR = 0x2702
    NearestMipmapLinear = 9986,
    //
    // Summary:
    //     Original was GL_LINEAR_MIPMAP_LINEAR = 0x2703
    LinearMipmapLinear = 9987,
    //
    // Summary:
    //     Original was GL_FILTER4_SGIS = 0x8146
    Filter4Sgis = 33094,
    //
    // Summary:
    //     Original was GL_LINEAR_CLIPMAP_LINEAR_SGIX = 0x8170
    LinearClipmapLinearSgix = 33136,
    //
    // Summary:
    //     Original was GL_PIXEL_TEX_GEN_Q_CEILING_SGIX = 0x8184
    PixelTexGenQCeilingSgix = 33156,
    //
    // Summary:
    //     Original was GL_PIXEL_TEX_GEN_Q_ROUND_SGIX = 0x8185
    PixelTexGenQRoundSgix = 33157,
    //
    // Summary:
    //     Original was GL_PIXEL_TEX_GEN_Q_FLOOR_SGIX = 0x8186
    PixelTexGenQFloorSgix = 33158,
    //
    // Summary:
    //     Original was GL_NEAREST_CLIPMAP_NEAREST_SGIX = 0x844D
    NearestClipmapNearestSgix = 33869,
    //
    // Summary:
    //     Original was GL_NEAREST_CLIPMAP_LINEAR_SGIX = 0x844E
    NearestClipmapLinearSgix = 33870,
    //
    // Summary:
    //     Original was GL_LINEAR_CLIPMAP_NEAREST_SGIX = 0x844F
    LinearClipmapNearestSgix = 33871
  }
}
