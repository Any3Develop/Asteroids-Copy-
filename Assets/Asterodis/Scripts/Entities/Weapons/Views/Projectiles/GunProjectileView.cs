namespace Asterodis.Entities.Weapons
{
    public class GunProjectileView : ProjectileBaseView, ITaggedEntity
    {
        public string Tag { get; private set; }

        public void SetTag(string value)
        {
            Tag = value;
        }

        protected override void OnDespawned()
        {
            base.OnDespawned();
            SetTag(string.Empty);
        }
    }
}