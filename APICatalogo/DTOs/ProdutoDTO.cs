using APICatalogo.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace APICatalogo.DTOs;

public record ProdutoDTO
{
    public int ProdutoId { get; set; }

    [Required(ErrorMessage = "O nome é obrigatório.")]
    [StringLength(80, ErrorMessage = "O nome deve ter no máximo {1} e no mínimo {2} caracteres.", MinimumLength = 5)]
    //[PrimeiraLetraMaiuscula]
    public string? Nome { get; set; }

    [Required]
    [StringLength(300, ErrorMessage = "A descrição deve ter no máximo {1} caracteres.")]
    public string? Descricao { get; set; }

    [Required]
    [Range(1, 10000, ErrorMessage = "O preço deve estar entre {1} e {2}.")]
    public decimal Preco { get; set; }

    [Required]
    [StringLength(300, MinimumLength = 10)]
    public string? ImagemUrl { get; set; }

    public int CategoriaId { get; set; }
}
