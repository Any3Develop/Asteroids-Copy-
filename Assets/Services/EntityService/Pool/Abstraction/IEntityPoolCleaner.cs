namespace Services.EntityService.Pool
{
    public interface IEntityPoolCleaner
    {
        void AddPool(IEntityPool pool);
        void ClearAllPools();
    }
}