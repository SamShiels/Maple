using System;
using System.Collections.Generic;
using System.Text;

namespace Maple.Rendering
{
  public enum DepthFunction
  {
    //
    // Summary:
    //     Original was GL_NEVER = 0x0200
    Never = 0x200,
    //
    // Summary:
    //     Original was GL_LESS = 0x0201
    Less = 513,
    //
    // Summary:
    //     Original was GL_EQUAL = 0x0202
    Equal = 514,
    //
    // Summary:
    //     Original was GL_LEQUAL = 0x0203
    Lequal = 515,
    //
    // Summary:
    //     Original was GL_GREATER = 0x0204
    Greater = 516,
    //
    // Summary:
    //     Original was GL_NOTEQUAL = 0x0205
    Notequal = 517,
    //
    // Summary:
    //     Original was GL_GEQUAL = 0x0206
    Gequal = 518,
    //
    // Summary:
    //     Original was GL_ALWAYS = 0x0207
    Always = 519,
    None = 0
  }
}
