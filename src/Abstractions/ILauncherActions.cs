using Models;

namespace Abstractions;

public interface ILauncherActions
{
    string Launch(ApplicationEntry application);

    string OpenContainingFolder(ApplicationEntry application);

    bool IsAvailable(ApplicationEntry application);

    string ResolveValue(string? value);
}
