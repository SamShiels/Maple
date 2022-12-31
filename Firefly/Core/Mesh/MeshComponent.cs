using OpenTK.Mathematics;
using Firefly.World.Mesh;

namespace Firefly.Core.Mesh
{
	internal class MeshComponent
	{
		private uint LastVertexDirtyId;
		private uint LastBoundDirtyId;
		private uint LastModelVertexDirtyId;
		private uint LastModelBoundDirtyId;
		private MeshObject Owner;

		private float[] worldVertices;

		public float[] WorldVertices { get
			{
				if (Owner.Model.Indices.Length > Constants.BATCH_BUFFER_MAX_INDICES)
				{
					return Owner.Model.Vertices;
				}

				if (Owner.Model.DirtyId != LastModelVertexDirtyId)
        {
					// The core model vertices have changed. Re-init the array with the updated length.
					worldVertices = new float[Owner.Model.Vertices.Length];
				}

				if ((Owner.Transform.DirtyId != LastVertexDirtyId) || (Owner.Model.DirtyId != LastModelVertexDirtyId))
				{
					Matrix4 localToWorld = Owner.Transform.GetLocalMatrix();
					CalculateWorldVertices(localToWorld);
				}
				return worldVertices;
			}
			private set { worldVertices = value; }
		}

		private float[] worldBounds;

		public float[] WorldBounds { get
			{
				if (Owner.Model.DirtyId != LastModelBoundDirtyId)
				{
					// The core model vertices have changed. Re-init the array with the updated length.
					worldBounds = new float[Owner.Model.Bounds.Length];
				}

				if ((Owner.Transform.DirtyId != LastBoundDirtyId) || (Owner.Model.DirtyId != LastModelBoundDirtyId))
				{
					CalculateWorldBounds(Owner.Transform.GetLocalMatrix());
				}
				return worldBounds;
			}
			private set { worldBounds = value; }
		}

		public MeshComponent(MeshObject owner)
		{
			Owner = owner;
			LastModelVertexDirtyId = 0;
			LastModelBoundDirtyId = 0;
			LastVertexDirtyId = 0;
			LastBoundDirtyId = 0;
		}

		public void Destroy() {
			Owner = null;
			WorldVertices = null;
			WorldBounds = null;
		}

		private void CalculateWorldVertices(Matrix4 m) {
			float[] verts = Owner.Model.Vertices;
			for (int vert = 0; vert < worldVertices.Length; vert += 3) {
				float x = verts[vert];
				float y = verts[vert + 1];
				float z = verts[vert + 2];
				Vector3 localPoint = new Vector3(x, y, z);
				Vector3 point = Vector3.TransformPosition(localPoint, m);

				worldVertices[vert] = point.X;
				worldVertices[vert + 1] = point.Y;
				worldVertices[vert + 2] = point.Z;
			}

			LastModelVertexDirtyId = Owner.Model.DirtyId;
			LastVertexDirtyId = Owner.Transform.DirtyId;
		}

		private void CalculateWorldBounds(Matrix4 m) {
			float[] bounds = Owner.Model.Bounds;
			for (int bound = 0; bound < bounds.Length; bound += 3)
			{
				float x = bounds[bound];
				float y = bounds[bound + 1];
				float z = bounds[bound + 2];

				(float, float, float) point = Utilities.Math.TransformPoint(m, x, y, z);

				worldBounds[bound] = point.Item1;
				worldBounds[bound + 1] = point.Item2;
				worldBounds[bound + 2] = point.Item3;
			}

			LastModelBoundDirtyId = Owner.Model.DirtyId;
			LastBoundDirtyId = Owner.Transform.DirtyId;
		}
	}
}
