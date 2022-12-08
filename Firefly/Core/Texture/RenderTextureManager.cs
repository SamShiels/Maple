using Firefly.Texturing;
using System;
using System.Collections.Generic;
using System.Text;

namespace Firefly.Core.Texture
{
  internal class RenderTextureManager
  {
    /// <summary>
    /// Texture components used for binding.
    /// </summary>
    private Dictionary<int, RenderTextureComponent> textureComponents;
    private TextureManager textureManager;

    public RenderTextureManager(TextureManager textureManager)
    {
      textureComponents = new Dictionary<int, RenderTextureComponent>();
      this.textureManager = textureManager;
    }

    /// <summary>
    /// Bind to a texture. Upload it to the GPU if it doesn't exist.
    /// </summary>
    /// <param name="texture"></param>
    public void BindRenderTexture(RenderTexture renderTexture)
    {
      int id = renderTexture.Id;
      RenderTextureComponent component;
      if (!textureComponents.TryGetValue(id, out component))
      {
        component = new RenderTextureComponent(textureManager);
        textureComponents.Add(id, component);
      }

      component.BindRenderTexture(renderTexture);
    }
  }
}
