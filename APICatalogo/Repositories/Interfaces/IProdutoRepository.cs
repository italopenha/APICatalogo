using APICatalogo.Models;
using APICatalogo.Pagination;

namespace APICatalogo.Repositories.Interfaces;

public interface IProdutoRepository : IRepository<Produto>
{
    //IEnumerable<Produto> GetProdutos(ProdutosParameters produtosParams);
    PagedList<Produto> GetProdutos(ProdutosParameters produtosParams);
    PagedList<Produto> GetProdutosFiltroPreco(ProdutosFiltroPreco produtosFiltroParams);
    IEnumerable<Produto> GetProdutosPorCategoria(int id);
}
