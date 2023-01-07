using UnityEngine;
using Random = UnityEngine.Random;

namespace Asterodis.Entities.Weapons
{
    [RequireComponent(typeof(PolygonCollider2D))]
    public class AsteroidProjectileView : ProjectileBaseView, IIndexedEntity, ITaggedEntity
    {
        [SerializeField] private ShardPresset[] shardsPresset;

        public int Index { get; private set; }
        
        public string Tag { get; private set; }

        public void SetTag(string value)
        {
            Tag = value;
        }

        public void SetIndex(int value)
        {
            Index = value;
            var randomStyle = Random.Range(0, shardsPresset.Length);
            var points = shardsPresset[randomStyle].ChoiseShard(value);
            if (contactCollider is PolygonCollider2D placeHolder)
                placeHolder.points = points;
        }

        protected override void OnSpawned()
        {
            base.OnSpawned();
            foreach (var shardPresset in shardsPresset)
            {
                shardPresset.Disable();
            }
        }
        
        protected override void OnDespawned()
        {
            base.OnDespawned();
            Tag = string.Empty;
            Index = 0;
        }
    }
}