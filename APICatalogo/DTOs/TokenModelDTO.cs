namespace APICatalogo.DTOs;

public record TokenModelDTO
{
    public string? AccessToken { get; set; }

    public string? RefreshToken { get; set; }
}
