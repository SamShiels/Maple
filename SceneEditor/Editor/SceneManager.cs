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
    private TreeViewItem selectedObject;
    private TreeViewItem root;
    private TreeView treeView;

    internal SceneManager(TreeView treeView)
    {
      Scene = new Scene();
      this.treeView = treeView;

      root = new TreeViewItem();
      root.Header = "Scene";
      root.DataContext = Scene.RootObject;
      this.treeView.Items.Add(root);
    }

    internal T CreateObject<T>() where T : WorldObject
    {
      T worldObject = (T)Activator.CreateInstance(typeof(T));
      Scene.AddObject(worldObject);

      TreeViewItem item = new TreeViewItem();
      item.Header = worldObject.TYPE;
      item.DataContext = worldObject;

      ItemCollection collection = selectedObject != null ? selectedObject.Items : root.Items;
      collection.Add(item);
      return worldObject;
    }

    internal void NewItemSelected(TreeViewItem selectedObject)
    {
      this.selectedObject = selectedObject;
    }
  }
}
