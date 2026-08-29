namespace _TevLib.ServiceLocatorSystem.PoolService
{
    public interface IPoolingService
    {
        IPoolable Pop(PoolItemSO itemSo);
        void Push(IPoolable item);
    }
}