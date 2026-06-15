namespace UserApp.Application.Common;

public static class ServiceProviderAccessor
{
    private static readonly AsyncLocal<IServiceProvider?> _current = new();
    public static IServiceProvider? Current
    {
        get => _current.Value;
        set => _current.Value = value;
    }
}
