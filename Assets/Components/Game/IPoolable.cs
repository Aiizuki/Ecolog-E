namespace Assets.Components.Game
{
    /// <summary>
    /// This interface defines the methods that an object must implement to be used with a <see cref="ObjectPoolManager"/>
    /// It allows the object to perform specific actions when it is created, retrieved from the pool, or returned to the pool
    /// </summary>
    public interface IPoolable
    {
        void OnCreatedByPool();
        void OnGetFromPool();
        void OnReturnToPool();
    }
}