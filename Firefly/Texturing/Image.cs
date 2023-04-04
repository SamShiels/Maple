using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Firefly.Texturing
{
  public class Image
  {
    private List<byte> pixels;
    public uint Width { get; private set; }
    public uint Height { get; private set; }

    public Image(Stream stream)
    {
      Image<Rgba32> image = SixLabors.ImageSharp.Image.Load<Rgba32>(stream);
      //image.Mutate(x => x.Flip(FlipMode.Vertical));

      pixels = new List<byte>(4 * image.Width * image.Height);

      image.ProcessPixelRows(accessor =>
      {
        for (int y = 0; y < accessor.Height; y++)
        {
          Span<Rgba32> row = accessor.GetRowSpan(y);
          for (int x = 0; x < row.Length; x++)
          {
            pixels.Add(row[x].R);
            pixels.Add(row[x].G);
            pixels.Add(row[x].B);
            pixels.Add(row[x].A);
          }
        }
      });

      Width = (uint)image.Width;
      Height = (uint)image.Height;
    }

    public byte[] GetPixelArray()
    {
      return pixels.ToArray();
    }

    ~Image()
    {
      pixels.Clear();
      pixels = null;
    }
  }
}
