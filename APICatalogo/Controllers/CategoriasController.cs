using APICatalogo.Context;
using APICatalogo.DTOs;
using APICatalogo.DTOs.Mappings;
using APICatalogo.Filters;
using APICatalogo.Models;
using APICatalogo.Pagination;
using APICatalogo.Repositories.Interfaces;
using APICatalogo.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using X.PagedList;

namespace APICatalogo.Controllers;

[EnableCors("OrigensComAcessoPermitido")]
[Route("[controller]")]
[ApiController]
[EnableRateLimiting("fixedwindow")]
[ApiExplorerSettings(IgnoreApi = true)]
public class CategoriasController : ControllerBase
{
    private readonly IUnitOfWork _uof;
    private readonly IConfiguration _configuration;
    private readonly ILogger _logger;

    public CategoriasController(IUnitOfWork uof, IConfiguration configuration, ILogger<CategoriasController> logger)
    {
        _uof = uof;
        _configuration = configuration;
        _logger = logger;
    }

    [HttpGet("LerArquivoConfiguracao")]
    public string GetValores()
    {
        var valor1 = _configuration["chave1"];
        var valor2 = _configuration["chave2"];

        var secao1 = _configuration["secao1:chave2"];

        return $"Chave1 = {valor1} \nChave2 = {valor2} \nSeção1 => Chave2 = {secao1}";
    }


    [HttpGet("UsandoFromServices/{nome}")]
    public ActionResult<string> GetSaudacaoFromServices([FromServices] IMeuServico meuServico, string nome)
    {
        return meuServico.Saudacao(nome);
    }

    [HttpGet("SemUsarFromServices/{nome}")]
    public ActionResult<string> GetSaudacaoSemFromServices(IMeuServico meuServico, string nome)
    {
        return meuServico.Saudacao(nome);
    }

    [HttpGet("pagination")]
    public async Task<ActionResult<IEnumerable<CategoriaDTO>>> GetAsync([FromQuery] CategoriasParameters categoriasParameters)
    {
        var categorias = await _uof.CategoriaRepository.GetCategoriasAsync(categoriasParameters);
        return ObterCategorias(categorias);
    }

    private ActionResult<IEnumerable<CategoriaDTO>> ObterCategorias(IPagedList<Categoria> categorias)
    {
        var metadata = new
        {
            categorias.Count,
            categorias.PageSize,
            categorias.PageCount,
            categorias.TotalItemCount,
            categorias.HasNextPage,
            categorias.HasPreviousPage
        };

        Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(metadata));

        var categoriasDTO = categorias.ToCategoriasDTOList();

        return Ok(categoriasDTO);
    }

    [HttpGet("filter/nome/pagination")]
    public async Task<ActionResult<IEnumerable<CategoriaDTO>>> GetCategoriasFiltradasAsync([FromQuery] CategoriasFiltroNome categoriasFiltro)
    {
        var categoriasFiltradas = await _uof.CategoriaRepository.GetCategoriasFiltroNomeAsync(categoriasFiltro);
        return ObterCategorias(categoriasFiltradas);
    }

    [HttpGet]
    //[Authorize]
    [DisableRateLimiting]
    public async Task<ActionResult<IEnumerable<CategoriaDTO>>> GetAsync()
    {
        var categorias = await _uof.CategoriaRepository.GetAllAsync();

        if (categorias is null)
            return NotFound("Não existe nenhuma categoria.");

        var categoriasDTO = categorias.ToCategoriasDTOList();

        return Ok(categoriasDTO);
    }

    [DisableCors]
    [HttpGet("{id:int}", Name = "ObterCategoria")]
    public async Task<ActionResult<CategoriaDTO>> GetAsync(int id)
    {
        var categoria = await _uof.CategoriaRepository.GetAsync(c => c.CategoriaId == id);

        if (categoria is null)
        {
            _logger.LogWarning($"Categoria com id= {id} não encontrada.");
            return NotFound($"Categoria com id= {id} não encontrada.");
        }

        var categoriaDTO = categoria.ToCategoriaDTO();

        return Ok(categoriaDTO);
    }

    [HttpPost]
    public async Task<ActionResult<CategoriaDTO>> PostAsync(CategoriaDTO categoriaDTO)
    {
        if (categoriaDTO is null)
        {
            _logger.LogWarning("Dados inválidos.");
            return BadRequest("Dados inválidos.");
        }

        var categoria = categoriaDTO.ToCategoria();

        var categoriaCriada = _uof.CategoriaRepository.Create(categoria);
        await _uof.CommitAsync();

        var novaCategoriaDTO = categoriaCriada.ToCategoriaDTO();

        return new CreatedAtRouteResult("ObterCategoria", new { id = novaCategoriaDTO.CategoriaId }, novaCategoriaDTO);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<CategoriaDTO>> PutAsync(int id, CategoriaDTO categoriaDTO)
    {
        if (id != categoriaDTO.CategoriaId)
        {
            _logger.LogWarning("Dados inválidos.");
            return BadRequest("Dados inválidos.");
        }

        var categoria = categoriaDTO.ToCategoria();

        var categoriaAtualizada = _uof.CategoriaRepository.Update(categoria);
        await _uof.CommitAsync();

        var categoriaAtualizadaDTO = categoriaAtualizada.ToCategoriaDTO();

        return Ok(categoriaAtualizadaDTO);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<CategoriaDTO>> DeleteAsync(int id)
    {
        var categoria = await _uof.CategoriaRepository.GetAsync(c => c.CategoriaId == id);

        if (categoria is null)
        {
            _logger.LogWarning($"Categoria com id= {id} não encontrada.");
            return NotFound($"Categoria com id= {id} não encontrada.");
        }

        var categoriaExcluida = _uof.CategoriaRepository.Delete(categoria);
        await _uof.CommitAsync();

        var categoriaExcluidaDTO = categoriaExcluida.ToCategoriaDTO();

        return Ok(categoriaExcluidaDTO);
    }
}