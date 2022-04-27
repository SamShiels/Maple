using Firefly.Texturing;
using Firefly.World.Mesh;

namespace Firefly.World
{

  public class Sprite : MeshObject
  {
    public override string TYPE { get; protected set; } = "Sprite";

    private static float[] vertices = new float[]
    {
       0.0f, 0.0f, 0.0f,
       1.0f, 0.0f, 0.0f,
       1.0f, 1.0f, 0.0f,
       0.0f, 1.0f, 0.0f
    };

    private static int[] indices = new int[]
    {
      0, 1, 2,
      0, 2, 3
    };

    private static float[] texcoords = new float[]
    {
      0.0f, 0.0f,
      1.0f, 0.0f,
      1.0f, 1.0f,
      0.0f, 1.0f
    };

    private float width = 1;
    public float Width
    {
      get { return width; }
      set
      {
        width = value;
        UpdateDimensions();
      }
    }

    private float height = 1;
    public float Height
    {
      get { return height; }
      set
      {
        height = value;
        UpdateDimensions();
      }
    }

    private float originX = 0;
    public float OriginX
    {
      get { return originX; }
      set
      {
        originX = value;
        UpdateDimensions();
      }
    }

    private float originY = 0;
    public float OriginY
    {
      get { return originY; }
      set
      {
        originY = value;
        UpdateDimensions();
      }
    }

    public Sprite()
    {
      width = 1;
      height = 1;
      Model = new Model()
      {
        Vertices = vertices,
        Indices = indices,
        Texcoords = texcoords
      };

      UpdateDimensions();
    }

    private void UpdateDimensions()
    {
      float minX = -originX * width;
      float maxX = -originX * width + width;
      float minY = -originY * height;
      float maxY = -originY * height + height;

      Model.Vertices = new float[]
      {
         minX, minY, vertices[2],
         maxX, minY, vertices[5],
         maxX, maxY, vertices[8],
         minX, maxY, vertices[11]
      };
    }
  }
}
