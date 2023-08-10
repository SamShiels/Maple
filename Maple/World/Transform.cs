using Maple.Core;
using OpenTK.Mathematics;
using System;
using System.Collections.Generic;

namespace Maple.World
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
        rotation = Quaternion.FromEulerAngles(eulerAngles.X * (float)Math.PI / 180f, eulerAngles.Y * (float)Math.PI / 180f, eulerAngles.Z * (float)Math.PI / 180f);
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
        CalculateLocalMatrix();
        return forward;
      }
    }

    private Vector3 up;
    public Vector3 Up
    {
      get
      {
        CalculateLocalMatrix();
        return up;
      }
    }

    private Vector3 right;
    public Vector3 Right
    {
      get
      {
        CalculateLocalMatrix();
        return right;
      }
    }

    private Matrix4 LocalToWorldMatrix { get; set; }
    private Matrix4 LocalToWorldRotationMatrix { get; set; }
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
    public Matrix4 GetLocalMatrix()
    {
      CalculateLocalMatrix();

      return LocalToWorldMatrix;
    }

    /// <summary>
    /// Get the local to world matrix.
    /// </summary>
    /// <returns></returns>
    public Matrix4 GetLocalRotationMatrix()
    {
      CalculateLocalMatrix();

      return LocalToWorldRotationMatrix;
    }

    /// <summary>
    /// Calculate the local to world matrix.
    /// </summary>
    private void CalculateLocalMatrix()
    {
      if (LastDirtyId != DirtyId)
      {
        float rotationX = eulerAngles.X;
        float rotationY = eulerAngles.Y;
        float rotationZ = eulerAngles.Z;

        Matrix4 scaleMatrix = Matrix4.CreateScale(localScale);
        Matrix4 rotationMatrix = Matrix4.CreateFromQuaternion(rotation);
        Matrix4 translationMatrix = Matrix4.CreateTranslation(position);
        LocalToWorldMatrix = scaleMatrix * rotationMatrix * translationMatrix;
        LocalToWorldRotationMatrix = rotationMatrix;
        //LocalToWorldMatrix = Matrix4.Mult(Translation, Matrix4.Mult(Scale, Rotation));
        //LocalToWorldNormalMatrix = new Matrix4(LocalToWorldMatrix.Row0, LocalToWorldMatrix.Row1, LocalToWorldMatrix.Row2, LocalToWorldMatrix.Row3);
        //LocalToWorldNormalMatrix.Transpose();
        //LocalToWorldNormalMatrix.Invert();
        if (parent != null)
        {
          LocalToWorldMatrix = Matrix4.Mult(LocalToWorldMatrix, parent.GetLocalMatrix());
          LocalToWorldRotationMatrix = Matrix4.Mult(LocalToWorldRotationMatrix, parent.GetLocalRotationMatrix());
        }

        right = Vector3.TransformVector(Vector3.UnitX, LocalToWorldRotationMatrix);
        up = Vector3.TransformVector(Vector3.UnitY, LocalToWorldRotationMatrix);
        forward = Vector3.TransformVector(Vector3.UnitZ, LocalToWorldRotationMatrix);
        LastDirtyId = DirtyId;
      }
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
