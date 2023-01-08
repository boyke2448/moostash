using System.Security.Claims;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.Authorization;

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


public class ClientPrincipal
{
    public string? IdentityProvider { get; set; }
    public string? UserId { get; set; }
    public string? UserDetails { get; set; }
    public IEnumerable<string>? UserRoles { get; set; }
    public IEnumerable<SwaClaims>? Claims { get; set; }
    public string? AccessToken { get; set; }
}

public class SwaClaims
{
    public string? Typ { get; set; }
    public string? Val { get; set; }
}

public class AuthenticationData
{
    public ClientPrincipal? ClientPrincipal { get; set; }
}

public static class ClientPrincipleToClaimsPrinciple
{
    public static ClaimsPrincipal GetClaimsFromClientClaimsPrincipal(ClientPrincipal principal)
    {
        principal.UserRoles =
            principal.UserRoles?.Except(new[] { "anonymous" }, StringComparer.CurrentCultureIgnoreCase) ?? new List<string>();

        if (!principal.UserRoles.Any())
        {
            return new ClaimsPrincipal();
        }

        ClaimsIdentity identity = AdaptToClaimsIdentity(principal);

        return new ClaimsPrincipal(identity);
    }

    private static ClaimsIdentity AdaptToClaimsIdentity(ClientPrincipal principal)
    {
        var identity = new ClaimsIdentity(principal.IdentityProvider);
        identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, principal.UserId!));
        identity.AddClaim(new Claim(ClaimTypes.Name, principal.UserDetails!));
        identity.AddClaims(principal.UserRoles!.Select(r => new Claim(ClaimTypes.Role, r)));
        return identity;
    }
}