namespace VoidNone.Logging.Core;

internal class NoneDisposable : IDisposable
{
    public void Dispose()
    {
    }

    public static NoneDisposable Instance { get; } = new NoneDisposable();
}