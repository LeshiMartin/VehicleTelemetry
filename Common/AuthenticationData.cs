namespace Common;

public class AuthenticationData
{
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public byte[] SigningKey { get; set; } = [];
}
