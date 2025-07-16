using booking_platform.DTO;
public class AuthService
{
    private readonly HttpClient _httpClient;

    public AuthService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<AuthResponse> LoginAsync(string email, string password)
    {
        var response = await _httpClient.PostAsJsonAsync("/auth/login", new { email, password });

        if (response.IsSuccessStatusCode)
        {
            return new AuthResponse { Sucesso = true, Erros = [] };
        }
        else
        {
            var errors = await response.Content.ReadFromJsonAsync<string[]>();
            return new AuthResponse { Sucesso = false, Erros = errors };
        }
    }
} 