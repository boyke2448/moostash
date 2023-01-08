using Microsoft.AspNetCore.Components.Authorization;

public static class AuthetnicationExtensions
{
    public static IServiceCollection AddStaticWebAppAuthentication(this IServiceCollection services)
    {
        return services.AddAuthorizationCore()
        .AddScoped<AuthenticationStateProvider, StaticWebAppsAutenticationStateProvider>();
    }
}
