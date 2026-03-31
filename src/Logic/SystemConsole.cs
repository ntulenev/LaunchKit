namespace Logic;

internal sealed class SystemConsole : ISystemConsole
{
    public void Clear() => Console.Clear();

    public void WriteLine(string? value) => Console.WriteLine(value);
}
