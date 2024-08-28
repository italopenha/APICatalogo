using APICatalogo.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APICatalogoxUnitTests.UnitTests;

public class GetProdutoUnitTests : IClassFixture<ProdutosUnitTestController>
{
    private readonly ProdutosController _controller;

    public GetProdutoUnitTests(ProdutosUnitTestController controller)
    {
        _controller = new ProdutosController(controller.repository, controller.mapper);
    }

    [Fact]
    public async Task GetProdutoById_OkResult()
    {
        // Arrange
        var prodId = 2;

        // Act
        var data = await _controller.GetAsync(prodId);

        // Assert (xUnit)
        // var okResult = Assert.IsType<OkObjectResult>(data.Result);
        // Assert.Equal(200, okResult.StatusCode);

        // Assert (fluentassertions)
        data.Result.Should().BeOfType<ObjectResult>()
            .Which.StatusCode.Should().Be(200);
    }
}
