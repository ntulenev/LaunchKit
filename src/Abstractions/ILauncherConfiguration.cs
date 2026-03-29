using Models;

namespace Abstractions;

public interface ILauncherConfiguration
{
    LauncherOptions Load();
}
