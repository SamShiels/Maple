
namespace Firefly.World.Mesh
{
  public class Model
  {
		private static uint nextId = 0;
		public uint Id { get; private set; }
		public uint DirtyId { get; private set; }

		private float[] vertices;
		public float[] Vertices { get { return vertices; }
			set
			{
				vertices = value;
				vertexCount = value.Length / 3;
				IncrementDirtyId();
				CalculateBounds();
			}
		}

		private int vertexCount;
		public int VertexCount
		{
			get { return vertexCount; }
		}

		private float[] texcoords;
		public float[] Texcoords
		{
			get { return texcoords; }
			set
			{
				texcoords = value;
				texcoordCount = value.Length / 2;
			}
		}

		private int texcoordCount;
		public int TexcoordCount
		{
			get { return texcoordCount; }
		}

		private float[] normals;
		public float[] Normals
		{
			get { return normals; }
			set
			{
				normals = value;
				normalCount = value.Length / 3;
			}
		}

		private int normalCount;
		public int NormalCount
		{
			get { return normalCount; }
		}

		private float[] colors;
		public float[] Colors
		{
			get { return colors; }
			set
			{
				colors = value;
				colorCount = value.Length / 4;
			}
		}

		private int colorCount;
		public int ColorCount
		{
			get { return colorCount; }
		}

		private int[] indices;
		public int[] Indices
		{
			get { return indices; }
			set
			{
				indices = value;
			}
		}

		public float[] Bounds { get; private set; }

		public Model()
		{
			Id = nextId;
			nextId++;
		}

		private void CalculateBounds()
    {
			float[] verts = Vertices;
			float minX = float.PositiveInfinity;
			float maxX = float.NegativeInfinity;
			float minY = float.PositiveInfinity;
			float maxY = float.NegativeInfinity;
			float minZ = float.PositiveInfinity;
			float maxZ = float.NegativeInfinity;

			for (int vert = 0; vert < verts.Length; vert += 3)
			{
				float x = verts[vert];
				float y = verts[vert + 1];
				float z = verts[vert + 2];

				if (x < minX)
				{
					minX = x;
				}

				if (x > maxX)
				{
					maxX = x;
				}

				if (y < minY)
				{
					minY = y;
				}

				if (y > maxY)
				{
					maxY = y;
				}

				if (z < minZ)
				{
					minZ = z;
				}

				if (z > maxZ)
				{
					maxZ = z;
				}
			}

			Bounds = new float[] {
				minX, minY, minZ,
				maxX, minY, minZ,
				minX, maxY, minZ,
				maxX, maxY, minZ,
				minX, minY, maxZ,
				maxX, minY, maxZ,
				minX, maxY, maxZ,
				maxX, maxY, maxZ
			};
		}

    private void IncrementDirtyId()
    {
			DirtyId++;
		}

		public void Destroy()
		{
			vertices = null;
			texcoords = null;
			normals = null;
			indices = null;
		}
  }
}
