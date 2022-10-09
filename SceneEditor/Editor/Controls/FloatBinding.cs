using System;
using System.Collections.Generic;
using System.Text;

namespace SceneEditor.Editor.Controls
{
  internal class FloatBinding
  {
    public float this[int index]
    {
      get
      {
        return index switch
        {
          0 => value,
          _ => throw new IndexOutOfRangeException("You tried to access this float at index: " + index),
        };
      }
      set
      {
        switch (index)
        {
          case 0:
            this.value = value;
            break;
          default:
            throw new IndexOutOfRangeException("You tried to set this float at index: " + index);
        }
      }
    }
    public float value;
  }
}
