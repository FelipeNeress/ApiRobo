using ApiRobo.Domain.Models;
using ApiRobo.Repositories.Repositorio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiRobo.Service
{
    public class ProdutoService
    {
        private readonly ProdutoRepositorio _repositorio;
        public ProdutoService(ProdutoRepositorio repositorio)
        {
            _repositorio = repositorio;
        }
        public List<Produto> Listar(string? titulo)
        {
            try
            {
                _repositorio.AbrirConexao();
                return _repositorio.ListarProdutos(titulo);
            }
            finally
            {
                _repositorio.FecharConexao();
            }
        }
        public void Inserir()
        {
            try
            {
                _repositorio.AbrirConexao();
                _repositorio.Inserir();
            }
            finally
            {
                _repositorio.FecharConexao();
            }
        }
    }
}
