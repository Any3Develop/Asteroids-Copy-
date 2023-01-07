using UnityEngine;

namespace Services.Extensions
{
    public static class TextureExtensions
    {
        public const string RuntimeArtTag = "RuntimeArt_";

        public static void DestroySafe(this Texture value)
        {
            if (value == null || !value.name.Contains(RuntimeArtTag))
                return;
            
            Object.Destroy(value);
        }
        
        public static void DestroySafe(this Sprite value)
        {
            if (value == null || !value.name.Contains(RuntimeArtTag))
                return;

            Object.Destroy(value);
        }
    }
}