using APICatalogo.Context;
using APICatalogo.Filters;
using APICatalogo.Models;
using APICatalogo.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;

namespace APICatalogo.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CategoriasController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly ILogger _logger;

    public CategoriasController(AppDbContext context, IConfiguration configuration, ILogger<CategoriasController> logger)
    {
        _context = context;
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

    [HttpGet("produtos")]
    public ActionResult<IEnumerable<Categoria>> GetCategoriasProdutos()
    {
        //return _context.Categorias.Include(p => p.Produtos).AsNoTracking().ToList();
        _logger.LogInformation("=========== GET api/categorias/produtos ===========");

        // Nunca retorne objetos relacionados sem aplicar um filtro
        return _context.Categorias.Include(p => p.Produtos).Where(c => c.CategoriaId <= 5).AsNoTracking().ToList();
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Categoria>>> GetAsync()
    {
        return await _context.Categorias.AsNoTracking().ToListAsync();
    }

    [HttpGet("{id:int}", Name = "ObterCategoria")]
    public async Task<ActionResult<Categoria>> GetAsync(int id)
    {
        var categoria = await _context.Categorias.FirstOrDefaultAsync(c => c.CategoriaId == id);

        if (categoria is null)
        {
            _logger.LogWarning($"Categoria com id= {id} não encontrada.");
            return NotFound($"Categoria com id= {id} não encontrada.");
        }
        return Ok(categoria);
    }

    [HttpPost]
    public async Task<ActionResult> PostAsync(Categoria categoria)
    {
        if (categoria is null)
        {
            _logger.LogWarning("Dados inválidos.");
            return BadRequest("Dados inválidos.");
        }

        await _context.Categorias.AddAsync(categoria);
        await _context.SaveChangesAsync();

        return new CreatedAtRouteResult("ObterCategoria", new { id = categoria.CategoriaId }, categoria);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> PutAsync(int id, Categoria categoria)
    {
        if (id != categoria.CategoriaId)
        {
            _logger.LogWarning("Dados inválidos.");
            return BadRequest("Dados inválidos.");
        }

        _context.Entry(categoria).State = EntityState.Modified;
        await _context.SaveChangesAsync();

        return Ok(categoria);
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<Categoria>> DeleteAsync(int id)
    {
        var categoria = await _context.Categorias.FirstOrDefaultAsync(c => c.CategoriaId == id);

        if (categoria is null)
        {
            _logger.LogWarning($"Categoria com id= {id} não encontrada.");
            return NotFound($"Categoria com id= {id} não encontrada.");
        }

        _context.Categorias.Remove(categoria);
        await _context.SaveChangesAsync();

        return Ok(categoria);
    }
}