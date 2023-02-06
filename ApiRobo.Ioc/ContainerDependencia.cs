using ApiRobo.Repositories.Repositorio;
using ApiRobo.Service;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiRobo.Ioc
{
    public class ContainerDependencia
    {
        public static void RegistrarServicos(IServiceCollection services)
        {
            //repositorios
            services.AddScoped<ProdutoRepositorio, ProdutoRepositorio>();

            //services
            services.AddScoped<ProdutoService, ProdutoService>();
        }
    }
}
