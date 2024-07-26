using APICatalogo.Context;
using APICatalogo.DTOs;
using APICatalogo.DTOs.Mappings;
using APICatalogo.Filters;
using APICatalogo.Models;
using APICatalogo.Repositories.Interfaces;
using APICatalogo.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;

namespace APICatalogo.Controllers;

[Route("[controller]")]
[ApiController]
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

    [HttpGet]
    public ActionResult<IEnumerable<CategoriaDTO>> Get()
    {
        var categorias = _uof.CategoriaRepository.GetAll();

        if (categorias is null)
            return NotFound("Não existe nenhuma categoria.");

        var categoriasDTO = categorias.ToCategoriasDTOList();

        return Ok(categoriasDTO);
    }

    [HttpGet("{id:int}", Name = "ObterCategoria")]
    public ActionResult<CategoriaDTO> Get(int id)
    {
        var categoria = _uof.CategoriaRepository.Get(c => c.CategoriaId == id);

        if (categoria is null)
        {
            _logger.LogWarning($"Categoria com id= {id} não encontrada.");
            return NotFound($"Categoria com id= {id} não encontrada.");
        }

        var categoriaDTO = categoria.ToCategoriaDTO();

        return Ok(categoriaDTO);
    }

    [HttpPost]
    public ActionResult<CategoriaDTO> Post(CategoriaDTO categoriaDTO)
    {
        if (categoriaDTO is null)
        {
            _logger.LogWarning("Dados inválidos.");
            return BadRequest("Dados inválidos.");
        }

        var categoria = categoriaDTO.ToCategoria();

        var categoriaCriada = _uof.CategoriaRepository.Create(categoria);
        _uof.Commit();

        var novaCategoriaDTO = categoriaCriada.ToCategoriaDTO();

        return new CreatedAtRouteResult("ObterCategoria", new { id = novaCategoriaDTO.CategoriaId }, novaCategoriaDTO);
    }

    [HttpPut("{id:int}")]
    public ActionResult<CategoriaDTO> Put(int id, CategoriaDTO categoriaDTO)
    {
        if (id != categoriaDTO.CategoriaId)
        {
            _logger.LogWarning("Dados inválidos.");
            return BadRequest("Dados inválidos.");
        }

        var categoria = categoriaDTO.ToCategoria();

        var categoriaAtualizada = _uof.CategoriaRepository.Update(categoria);
        _uof.Commit();

        var categoriaAtualizadaDTO = categoriaAtualizada.ToCategoriaDTO();

        return Ok(categoriaAtualizadaDTO);
    }

    [HttpDelete("{id:int}")]
    public ActionResult<CategoriaDTO> DeleteAsync(int id)
    {
        var categoria = _uof.CategoriaRepository.Get(c => c.CategoriaId == id);

        if (categoria is null)
        {
            _logger.LogWarning($"Categoria com id= {id} não encontrada.");
            return NotFound($"Categoria com id= {id} não encontrada.");
        }

        var categoriaExcluida = _uof.CategoriaRepository.Delete(categoria);
        _uof.Commit();

        var categoriaExcluidaDTO = categoriaExcluida.ToCategoriaDTO();

        return Ok(categoriaExcluidaDTO);
    }
}