namespace Application.ViewModels;

public class AuthenticationResult
{
    public string Token { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
}
