using System;
using System.Collections.Generic;
using System.Text;
using OpenTK.Graphics.OpenGL4;

namespace Firefly.Textures
{
  public class TextureManager
  {
    /// <summary>
    /// Texture components used for binding.
    /// </summary>
    private Dictionary<int, TextureComponent> textureComponents;
    /// <summary>
    /// A list of bound textures.
    /// </summary>
    private List<int> activeTextures;
    /// <summary>
    /// The next unit we can bind a texture to.
    /// Also represents the amount of textures currently bound.
    /// </summary>
    private int nextAvailableTextureUnit;
    /// <summary>
    /// The maximum amount of textures this device can bind simultaneously.
    /// </summary>
    private int maximumTextureUnits;

    public TextureManager()
    {
      textureComponents = new Dictionary<int, TextureComponent>();
      maximumTextureUnits = GL.GetInteger(GetPName.MaxTextureImageUnits);
      activeTextures = new List<int>();
      nextAvailableTextureUnit = 0;
    }

    /// <summary>
    /// Get the amount of textures currently bound.
    /// </summary>
    /// <returns></returns>
    public int GetActiveTextureCount()
    {
      return activeTextures.Count;
    }

    /// <summary>
    /// Gets the amount of texture units we have available.
    /// </summary>
    /// <returns></returns>
    public int GetFreeTextureUnitCount()
    {
      return maximumTextureUnits - nextAvailableTextureUnit;
    }

    /// <summary>
    /// Gets the maximum amount of texture units available on the current device.
    /// </summary>
    /// <returns></returns>
    public int GetMaxTextureUnitCount()
    {
      return maximumTextureUnits;
    }

    /// <summary>
    /// Bind to a texture. Upload it to the GPU if it doesn't exist.
    /// </summary>
    /// <param name="texture"></param>
    public int UseTexture(Texture texture)
    {
      int id = texture.Id;
      TextureComponent component;
      if (!textureComponents.TryGetValue(texture.Id, out component))
      {
        component = new TextureComponent();
        component.CreateTexture(texture);
        textureComponents.Add(id, component);
      }

      if (!activeTextures.Contains(id))
      {
        // this texture has not been bound
        TextureUnit slot = TextureUnit.Texture0 + nextAvailableTextureUnit;
        component.SetUnit(slot);

        nextAvailableTextureUnit = (nextAvailableTextureUnit + 1) % (maximumTextureUnits);
        if (activeTextures.Count == maximumTextureUnits)
        {
          activeTextures[nextAvailableTextureUnit] = id;
        } else
        {
          activeTextures.Add(id);
        }
      }
      int usedSlot = activeTextures.IndexOf(id);

      return usedSlot;
    }

    /// <summary>
    /// Clear the active textures and reset the texture slots back to zero.
    /// </summary>
    public void ClearAllTextureSlots()
    {
      nextAvailableTextureUnit = 0;
      activeTextures.Clear();
    }
  }
}
