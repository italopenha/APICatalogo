using APICatalogo.Context;
using APICatalogo.Models;
using APICatalogo.Repositories.Interfaces;

namespace APICatalogo.Repositories;

public class ProdutoRepository : IProdutoRepository
{
    private readonly AppDbContext _context;

    public ProdutoRepository(AppDbContext context)
    {
        _context = context;
    }

    public IQueryable<Produto> GetProdutos()
    {
        return _context.Produtos;
    }

    public Produto GetProduto(int id)
    {
        var produto = _context.Produtos.FirstOrDefault(p => p.ProdutoId == id);

        if (produto is null)
            throw new InvalidOperationException("O produto é nulo");

        return produto;
    }

    public Produto Create(Produto produto)
    {
        if (produto is null)
            throw new InvalidOperationException("O produto é nulo");

        _context.Produtos.Add(produto);
        _context.SaveChanges();
        return produto;
    }

    public bool Update(Produto produto)
    {
        if (produto is null)
            throw new InvalidOperationException("O produto é nulo");

        if (_context.Produtos.Any(p => p.ProdutoId == produto.ProdutoId))
        {
            _context.Produtos.Update(produto);
            _context.SaveChanges();
            return true;
        }

        return false;
    }

    public bool Delete(int id)
    {
        var produto = _context.Produtos.Find(id);

        if (produto is not null)
        {
            _context.Produtos.Remove(produto);
            _context.SaveChanges();
            return true;
        }

        return false;
    } 
}
