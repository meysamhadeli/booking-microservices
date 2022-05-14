using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BuildingBlocks.Web;

public static class ConfigurationExtensions
{
    public static TModel GetOptions<TModel>(this IConfiguration configuration, string section) where TModel : new()
    {
        var model = new TModel();
        configuration.GetSection(section).Bind(model);
        return model;
    }

    public static TModel GetOptions<TModel>(this IServiceCollection service, string section) where TModel : new()
    {
        var model = new TModel();
        var configuration = service.BuildServiceProvider().GetService<IConfiguration>();
        configuration?.GetSection(section).Bind(model);
        return model;
    }

    public static TModel GetOptions<TModel>(this WebApplication app, string section) where TModel : new()
    {
        var model = new TModel();
        app.Configuration?.GetSection(section).Bind(model);
        return model;
    }
}
