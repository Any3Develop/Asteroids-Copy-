using System.Linq;
using UnityEngine;

namespace Services.Extensions
{
    public static class ComponentEnxtensions
    {
        public static void DestroyChildsExcept(this Transform source, params Transform[] except)
        {
            foreach (Transform child in source)
            {
                if (!except.Contains(child))
                    Object.Destroy(child.gameObject);
            }
        }
        
        public static void DestroyChilds(this Transform source)
        {
            foreach (Transform child in source)
            {
                Object.Destroy(child.gameObject);
            }
        }
    }
}