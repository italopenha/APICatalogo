using System.ComponentModel.DataAnnotations;

namespace APICatalogo.DTOs;

public class ProdutoDTOUpdateRequest
{
    [Range(1, 9999, ErrorMessage = "Estoque deve estar entre 1 e 9999")]
    public float Estoque { get; set; }

    public DateTime DataCadastro { get; set; }
}
