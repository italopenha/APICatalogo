using APICatalogo.Context;
using APICatalogo.Models;
using APICatalogo.Repositories;
using APICatalogo.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;

namespace APICatalogo.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProdutosController : ControllerBase
    {
        private readonly IUnitOfWork _uof;
        private readonly ILogger _logger;

        public ProdutosController(IUnitOfWork uof, ILogger<ProdutosController> logger)
        {
            _uof = uof;
            _logger = logger;
        }

        [HttpGet("produtos/{id}")]
        public ActionResult <IEnumerable<Produto>> GetProdutosPorCategoria(int id)
        {
            var produtos = _uof.ProdutoRepository.GetProdutosPorCategoria(id);

            if (produtos is null)
                return NotFound();

            return Ok(produtos);
        }

        // Rota: api/produtos
        [HttpGet]
        public ActionResult<IEnumerable<Produto>> Get()
        {
            // Nunca retorne todos os registros em uma consulta
            var produtos = _uof.ProdutoRepository.GetAll().ToList();

            if (produtos is null)
                return NotFound("Nenhum produto encontrado.");

            return Ok(produtos);
        }

        [HttpGet("{id:int:min(1)}", Name = "ObterProduto")]
        public ActionResult<Produto> Get(int id)
        {
            var produto = _uof.ProdutoRepository.Get(p => p.ProdutoId == id);

            if (produto is null)
            {
                _logger.LogWarning($"Produto com id= {id} não encontrado.");
                return NotFound($"Produto com id= {id} não encontrado.");
            }

            return Ok(produto);
        }

        // Rota: api/produtos
        [HttpPost]
        public ActionResult Post(Produto produto)
        {
            if (produto is null)
            {
                _logger.LogWarning("Dados inválidos.");
                return BadRequest("Dados inválidos.");
            }

            var novoProduto = _uof.ProdutoRepository.Create(produto);
            _uof.Commit();

            return new CreatedAtRouteResult("ObterProduto", new { id = novoProduto.ProdutoId }, novoProduto);
        }

        // Rota: api/produtos/id
        [HttpPut("{id:int}")]
        public ActionResult Put(int id, Produto produto)
        {
            if (id != produto.ProdutoId)
            {
                _logger.LogWarning("Dados inválidos.");
                return BadRequest("Dados inválidos.");
            }

            var produtoAtualizado = _uof.ProdutoRepository.Update(produto);
            _uof.Commit();

            return Ok(produtoAtualizado);
        }

        // Rota: api/produtos/id
        [HttpDelete("{id:int}")]
        public ActionResult Delete(int id)
        {
            var produto = _uof.ProdutoRepository.Get(p => p.ProdutoId == id);

            if (produto is null)
                return NotFound("Produto não encontrado.");

            var produtoDeletado = _uof.ProdutoRepository.Delete(produto);
            _uof.Commit();

            return Ok(produtoDeletado);
        }
    }
}
