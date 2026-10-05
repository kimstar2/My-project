namespace _TevLib.ServiceLocatorSystem.TimeService
{
    public interface ITickService
    {
        float SecondTick { get; }
        float TimeTick { get; }
        int TickCount { get; }
    }
}