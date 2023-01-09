using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace Boyke.StaticWebAppAuthentication.Client;

public static class AuthetnicationExtensions
{
    public static IServiceCollection AddStaticWebAppAuthentication(this IServiceCollection services)
    {
        return services.AddAuthorizationCore()
        .AddScoped<AuthenticationStateProvider, StaticWebAppsAutenticationStateProvider>();
    }
}
