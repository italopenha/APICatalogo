using APICatalogo.Context;
using APICatalogo.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APICatalogo.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProdutosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProdutosController(AppDbContext context)
        {
            _context = context;
        }

        // Rota: /produtos/primeiro
        [HttpGet("primeiro")]
        public ActionResult<Produto> GetPrimeiro()
        {
            var produto = _context.Produtos.FirstOrDefault();

            if (produto is null)
                return NotFound("Nenhum produto encontrado.");

            return produto;
        }

        // Rota: /produtos
        [HttpGet]
        public ActionResult<IEnumerable<Produto>> Get()
        {
            // Nunca retorne todos os registros em uma consulta
            var produtos = _context.Produtos.Take(10).AsNoTracking().ToList();

            if (produtos is null)
                return NotFound("Nenhum produto encontrado.");

            return produtos;
        }

        // Rota: /produtos/id
        [HttpGet("{id:int}", Name="ObterProduto")]
        public ActionResult<Produto> Get(int id)
        {
            var produto = _context.Produtos.AsNoTracking().FirstOrDefault(p => p.ProdutoId == id);

            if (produto is null)
                return NotFound("Produto não encontrado.");

            return produto;
        }

        // Rota: /produtos
        [HttpPost]
        public ActionResult Post(Produto produto)
        {
            if (produto is null) 
                return BadRequest();

            _context.Produtos.Add(produto);
            _context.SaveChanges();

            return new CreatedAtRouteResult("ObterProduto", new { id = produto.ProdutoId }, produto);
        }

        // Rota: /produtos/id
        [HttpPut("{id:int}")]
        public ActionResult Put(int id, Produto produto)
        {
            if (id != produto.ProdutoId)
                return BadRequest();

            _context.Entry(produto).State = EntityState.Modified;
            _context.SaveChanges();

            return Ok(produto);
        }

        // Rota: /produtos/id
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
