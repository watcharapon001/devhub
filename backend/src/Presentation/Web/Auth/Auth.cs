namespace Web.Auth;
public record AuthLoginRequest(string Username, string Password);
public record AuthLoginResponse(string AccessToken, DateTime ExpiresAt, string Username);
