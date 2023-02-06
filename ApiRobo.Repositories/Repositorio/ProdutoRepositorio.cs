using ApiRobo.Domain.Models;
using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using Robo;
using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace ApiRobo.Repositories.Repositorio
{
    public class ProdutoRepositorio : Contexto
    {
        private readonly ProdutoScraper _produtoScraper;
        public ProdutoRepositorio(IConfiguration configuration) : base(configuration)
        {
            _produtoScraper = new ProdutoScraper("https://www.shopb.com.br/");
        }
        public List<Produto> ListarProdutos(string? titulo)
        {
            string comandoSql = @"SELECT Titulo, Preco, PrecoAntigo, Link, DataBusca FROM Produtos";

            if (!string.IsNullOrWhiteSpace(titulo))
                comandoSql += " WHERE Titulo LIKE @Titulo";

            using (var cmd = new MySqlCommand(comandoSql, _conn))
            {
                if (!string.IsNullOrWhiteSpace(titulo))
                    cmd.Parameters.AddWithValue("@Titulo", "%" + titulo + "%");

                using (var rdr = cmd.ExecuteReader())
                {
                    var produtos = new List<Produto>();
                    while (rdr.Read())
                    {
                        var produto = new Produto();
                        produto.Titulo = Convert.ToString(rdr["Titulo"]);
                        produto.Preco = Convert.ToDecimal(rdr["Preco"]);
                        produto.PrecoAntigo = rdr["PrecoAntigo"] is DBNull ? 0 : Convert.ToDecimal(rdr["PrecoAntigo"]);
                        produto.Link = Convert.ToString(rdr["Link"]);
                        produto.DataBusca = Convert.ToDateTime(rdr["DataBusca"]);
                        produtos.Add(produto);
                    }
                    return produtos;
                }
            }
        }
        public void Inserir()
        {
            var produtos = _produtoScraper.ObterProdutos().ToList();

            foreach (var produto in produtos)
            {
                if (ProdutoExiste(produto.Titulo, produto.Preco))
                {
                    AtualizarProduto(produto);
                }
                else
                {
                    InserirProduto(produto);
                }
            }
        }

        private bool ProdutoExiste(string titulo, decimal preco)
        {
            string comandoSql = "SELECT COUNT(*) FROM Produtos WHERE Titulo = @Titulo AND Preco = @Preco";

            using (MySqlCommand command = new MySqlCommand(comandoSql, _conn))
            {
                command.Parameters.AddWithValue("@Titulo", titulo);
                command.Parameters.AddWithValue("@Preco", preco.ToString("N2", CultureInfo.InvariantCulture));

                int count = Convert.ToInt32(command.ExecuteScalar());
                return count > 0;
            }
        }

        private void AtualizarProduto(Produto produto)
        {
            string comandoSql = @"UPDATE Produtos SET Preco = @Preco, PrecoAntigo = @PrecoAntigo, DataBusca = @DataBusca WHERE Titulo = @Titulo";
            decimal precoAntigo = ObterPrecoAtualDoProduto(produto.Titulo);
            if (precoAntigo != produto.Preco)
            {
                using (MySqlCommand command = new MySqlCommand(comandoSql, _conn))
                {
                    command.Parameters.AddWithValue("@Titulo", produto.Titulo);
                    command.Parameters.AddWithValue("@Preco", produto.Preco.ToString("N2", CultureInfo.InvariantCulture));
                    command.Parameters.AddWithValue("@PrecoAntigo", precoAntigo.ToString("N2", CultureInfo.InvariantCulture));
                    command.Parameters.AddWithValue("@DataBusca", produto.DataBusca);

                    command.ExecuteNonQuery();
                }
            }
        }

        private decimal ObterPrecoAtualDoProduto(string titulo)
        {
            string comandoSql = @"SELECT Preco FROM Produtos WHERE Titulo = @Titulo";
            decimal preco = 0;
            using (MySqlCommand command = new MySqlCommand(comandoSql, _conn))
            {
                command.Parameters.AddWithValue("@Titulo", titulo);

                using (MySqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        preco = Convert.ToDecimal(reader["Preco"], CultureInfo.InvariantCulture);
                    }
                }
            }

            return preco;
        }

        private void InserirProduto(Produto produto)
        {
            string comandoSql = @"INSERT INTO Produtos (Titulo, Preco, Link, DataBusca) VALUES (@Titulo, @Preco, @Link, @DataBusca)";

            using (MySqlCommand command = new MySqlCommand(comandoSql, _conn))
            {
                command.Parameters.AddWithValue("@Titulo", produto.Titulo);
                command.Parameters.AddWithValue("@Preco", produto.Preco.ToString("N2", CultureInfo.InvariantCulture));
                command.Parameters.AddWithValue("@Link", produto.Link);
                command.Parameters.AddWithValue("@DataBusca", produto.DataBusca);

                command.ExecuteNonQuery();
            }
        }
     
    }
}
