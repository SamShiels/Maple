using OpenTK.Mathematics;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace SceneEditor.Editor.Controls
{
  internal class Vector3Controls
  {
    private Vector3 data;

    private TextBox xBox;
    private TextBox yBox;
    private TextBox zBox;

    public Vector3Controls(TextBox xBox, TextBox yBox, TextBox zBox)
    {
      this.xBox = xBox;
      this.yBox = yBox;
      this.zBox = zBox;
      SetEnabled(false);
    }

    public void AttachData(Vector3 data)
    {
      this.data = data;

      xBox.DataContext = data;
      yBox.DataContext = data;
      zBox.DataContext = data;
    }

    public void SetEnabled(bool enabled)
    {
      xBox.IsReadOnly = !enabled;
      yBox.IsReadOnly = !enabled;
      zBox.IsReadOnly = !enabled;
    }

    public Vector3 xChangedEventHandler(object sender, TextChangedEventArgs args)
    {
      float newValue = ParseFloatValue(((TextBox)sender).Text);
      data = new Vector3(newValue, data.Y, data.Z);

      return data;
    }

    public Vector3 yChangedEventHandler(object sender, TextChangedEventArgs args)
    {
      float newValue = ParseFloatValue(((TextBox)sender).Text);
      data = new Vector3(data.X, newValue, data.Z);

      return data;
    }

    public Vector3 zChangedEventHandler(object sender, TextChangedEventArgs args)
    {
      float newValue = ParseFloatValue(((TextBox)sender).Text);
      data = new Vector3(data.X, data.Y, newValue);

      return data;
    }

    private float ParseFloatValue(string text)
    {
      string numbersOnly = Regex.Replace(text, "[^0-9.]", "");
      float newValue = float.Parse(numbersOnly.Length > 0 ? numbersOnly : "0", CultureInfo.InvariantCulture.NumberFormat);
      return newValue;
    }
  }
}
