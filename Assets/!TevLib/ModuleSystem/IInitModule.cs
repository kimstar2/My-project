namespace _TevLib.ModuleSystem
{
    public interface IInitModule : IModule
    {
        void Init(ModuleOwner owner);
    }
}