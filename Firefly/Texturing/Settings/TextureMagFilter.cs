using System;
using System.Collections.Generic;
using System.Text;

namespace Firefly.Texturing.Settings
{
  public enum TextureMagFilter
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
    //     Original was GL_LINEAR_DETAIL_SGIS = 0x8097
    LinearDetailSgis = 32919,
    //
    // Summary:
    //     Original was GL_LINEAR_DETAIL_ALPHA_SGIS = 0x8098
    LinearDetailAlphaSgis = 32920,
    //
    // Summary:
    //     Original was GL_LINEAR_DETAIL_COLOR_SGIS = 0x8099
    LinearDetailColorSgis = 32921,
    //
    // Summary:
    //     Original was GL_LINEAR_SHARPEN_SGIS = 0x80AD
    LinearSharpenSgis = 32941,
    //
    // Summary:
    //     Original was GL_LINEAR_SHARPEN_ALPHA_SGIS = 0x80AE
    LinearSharpenAlphaSgis = 32942,
    //
    // Summary:
    //     Original was GL_LINEAR_SHARPEN_COLOR_SGIS = 0x80AF
    LinearSharpenColorSgis = 32943,
    //
    // Summary:
    //     Original was GL_FILTER4_SGIS = 0x8146
    Filter4Sgis = 33094,
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
    PixelTexGenQFloorSgix = 33158
  }
}
