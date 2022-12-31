using System;
using System.Collections.Generic;
using System.Text;

namespace Firefly.World.Mesh
{
  public class ModelLibrary
  {
    private Dictionary<string, Model> library;

    private static ModelLibrary instance = null;
    public static ModelLibrary Instance
    {
      get
      {
        if (instance == null)
        {
          instance = new ModelLibrary();
        }
        return instance;
      }
    }

    public ModelLibrary()
    {
      library = new Dictionary<string, Model>();
      CreateCube();
    }

    public Model GetModel(string ModelName)
    {
      Model Model;
      if (library.TryGetValue(ModelName, out Model))
      {
        return Model;
      }
      return null;
    }

    private void SetModel(string ModelName, Model Model)
    {
      library.Add(ModelName, Model);
    }

    private void CreateCube()
    {
      //Model Model = new Model(vertex, fragment);
      //SetModel("spriteBasic", Model);
    }
  }
}
