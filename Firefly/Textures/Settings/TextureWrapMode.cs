using System;
using System.Collections.Generic;
using System.Text;

namespace Renderer.Textures.Settings
{
  public enum TextureWrapMode
  {
    //
    // Summary:
    //     Original was GL_REPEAT = 0x2901
    Repeat = 10497,
    //
    // Summary:
    //     Original was GL_CLAMP_TO_BORDER = 0x812D
    ClampToBorder = 33069,
    //
    // Summary:
    //     Original was GL_CLAMP_TO_BORDER_ARB = 0x812D
    ClampToBorderArb = 33069,
    //
    // Summary:
    //     Original was GL_CLAMP_TO_BORDER_NV = 0x812D
    ClampToBorderNv = 33069,
    //
    // Summary:
    //     Original was GL_CLAMP_TO_BORDER_SGIS = 0x812D
    ClampToBorderSgis = 33069,
    //
    // Summary:
    //     Original was GL_CLAMP_TO_EDGE = 0x812F
    ClampToEdge = 33071,
    //
    // Summary:
    //     Original was GL_CLAMP_TO_EDGE_SGIS = 0x812F
    ClampToEdgeSgis = 33071,
    //
    // Summary:
    //     Original was GL_MIRRORED_REPEAT = 0x8370
    MirroredRepeat = 33648
  }
}
