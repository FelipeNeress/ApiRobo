$(document).ready(function () {
    ListarProdutos();
})

var tabelaProduto;
var urlBaseApi = "https://localhost:44318";


function LimparCorpoTabelaProdutos() {
    var componenteSelecionado = $('#tabelaProduto tbody');
    componenteSelecionado.html('');
}

function ListarProdutos() {
    var rotaApi = '/produto';

    $.ajax({
        url: urlBaseApi + rotaApi,
        method: 'GET',
        dataType: "json"
    }).done(function (resultado) {
        ConstruirTabela(resultado);
    }).fail(function (err, errr, errrr) {

    });
}

function ConstruirTabela(linhas) {

    var htmlTabela = '';

    $(linhas).each(function (index, linha) {
        var precoFormatado = linha.preco.toFixed(2);
        var precoAntigoFormatado = linha.precoAntigo.toFixed(2);

        var precoClasse = "";
        if (linha.precoAntigo == 0) {
            precoClasse = "text-primary";
        } else if (linha.preco > linha.precoAntigo) {
            precoClasse = "text-danger";
        } else if (linha.preco < linha.precoAntigo) {
            precoClasse = "text-success";
        }

        var diferenca = ((linha.preco - linha.precoAntigo) / linha.precoAntigo) * 100;
        var diferencaFormatada = (isFinite(diferenca)) ? diferenca.toFixed(2) : "Não há registro suficiente para cálculo da diferença em ";
        var sinal = (diferenca > 0) ? '+' : '';

        htmlTabela += `<tr><th><a href="${linha.link}" target="_blank">${linha.titulo}</a></th>
                        <td class="${precoClasse}">${precoFormatado}</td>
                        <td>${(linha.precoAntigo == 0) ? "Não há registro de preço antigo" : precoAntigoFormatado}</td>
                        <td>${sinal}${diferencaFormatada}%</td>
                        <td>${formatarData(linha.dataBusca)}</td></tr>`
    });

    $('#tabelaProduto tbody').html(htmlTabela);
    if (tabelaProduto == undefined) {
        tabelaProduto = $('#tabelaProduto').DataTable({
            language: {
                url: 'https://cdn.datatables.net/plug-ins/1.13.2/i18n/pt-BR.json'
            }
        });
    }
}

function AtualizarProdutos() {
    var rotaApi = '/produto';

    $.ajax({
        url: urlBaseApi + rotaApi,
        method: 'POST',
        
    }).done(function () {
        ListarProdutos();
        Swal.fire({
            position: 'center',
            icon: 'success',
            title: 'Tabela atualizada com sucesso',
            showConfirmButton: false,
            timer: 1500
        });
    }).fail(function (err, errr, errrr) {
        Swal.fire({
            icon: 'error',
            title: 'Oops...',
            text: 'Aconteceu algum erro inesperado!',
        });

    });
}

function formatarData(data) {

    let dataFormatada = new Date(data);
    let options = {
        day: "numeric",
        month: "numeric",
        year: "numeric"
    };

    return dataFormatada.toLocaleDateString("pt-BR", options);
}