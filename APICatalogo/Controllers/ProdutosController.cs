using APICatalogo.Context;
using APICatalogo.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APICatalogo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProdutosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProdutosController(AppDbContext context)
        {
            _context = context;
        }

        // Rota: /primeiro (Assim ignora o atributo definido em Route)
        //[HttpGet("primeiro")]
        //[HttpGet("teste")]
        //[HttpGet("/primeiro")]
        [HttpGet("{valor:alpha:length(5)}")]
        public ActionResult<Produto> GetPrimeiro(string valor)
        {
            var teste = valor;
            var produto = _context.Produtos.FirstOrDefault();

            if (produto is null)
                return NotFound("Nenhum produto encontrado.");

            return produto;
        }

        // Rota: api/produtos
        [HttpGet]
        public ActionResult<IEnumerable<Produto>> Get()
        {
            // Nunca retorne todos os registros em uma consulta
            var produtos = _context.Produtos.Take(10).AsNoTracking().ToList();

            if (produtos is null)
                return NotFound("Nenhum produto encontrado.");

            return produtos;
        }

        // Rota: api/produtos/id
        [HttpGet("{id:int:min(1)}", Name="ObterProduto")]
        public ActionResult<Produto> Get(int id)
        {
            var produto = _context.Produtos.AsNoTracking().FirstOrDefault(p => p.ProdutoId == id);

            if (produto is null)
                return NotFound("Produto não encontrado.");

            return produto;
        }

        // Rota: api/produtos
        [HttpPost]
        public ActionResult Post(Produto produto)
        {
            if (produto is null) 
                return BadRequest();

            _context.Produtos.Add(produto);
            _context.SaveChanges();

            return new CreatedAtRouteResult("ObterProduto", new { id = produto.ProdutoId }, produto);
        }

        // Rota: api/produtos/id
        [HttpPut("{id:int}")]
        public ActionResult Put(int id, Produto produto)
        {
            if (id != produto.ProdutoId)
                return BadRequest();

            _context.Entry(produto).State = EntityState.Modified;
            _context.SaveChanges();

            return Ok(produto);
        }

        // Rota: api/produtos/id
        [HttpDelete("{id:int}")]
        public ActionResult Delete(int id)
        {
            var produto = _context.Produtos.FirstOrDefault(p => p.ProdutoId == id);

            if (produto is null)
                return NotFound("Produto não encontrado.");

            _context.Produtos.Remove(produto);
            _context.SaveChanges();

            return Ok(produto);
        }
    }
}
