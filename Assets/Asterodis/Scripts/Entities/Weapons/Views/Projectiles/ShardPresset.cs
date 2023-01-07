using System;
using System.Linq;
using UnityEngine;

namespace Asterodis.Entities.Weapons
{
    [Serializable]
    public struct ShardPresset
    {
        [SerializeField] private GameObject[] shards;

        public Vector2[] ChoiseShard(int index)
        {
            if (index >= shards.Length)
                throw new InvalidOperationException($"Shard index is out of bounds : {index}");


            var shard = shards[index];
            shard.SetActive(true);
            var collider = shard.GetComponent<PolygonCollider2D>();
            var size = collider.transform.localScale;
            return collider.points.Select(x => x * size).ToArray();
        }

        public void Disable()
        {
            foreach (var shard in shards)
            {
                if (shard)
                    shard.SetActive(false);
            }
        }
    }
}