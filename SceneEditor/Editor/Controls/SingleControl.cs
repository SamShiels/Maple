using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace SceneEditor.Editor.Controls
{
  internal class SingleControl
  {
    private TextBox textBox;
    private FloatBinding data;

    public SingleControl(TextBox textBox)
    {
      this.textBox = textBox;
      SetEnabled(false);
    }

    public void AttachData(FloatBinding data)
    {
      this.data = data;
      textBox.DataContext = data;
    }

    public void SetEnabled(bool enabled)
    {
      textBox.IsReadOnly = !enabled;
    }

    public FloatBinding ChangedEventHandler(object sender, TextChangedEventArgs args)
    {
      float newValue = ParseFloatValue(((TextBox)sender).Text);
      data = new FloatBinding { value = newValue };
      return data;
    }

    private float ParseFloatValue(string text)
    {
      string numbersOnly = Regex.Replace(text, "[^0-9.]", "");
      return float.Parse(numbersOnly.Length > 0 ? numbersOnly : "0", CultureInfo.InvariantCulture.NumberFormat);
    }
  }
}
