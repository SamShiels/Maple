using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using Firefly;
using Firefly.Core;
using Firefly.Rendering;
using Firefly.Texturing;
using Firefly.Utilities;
using Firefly.World;
using Firefly.World.Mesh;
using OpenTK.Mathematics;


namespace SceneEditor.Editor
{
  internal class SceneManager
  {
    internal Scene Scene { get; private set; }
    private WorldObject sceneContainer;
    private WorldObject editorGizmoContainer;

    private TreeViewItem selectedObject;
    private TreeView treeView;

    internal SceneManager(TreeView treeView)
    {
      Scene = new Scene();
      // Create two containers to seperate the scene content from the editor gizmos
      sceneContainer = new WorldObject();
      Scene.AddObject(sceneContainer);

      editorGizmoContainer = new WorldObject();
      Scene.AddObject(editorGizmoContainer);
      this.treeView = treeView;
    }

    internal void AssignCamera(Camera camera)
    {
      Scene.AssignCamera(camera);
    }

    internal void AddEditorObject(WorldObject worldObject)
    {
      editorGizmoContainer.Transform.AddChild(worldObject.Transform);
    }

    internal T CreateObject<T>() where T : WorldObject
    {
      T worldObject = (T)Activator.CreateInstance(typeof(T));

      Scene.AddObject(worldObject, (WorldObject)selectedObject?.DataContext);

      TreeViewItem item = new TreeViewItem();
      item.Header = worldObject.GetType();
      item.DataContext = worldObject;

      ItemCollection collection = selectedObject != null ? selectedObject.Items : treeView.Items;
      collection.Add(item);
      return worldObject;
    }

    internal void NewItemSelected(TreeViewItem selectedObject)
    {
      this.selectedObject = selectedObject;
    }
  }
}
