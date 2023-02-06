using ApiRobo.Domain.Models;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;
using static Robo.Program;

namespace Robo
{
    public class ProdutoScraper
    {
        private readonly string _urlBase;

        public ProdutoScraper(string urlBase)
        {
            _urlBase = urlBase;
        }

        public List<Produto> ObterProdutos()
        {
            var client = new HttpClient();
            var result = client.GetAsync("https://www.shopb.com.br/playstation?pagina=1").Result;
            if (!result.IsSuccessStatusCode)
                throw new Exception("Erro ao conectar ao site");

            Utf8EncodingProvider.Register();
            var html = result.Content.ReadAsStringAsync().Result;

            var totalPagina = 7;
            var paginas = Enumerable.Range(1, totalPagina);

            var listaProdutos = new List<Produto>();
          
            foreach (var pagina in paginas)
            {
                result = client.GetAsync(_urlBase + "playstation?pagina=" + pagina).Result;
                html = result.Content.ReadAsStringAsync().Result;

                var doc = new HtmlDocument();
                doc.LoadHtml(html);

                var produtos = doc.DocumentNode.SelectNodes("//li[contains(@class, 'span3')]");
                if (produtos is null)
                    throw new Exception("Não foi possível encontrar elementos de produto na página");


                foreach (var produto in produtos)
                {
                    var elementoPreco = produto.SelectNodes(".//strong[contains(@class, 'preco-promocional cor-principal ')]");
                    if (elementoPreco is null)
                        continue;

                    var preco = decimal.Parse(elementoPreco[0].InnerText.Replace("R$", "").Replace(" ", ""));
                    var elementoA = produto.Descendants("a").First();
                    var linkProduto = elementoA.Attributes["href"].Value;
                    var linkCompleto = linkProduto.Replace("./", "/");
                    var titulo = produto.SelectNodes(".//a[contains(@class, 'nome-produto cor-secundaria')]").First().InnerText.Replace("\"", "").Replace("\n", string.Empty);

                    listaProdutos.Add(new Produto
                    {
                        Titulo = titulo,
                        Link = linkCompleto,
                        Preco = preco
                    });
                }
            }
            return listaProdutos;
        }
        public class Utf8EncodingProvider : EncodingProvider
        {
            public override Encoding GetEncoding(string name)
            {
                return name == "utf8" ? Encoding.UTF8 : null;
            }

            public override Encoding GetEncoding(int codepage)
            {
                return null;
            }

            public static void Register()
            {
                Encoding.RegisterProvider(new Utf8EncodingProvider());
            }
        }
    }
}
