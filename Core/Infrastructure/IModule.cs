using Panda.Infrastructure;

namespace Core.Infrastructure
{
    public interface IModule : IScreen
    {
        ModuleType Type { get; }
    }
}
