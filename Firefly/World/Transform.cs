using Firefly.Core;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;

namespace Firefly.World
{
  public class Transform
  {
    public WorldObject Owner { get; private set; }

    private Transform parent;
    public Transform Parent {
      get 
      { 
        return parent;
      } 
      set
      {
        if (parent != null)
        {
          parent.RemoveChild(this);
        }
        parent = value;
        if (parent != null)
        {
          parent.AddChild(this);
        }
        SetDirty();
      }
    }

    private List<Transform> children;

    private Vector3 position;
    public Vector3 Position
    {
      get
      {
        return position;
      }
      set
      {
        position = value;
        SetDirty();
      }
    }

    private Quaternion rotation;
    public Quaternion Rotation {
      get
      {
        return rotation;
      }
      set
      {
        rotation = value;
        Quaternion.ToEulerAngles(rotation, out eulerAngles);
        SetDirty();
      }
    }

    private Vector3 eulerAngles;
    public Vector3 EulerAngles
    {
      get
      {
        return eulerAngles;
      }
      set
      {
        eulerAngles = value;
        rotation = Quaternion.FromEulerAngles(eulerAngles.X, eulerAngles.Y, eulerAngles.Z);
        SetDirty();
      }
    }

    private Vector3 localScale;
    public Vector3 LocalScale
    {
      get
      {
        return localScale;
      }
      set
      {
        localScale = value;
        SetDirty();
      }
    }

    private Vector3 forward;
    public Vector3 Forward
    {
      get
      {
        return forward;
      }
    }

    private Vector3 up;
    public Vector3 Up
    {
      get
      {
        return up;
      }
    }

    private Vector3 right;
    public Vector3 Right
    {
      get
      {
        return right;
      }
    }

    private Matrix4X4<float> LocalToWorldMatrix { get; set; }
    private Matrix4X4<float> LocalToWorldRotationMatrix { get; set; }
    public uint DirtyId { get; private set; }

    private uint LastDirtyId;
    private List<Transform> RecursiveChildren;

    public Transform(WorldObject owner)
    {
      Owner = owner;
      parent = null;
      children = new List<Transform>();
      position = new Vector3(0, 0, 0);
      rotation = new Quaternion(0, 0, 0, 1);
      eulerAngles = new Vector3(0, 0, 0);
      localScale = new Vector3(1, 1, 1);
      DirtyId = 0;
      LastDirtyId = 0;
      SetDirty();
    }

    /// <summary>
    /// Add a child to this Transform.
    /// </summary>
    /// <param name="child"></param>
    public void AddChild(Transform child)
    {
      if (!children.Contains(child))
      {
        children.Add(child);
        child.Parent = this;
      }
    }

    /// <summary>
    /// Remove a child from this Transform.
    /// </summary>
    /// <param name="child"></param>
    public void RemoveChild(Transform child)
    {
      if (children.Contains(child))
      {
        children.Remove(child);
        child.Parent = null;
      }
    }

    /// <summary>
    /// Get the children of this Transform.
    /// </summary>
    /// <returns></returns>
    public List<Transform> GetChildren()
    {
      return children;
    }

    /// <summary>
    /// Get all children recursively.
    /// </summary>
    /// <returns></returns>
    public List<Transform> GetChildrenRecursively()
    {
      RecursiveChildren = new List<Transform>();
      GetChildrenOfTransform(this);
      return RecursiveChildren;
    }

    /// <summary>
    /// Recursively obtain the all children of this Transform.
    /// </summary>
    /// <param name="transform"></param>
    private void GetChildrenOfTransform(Transform transform)
    {
      if (transform != this)
      {
        RecursiveChildren.Add(transform);
      }
      List<Transform> children = transform.GetChildren();
      for (int i = 0; i < children.Count; i++)
      {
        GetChildrenOfTransform(children[i]);
      }
    }

    /// <summary>
    /// Mark the Transform dirty.
    /// </summary>
    public void SetDirty() {
      DirtyId++;
      for (int i = 0; i < children.Count; i++)
      {
        children[i].SetDirty();
      }
    }

    /// <summary>
    /// Get the local to world matrix.
    /// </summary>
    /// <returns></returns>
    public Matrix4X4<float> GetLocalMatrix()
    {
      if (LastDirtyId != DirtyId)
      {
        CalculateLocalMatrix();
      }

      return LocalToWorldMatrix;
    }

    /// <summary>
    /// Get the local to world matrix.
    /// </summary>
    /// <returns></returns>
    public Matrix4X4<float> GetLocalRotationMatrix()
    {
      if (LastDirtyId != DirtyId)
      {
        CalculateLocalMatrix();
      }

      return LocalToWorldRotationMatrix;
    }

    /// <summary>
    /// Calculate the local to world matrix.
    /// </summary>
    private void CalculateLocalMatrix()
    {
      //float positionX = position.X;
      //float positionY = position.Y;
      //float positionZ = position.Z;
      //Matrix4 Translation = new Matrix4(1, 0, 0, positionX,
      //                                  0, 1, 0, positionY,
      //                                  0, 0, 1, positionZ,
      //                                  0, 0, 0, 1);

      //float scaleX = localScale.X;
      //float scaleY = localScale.Y;
      //float scaleZ = localScale.Z;
      //Matrix4 Scale = new Matrix4(scaleX, 0, 0, 0,
      //                            0, scaleY, 0, 0,
      //                            0, 0, scaleZ, 0,
      //                            0, 0, 0, 1);

      //float rotationX = rotation.X;
      //float rotationY = rotation.Y;
      //float rotationZ = rotation.Z;

      //float rxCos = (float)Math.Cos(rotationX);
      //float rxSin = (float)Math.Sin(rotationX);
      //Matrix4 Rx = new Matrix4(1, 0, 0, 0,
      //                         0, rxCos, -rxSin, 0,
      //                         0, rxSin, rxCos, 0,
      //                         0, 0, 0, 1);

      //float ryCos = (float)Math.Cos(rotationY);
      //float rySin = (float)Math.Sin(rotationY);
      //Matrix4 Ry = new Matrix4(ryCos, 0, rySin, 0,
      //                         0, 1, 0, 0,
      //                        -rySin, 0, ryCos, 0,
      //                         0, 0, 0, 1);

      //float rzCos = (float)Math.Cos(rotationZ);
      //float rzSin = (float)Math.Sin(rotationZ);
      //Matrix4 Rz = new Matrix4(rzCos, -rzSin, 0, 0,
      //                         rzSin, rzCos, 0, 0,
      //                         0, 0, 1, 0,
      //                         0, 0, 0, 1);

      //Matrix4 Rotation = Matrix4.Mult(Rz, Matrix4.Mult(Ry, Rx));

      float rotationX = eulerAngles.X;
      float rotationY = eulerAngles.Y;
      float rotationZ = eulerAngles.Z;

      Matrix4X4<float> scaleMatrix = Matrix4X4<float>.CreateScale(localScale);
      Matrix4X4<float> rotationMatrix = Matrix4X4<float>.CreateFromQuaternion(rotation);
      Matrix4X4<float> translationMatrix = Matrix4X4<float>.CreateTranslation(position);
      LocalToWorldMatrix = scaleMatrix * rotationMatrix * translationMatrix;
      LocalToWorldRotationMatrix = rotationMatrix;
      //LocalToWorldMatrix = Matrix4.Mult(Translation, Matrix4.Mult(Scale, Rotation));
      //LocalToWorldNormalMatrix = new Matrix4(LocalToWorldMatrix.Row0, LocalToWorldMatrix.Row1, LocalToWorldMatrix.Row2, LocalToWorldMatrix.Row3);
      //LocalToWorldNormalMatrix.Transpose();
      //LocalToWorldNormalMatrix.Invert();
      if (parent != null)
      {
        LocalToWorldMatrix = Matrix4X4<float>.Mult(LocalToWorldMatrix, parent.GetLocalMatrix());
        LocalToWorldRotationMatrix = Matrix4X4<float>.Mult(LocalToWorldRotationMatrix, parent.GetLocalRotationMatrix());
      }

      right = Vector3.TransformVector(Vector3.UnitX, LocalToWorldRotationMatrix);
      up = Vector3.TransformVector(Vector3.UnitY, LocalToWorldRotationMatrix);
      forward = Vector3.TransformVector(Vector3.UnitZ, LocalToWorldRotationMatrix);
      LastDirtyId = DirtyId;
    }

    public void Destroy()
    {
      if (Owner != null)
      {
        Owner.Destroy();
        Owner = null;
      }
      if (parent != null)
      {
        parent.RemoveChild(this);
        parent = null;
      }
      if (children != null)
      {
        children.ForEach((child) => child.Destroy());
        children.Clear();
        children = null;
      }
    }
  }
}
