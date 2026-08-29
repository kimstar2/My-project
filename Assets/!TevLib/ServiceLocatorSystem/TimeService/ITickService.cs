namespace _TevLib.ServiceLocatorSystem.TimeService
{
    public interface ITickService
    {
        float MaxTimeTick { get; }
        float TimeTick { get; }
        int TickCount { get; }
    }
}