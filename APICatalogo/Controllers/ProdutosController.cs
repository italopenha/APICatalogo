using APICatalogo.Context;
using APICatalogo.DTOs;
using APICatalogo.Models;
using APICatalogo.Pagination;
using APICatalogo.Repositories;
using APICatalogo.Repositories.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace APICatalogo.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class ProdutosController : ControllerBase
    {
        private readonly IUnitOfWork _uof;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        public ProdutosController(IUnitOfWork uof, ILogger<ProdutosController> logger, IMapper mapper)
        {
            _uof = uof;
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet("produtos/{id}")]
        public ActionResult <IEnumerable<ProdutoDTO>> GetProdutosPorCategoria(int id)
        {
            var produtos = _uof.ProdutoRepository.GetProdutosPorCategoria(id);

            if (produtos is null)
                return NotFound();

            // var destino = _mapper.Map<Destino>(origem);
            var produtosDTO = _mapper.Map<IEnumerable<ProdutoDTO>>(produtos);

            return Ok(produtosDTO);
        }

        [HttpGet("pagination")]
        public ActionResult<IEnumerable<ProdutoDTO>> Get([FromQuery] ProdutosParameters produtosParameters)
        {
            var produtos = _uof.ProdutoRepository.GetProdutos(produtosParameters);

            return ObterProdutos(produtos);
        }

        [HttpGet("filter/preco/pagination")]
        public ActionResult<IEnumerable<ProdutoDTO>> GetProdutosFilterPreco([FromQuery] ProdutosFiltroPreco produtosFilterParameters)
        {
            var produtos = _uof.ProdutoRepository.GetProdutosFiltroPreco(produtosFilterParameters);
            return ObterProdutos(produtos);
        }

        private ActionResult<IEnumerable<ProdutoDTO>> ObterProdutos(PagedList<Produto> produtos)
        {
            var metadata = new
            {
                produtos.TotalCount,
                produtos.PageSize,
                produtos.CurrentPage,
                produtos.TotalPages,
                produtos.HasNext,
                produtos.HasPrevious
            };

            Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(metadata));

            var produtosDTO = _mapper.Map<IEnumerable<ProdutoDTO>>(produtos);

            return Ok(produtosDTO);
        }

        // Rota: api/produtos
        [HttpGet]
        public ActionResult<IEnumerable<ProdutoDTO>> Get()
        {
            // Nunca retorne todos os registros em uma consulta
            var produtos = _uof.ProdutoRepository.GetAll().ToList();

            if (produtos is null)
                return NotFound("Nenhum produto encontrado.");

            var produtosDTO = _mapper.Map<IEnumerable<ProdutoDTO>>(produtos);

            return Ok(produtosDTO);
        }

        [HttpGet("{id:int:min(1)}", Name = "ObterProduto")]
        public ActionResult<ProdutoDTO> Get(int id)
        {
            var produto = _uof.ProdutoRepository.Get(p => p.ProdutoId == id);

            if (produto is null)
            {
                _logger.LogWarning($"Produto com id= {id} não encontrado.");
                return NotFound($"Produto com id= {id} não encontrado.");
            }

            var produtoDTO = _mapper.Map<ProdutoDTO>(produto);

            return Ok(produtoDTO);
        }

        // Rota: api/produtos
        [HttpPost]
        public ActionResult<ProdutoDTO> Post(ProdutoDTO produtoDTO)
        {
            if (produtoDTO is null)
            {
                _logger.LogWarning("Dados inválidos.");
                return BadRequest("Dados inválidos.");
            }

            var produto = _mapper.Map<Produto>(produtoDTO);

            var novoProduto = _uof.ProdutoRepository.Create(produto);
            _uof.Commit();

            var novoProdutoDTO = _mapper.Map<ProdutoDTO>(novoProduto);

            return new CreatedAtRouteResult("ObterProduto", new { id = novoProdutoDTO.ProdutoId }, novoProdutoDTO);
        }

        // Rota: api/produtos/id
        [HttpPut("{id:int}")]
        public ActionResult<ProdutoDTO> Put(int id, ProdutoDTO produtoDTO)
        {
            if (id != produtoDTO.ProdutoId)
            {
                _logger.LogWarning("Dados inválidos.");
                return BadRequest("Dados inválidos.");
            }

            var produto = _mapper.Map<Produto>(produtoDTO);

            var produtoAtualizado = _uof.ProdutoRepository.Update(produto);
            _uof.Commit();

            var produtoAtualizadoDTO = _mapper.Map<ProdutoDTO>(produtoAtualizado);

            return Ok(produtoAtualizadoDTO);
        }

        [HttpPatch("{id}/UpdatePartial")]
        public ActionResult<ProdutoDTOUpdateResponse> Patch(int id,
            JsonPatchDocument<ProdutoDTOUpdateRequest> patchProdutoDTO)
        {
            if (patchProdutoDTO is null || id <= 0)
                return BadRequest();

            var produto = _uof.ProdutoRepository.Get(p => p.ProdutoId == id);

            if (produto is null)
                return NotFound();

            var produtoUpdateRequest = _mapper.Map<ProdutoDTOUpdateRequest>(produto);

            if (produtoUpdateRequest is null)
                return BadRequest("Ocorreu um erro ao mapear o produto.");

            patchProdutoDTO.ApplyTo(produtoUpdateRequest, ModelState);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var validationResults = ValidateChangeFields(patchProdutoDTO, produtoUpdateRequest);

            if (validationResults.Any())
            {
                foreach (var validationResult in validationResults )
                {
                    ModelState.AddModelError(validationResult.MemberNames.First(), validationResult.ErrorMessage);
                }

                return BadRequest(ModelState);
            }

            _mapper.Map(produtoUpdateRequest, produto);

            _uof.ProdutoRepository.Update(produto);
            _uof.Commit();

            return Ok(_mapper.Map<ProdutoDTOUpdateResponse>(produto));
        }

        // Rota: api/produtos/id
        [HttpDelete("{id:int}")]
        public ActionResult<ProdutoDTO> Delete(int id)
        {
            var produto = _uof.ProdutoRepository.Get(p => p.ProdutoId == id);

            if (produto is null)
                return NotFound("Produto não encontrado.");

            var produtoDeletado = _uof.ProdutoRepository.Delete(produto);
            _uof.Commit();

            var produtoDeletadoDTO = _mapper.Map<ProdutoDTO>(produtoDeletado);

            return Ok(produtoDeletadoDTO);
        }

        private List<ValidationResult> ValidateChangeFields(JsonPatchDocument<ProdutoDTOUpdateRequest> patchDoc, ProdutoDTOUpdateRequest produtoUpdateRequest)
        {
            var validationResults = new List<ValidationResult>();

            foreach (var operation in patchDoc.Operations)
            {
                if (operation.path.Equals("/estoque", StringComparison.OrdinalIgnoreCase))
                {
                    var context = new ValidationContext(produtoUpdateRequest) { MemberName = nameof(produtoUpdateRequest.Estoque) };

                    Validator.TryValidateProperty(produtoUpdateRequest.Estoque, context, validationResults);
                }
                else if (operation.path.Equals("/datacadastro", StringComparison.OrdinalIgnoreCase))
                {
                    var context = new ValidationContext(produtoUpdateRequest) { MemberName = nameof(produtoUpdateRequest.DataCadastro) };

                    Validator.TryValidateProperty(produtoUpdateRequest.DataCadastro, context, validationResults);

                    if (produtoUpdateRequest.DataCadastro.Date <= DateTime.Now.Date)
                    {
                        validationResults.Add(new ValidationResult("A data deve ser maior ou igual a data atual.", new[] { nameof(produtoUpdateRequest.DataCadastro) }));
                    }
                }
            }

            return validationResults;
        }
    }
}
