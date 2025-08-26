namespace NewsCollection.Application.Common;

public class JwtSettings
{
    public string Token { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
}
