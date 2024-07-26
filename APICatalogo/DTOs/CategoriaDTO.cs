using System.ComponentModel.DataAnnotations;

namespace APICatalogo.DTOs;

public record CategoriaDTO
{
    public int CategoriaId { get; init; }

    [Required]
    [StringLength(80)]
    public string? Nome { get; init; }

    [Required]
    [StringLength(300)]
    public string? ImagemUrl { get; init; }
}
