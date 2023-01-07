using Services.Extensions;
using UnityEngine;

namespace Services.ImageLoaderService
{
    public class ImageLoader : IImageLoader
    {
        public bool TryLoadSpite(string path, out Sprite result)
        {
            result = Resources.Load<Sprite>(path);
            if (result == null)
                return false;
            
            result.texture.name = result.texture.name;
            result.name = result.name;
            return result;
        }

        public bool TryLoadTexture(string path, out Texture result)
        {
            result = Resources.Load<Texture2D>(path);
            if (result == null)
                return false;
            
            result.name = result.name;
            return result;
        }
    }
}