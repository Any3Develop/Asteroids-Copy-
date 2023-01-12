namespace Asterodis.Entities.Weapons
{
    public interface IProjectileWeapon : IWeapon
    {
        void Fire(params object[] args);
        bool CanAttack(params object[] args);
    }
}