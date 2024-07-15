﻿using APICatalogo.Context;
using APICatalogo.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
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
        public async Task<ActionResult<Produto>> GetPrimeiroAsync()
        {
            var produto = await _context.Produtos.FirstOrDefaultAsync();

            if (produto is null)
                return NotFound("Nenhum produto encontrado.");

            return produto;
        }

        // Rota: api/produtos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Produto>>> GetAsync()
        {
            // Nunca retorne todos os registros em uma consulta
            var produtos = await _context.Produtos.Take(10).AsNoTracking().ToListAsync();

            if (produtos is null)
                return NotFound("Nenhum produto encontrado.");

            return produtos;
        }

        // Rota: api/produtos/id
        //[HttpGet("{id:int:min(1)}", Name="ObterProduto")]
        //public async Task<ActionResult<Produto>> GetAsync(int id, [BindRequired]string nome)
        //{
        //    var nomeProduto = nome;

        //    var produto = await _context.Produtos.AsNoTracking().FirstOrDefaultAsync(p => p.ProdutoId == id);

        //    if (produto is null)
        //        return NotFound("Produto não encontrado.");

        //    return produto;
        //}

        [HttpGet("{id:int:min(1)}", Name = "ObterProduto")]
        public async Task<ActionResult<Produto>> GetAsync([FromQuery] int id)
        {
            var produto = await _context.Produtos.AsNoTracking().FirstOrDefaultAsync(p => p.ProdutoId == id);

            if (produto is null)
                return NotFound("Produto não encontrado.");

            return produto;
        }

        // Rota: api/produtos
        [HttpPost]
        public async Task<ActionResult> PostAsync(Produto produto)
        {
            if (produto is null) 
                return BadRequest();

            await _context.Produtos.AddAsync(produto);
            await _context.SaveChangesAsync();

            return new CreatedAtRouteResult("ObterProduto", new { id = produto.ProdutoId }, produto);
        }

        // Rota: api/produtos/id
        [HttpPut("{id:int}")]
        public async Task<ActionResult> PutAsync(int id, Produto produto)
        {
            if (id != produto.ProdutoId)
                return BadRequest();

            _context.Entry(produto).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return Ok(produto);
        }

        // Rota: api/produtos/id
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> DeleteAsync(int id)
        {
            var produto = await _context.Produtos.FirstOrDefaultAsync(p => p.ProdutoId == id);

            if (produto is null)
                return NotFound("Produto não encontrado.");

            _context.Produtos.Remove(produto);
            await _context.SaveChangesAsync();

            return Ok(produto);
        }
    }
}
