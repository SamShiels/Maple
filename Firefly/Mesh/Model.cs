
namespace Firefly.Mesh
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
				IncrementDirtyId();
				CalculateBounds();
			}
		}

		private float[] texcoords;
		public float[] Texcoords
		{
			get { return texcoords; }
			set
			{
				texcoords = value;
			}
		}

		private float[] normals;
		public float[] Normals
		{
			get { return normals; }
			set
			{
				normals = value;
			}
		}

		private int[] indices;
		public int[] Indices
		{
			get { return indices; }
			set
			{
				indices = value;
				//IncrementDirtyId();
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
