using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APICatalogo.Migrations
{
    /// <inheritdoc />
    public partial class PopulaProdutos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder mb)
        {
            mb.Sql("Insert Into Produtos(Nome,Descricao,Preco,ImagemUrl,Estoque,DataCadastro,CategoriaId)" + 
                "Values('Coca-Cola Diet','Refrigerante de Cola 350ml',5.45,'cocacola.jpg',50,now(),1)");

            mb.Sql("Insert Into Produtos(Nome,Descricao,Preco,ImagemUrl,Estoque,DataCadastro,CategoriaId)" +
                "Values('Lanche de Atum','Lanche de Atum com Maionese',8.50,'atum.jpg',10,now(),2)");

            mb.Sql("Insert Into Produtos(Nome,Descricao,Preco,ImagemUrl,Estoque,DataCadastro,CategoriaId)" +
                "Values('Pudim de 100g','Pudim de Leite Condensado 100g',6.75,'pudim.jpg',20,now(),3)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder mb)
        {
            mb.Sql("Delete from Produtos");
        }
    }
}
