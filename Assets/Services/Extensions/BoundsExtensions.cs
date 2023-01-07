using UnityEngine;

namespace Services.Extensions
{
    public static class BoundsExtensions
    {
        public static Bounds CalculateWorldBounds(this Camera camera, float margin = 0, float depth = 0)
        {
            var screenMax = new Vector3(Screen.width, Screen.height, depth);
            var worldMax = camera.ScreenToWorldPoint(screenMax).SetZ(depth);
            var center = camera.ScreenToWorldPoint(screenMax / 2f).SetZ(depth);
            var sizeX = Mathf.Abs(worldMax.x - center.x) * 2f;
            var sizeY = Mathf.Abs(worldMax.y - center.y) * 2f;
            var size = new Vector3(sizeX - margin, sizeY - margin, 0);
            return new Bounds(center, size);
        }
    }
}