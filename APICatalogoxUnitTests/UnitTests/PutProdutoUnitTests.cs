using APICatalogo.Controllers;
using APICatalogo.DTOs;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APICatalogoxUnitTests.UnitTests;

public class PutProdutoUnitTests : IClassFixture<ProdutosUnitTestController>
{
    private readonly ProdutosController _controller;

    public PutProdutoUnitTests(ProdutosUnitTestController controller)
    {
        _controller = new ProdutosController(controller.repository, controller.mapper);
    }

    [Fact]
    public async Task PutProduto_Return_OkResult()
    {
        // Arrange
        var prodId = 5;

        var updateProdutoDTO = new ProdutoDTO
        {
            ProdutoId = prodId,
            Nome = "Produto Atualizado - Testes",
            Descricao = "Minha descrição",
            ImagemUrl = "imagem.png",
            CategoriaId = 2
        };

        // Act
        var result = await _controller.PutAsync(prodId, updateProdutoDTO) as ActionResult<ProdutoDTO>;

        // Assert
        result.Should().NotBeNull();
        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task PutProduto_Return_BadRequest()
    {
        // Arrange
        var prodId = 1000;

        var meuProduto = new ProdutoDTO
        {
            ProdutoId = 5,
            Nome = "Produto Atualizado - Testes",
            Descricao = "Minha descrição alterada",
            ImagemUrl = "imagem.png",
            CategoriaId = 2
        };

        // Act
        var data = await _controller.PutAsync(prodId, meuProduto);

        // Assert
        data.Result.Should().BeOfType<BadRequestObjectResult>().Which.StatusCode.Should().Be(400); ;
    }
}
