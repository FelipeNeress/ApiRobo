using Microsoft.Extensions.Configuration;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiRobo.Repositories
{
    public class Contexto
    {
        internal readonly MySqlConnection _conn;
        public Contexto(IConfiguration configuration)
        {

            _conn = new MySqlConnection(configuration["DbCredentials"]);
        }

        public void AbrirConexao()
        {
            _conn.Open();
        }

        public void FecharConexao()
        {
            _conn.Close();
        }
    }
}
