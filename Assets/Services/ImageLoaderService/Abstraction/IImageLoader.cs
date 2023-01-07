using UnityEngine;

namespace Services.ImageLoaderService
{
    public interface IImageLoader
    {
        bool TryLoadSpite(string path, out Sprite result);
        
        bool TryLoadTexture(string path, out Texture result);
    }
}