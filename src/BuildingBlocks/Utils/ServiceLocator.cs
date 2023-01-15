using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Utils;

//ref: https://dotnetcoretutorials.com/2018/05/06/servicelocator-shim-for-net-core/
public class ServiceLocator
{
    private IServiceProvider _currentServiceProvider;
    private static IServiceProvider _serviceProvider;

    public ServiceLocator(IServiceProvider currentServiceProvider)
    {
        _currentServiceProvider = currentServiceProvider;
    }

    public static ServiceLocator Current
    {
        get
        {
            return new ServiceLocator(_serviceProvider);
        }
    }

    public static void SetLocatorProvider(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public object GetInstance(Type serviceType)
    {
        return _currentServiceProvider.GetService(serviceType);
    }

    public TService GetInstance<TService>()
    {
        return _currentServiceProvider.GetService<TService>();
    }
}
