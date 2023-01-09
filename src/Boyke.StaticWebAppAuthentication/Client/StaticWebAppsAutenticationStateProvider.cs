using System.Net.Http.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;

namespace Boyke.StaticWebAppAuthentication.Client;

public sealed class StaticWebAppsAutenticationStateProvider : AuthenticationStateProvider
{
    private readonly IConfiguration _config;
    private readonly HttpClient _httpClient;

    public StaticWebAppsAutenticationStateProvider(IConfiguration config, HttpClient httpClient)
    {
        _config = config;
        _httpClient = httpClient;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var clientPrinciple = await GetClientPrinciple();
            var claimsPrincipal = ClientPrincipleToClaimsPrinciple.GetClaimsFromClientClaimsPrincipal(clientPrinciple);
            return new AuthenticationState(claimsPrincipal);
        }
        catch
        {
            return new AuthenticationState(new ClaimsPrincipal());
        }
    }

    private async Task<ClientPrincipal> GetClientPrinciple()
    {
        var authDataUrl = _config.GetValue("StaticWebAppsAuthentication:AuthenticationDataUrl", "/.auth/me");
        var data = await _httpClient.GetFromJsonAsync<AuthenticationData>(authDataUrl);
        var clientPrinciple = data?.ClientPrincipal ?? new ClientPrincipal();
        return clientPrinciple;
    }
}
