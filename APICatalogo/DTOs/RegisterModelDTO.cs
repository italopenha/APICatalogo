using System.ComponentModel.DataAnnotations;

namespace APICatalogo.DTOs;

public record RegisterModelDTO
{
    [Required(ErrorMessage = "User name is required")]
    public string? UserName { get; set; }

    [EmailAddress]
    [Required(ErrorMessage = "E-mail is required")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Password is required")]
    public string? Password { get; set; }
}
