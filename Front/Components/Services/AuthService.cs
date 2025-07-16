using booking_platform.DTO;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

public class AuthService : AuthenticationStateProvider
{
    private readonly HttpClient _httpClient;
    private bool autenticado = false;

    public AuthService(IHttpClientFactory factory)
    {
        _httpClient = factory.CreateClient("API");
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        autenticado = false;
        var person = new ClaimsPrincipal();
        var response = await _httpClient.GetAsync("auth/manage/info");

        if (response.IsSuccessStatusCode)
        {
            var info = await response.Content.ReadFromJsonAsync<AuthDTO>();
            Claim[] data =
            [
                new Claim(ClaimTypes.Name, info.Email),
                new Claim(ClaimTypes.Email, info.Email)
            ];

            var identity = new ClaimsIdentity(data, "Cookies");
            person = new ClaimsPrincipal(identity);
            autenticado = true;
        }

        return new AuthenticationState(person);
    }

    public async Task<AuthResponse> LoginAsync(AuthDTO authDTO)
    {
        var response = await _httpClient.PostAsJsonAsync("/auth/login?useCookies=true", authDTO);
        if (response.IsSuccessStatusCode)
            return new AuthResponse { Sucesso = true };
        var errors = await response.Content.ReadFromJsonAsync<string[]>() ?? new[] { "Ocorreu um erro. Tente novamente." };
        return new AuthResponse { Sucesso = false, Erros = errors };
    }
}