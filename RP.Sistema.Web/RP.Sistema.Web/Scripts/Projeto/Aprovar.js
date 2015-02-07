
var row = null;
var data = null;
$(function () {


    $('form').submit(function (e) {
        if ($("#status").val() == "Vendido") {
            Parcela.inicializar();
            return $('#dlg-pagamento').modal(options);
        } else {
            return true;
        }
    });

    $('#tblProduto').tGrid({
        searchable: false,
        addMode: "append",
        pager: {
            enabled: true,
            size: 10
        },
        onCreate: function (grid) {
            var data = window.Functions.getJsonData('#tblProduto_data');
            if (data.length) {
                grid.progressbar();
                var work = new window.ThreadLoop(data);
                var unique;
                work.action = function (item, index, total) {
                    grid.addRow(Produto.estrutura, Produto.formpatData(item));
                    Functions.progressbar.update(grid.progressbar(), index, total);
                };
                work.finish = function (thread) {
                    grid.updateIndex();
                    grid.paginate();
                    Produto.addValorTotal(grid);
                    grid.progressbar().dispose();
                };
                work.start();
            }

        },
        onUpdate: function (grid) {
            $('#tblProduto_count').val(grid.count());
        }
    });

    $('#tblProduto').on({
        click: function ()
        { Produto.selecionar(this); return false; }
    }, 'a[data-event=selecionar]');


    $('#dlg-produto').dialog({
        width: 960
    });

    $('#dlg-pagamento').dialog({
        width: 960
    });

    $('input[data-input=calcular]').on({
        change: function ()
        { Produto.calcular(this); return false; }
    });

    $('input[data-input=calcular-projeto]').on({
        change: function () {
            var tbl = $('#tblProduto').tGrid();
            Produto.addValorTotal(tbl);
            return false;
        }
    });

    $('#atualizar-produto').on({
        click: function () {
            Produto.atualizar(this); return false;
        }
    });

    $('#gerar-parcela').on({
        click: function () {
            Parcela.gerarParcelas(this);
            return false;
        }
    });

    $('#btn-finalizar').on({
        click: function () {
            return Parcela.validarPagamento();
        }
    });

    $('#tblParcela').tGrid({ addMode: "append" });

    $('#tblParcela').on({
        change: function ()
        {
            var tbl = $('#tblParcela').tGrid();
            Parcela.addValorTotal(tbl);
            return false;
        }
    }, 'input[data-input=calcularParcela]');
});

var Parcela = {
    validarPagamento: function () {
        var tbl = $('#tblParcela').tGrid();
        var list = tbl.getRows();
        if (list.length == 0) {
            ShowMessage("Para finalizar a venda deve gerar pelo menos uma parcela", "erro");
            return false;
        }
        var somaParcelas = $('#somaParcelas').val().replaceAll(".", "").replace(",", ".");
        var Projeto_vlVenda = $('#Projeto_vlVenda').val().replaceAll(".", "").replace(",", ".");
        if(parseFloat(somaParcelas) != parseFloat(Projeto_vlVenda)){
            ShowMessage("A soma das parcelas deve ser igual ao valor total do projeto", "erro");
        return false;
        }

        for (var i = 0; i < list.length; i++) {
            var data = tbl.getRowData($(list[i]));
            if (data.dtVencimento == "") {
                ShowMessage("Todas as datas de vencimento das parcelas devem ser preenchidas", "erro");
                return false;
            }
        }
        return true;
    },
    inicializar: function () {
        $('#Projeto_vlVenda').val($('#vlTotalProjeto').val());
        $('#dlg-pagamento').modal('show');
    },
    gerarParcelas: function () {
        var data = $('#Projeto_dtVencimento').val().split("/");
        var d = new Date(data[2] + '/' + data[1] +'/' + data[0]);
        var parcelas = $('#Projeto_qtdParcela').val();
        var vlVenda = $('#Projeto_vlVenda').val().replaceAll(".", "").replace(",", ".");
        var vlParcela = parseFloat(vlVenda) / parseFloat(parcelas);
        var tbl = $('#tblParcela').tGrid();
        tbl.removeAllRows();
        for (var i = 0; i < parcelas; i++) {
            var dat = d.getDate();
            var mon = d.getMonth();
            var year = d.getFullYear();
            var vencimento = $.datepicker.formatDate("dd/mm/yy", d);
            tbl.addRow(Parcela.estrutura, { nrParcela: (i + 1), vlParcela: window.NumberFormat(parseFloat(vlParcela), 2, ",", "."), dtVencimento: vencimento });
            d.setMonth(d.getMonth() + 1);
        }
        window.Functions.handlers.mask();
        window.Functions.handlers.filter();
        window.Functions.handlers.datepicker();
        Parcela.addValorTotal(tbl);
        $('#Projeto_vlVenda').val($('#vlTotalProjeto').val());
        tbl.updateIndex();
        tbl.paginate();
    },

    addValorTotal: function (tbl) {
        var list = tbl.getRows();
        var vlTotal = 0;

        for (var i = 0; i < list.length; i++) {
            var data = tbl.getRowData($(list[i]));
            var totalItem = data.vlParcela.replaceAll(".", "").replace(",", ".");

            vlTotal = parseFloat(vlTotal) + parseFloat(totalItem);
        }

        $('#somaParcelas').val(window.NumberFormat(vlTotal, 2, ',', '.'));
        return false;
    },

    estrutura:
        '<tr class="data">' +
            '<td >' +
                '${nrParcela}' +
                '<input type="hidden" name="Parcelas[0].nrParcela" value="${nrParcela}"/>' +
            '</td>' +
            '<td>' +
                '<select style="margin: 0" data-input="situacao" class="span12" name="Parcelas[0].flFormaPagamento">' +
                    '<option value="Cheque">Cheque</option>' +
                    '<option value="Dinheiro">Dinheiro</option>' +
                    '<option value="Cartão de crédito">Cartão de crédito</option>' +
                    '<option value="Boleto">Boleto</option>' +
                '</select>' +
            '</td>' +
            '<td><input type="text" style="margin: 0" name="Parcelas[0].dsObservacao" value="${dsObservacao}" class="span12"></td>' +
            '<td><input type="text" style="margin: 0" name="Parcelas[0].dtVencimento" value="${dtVencimento}" class="span12 datepicker" mask="99/99/9999" ></td>' +
            '<td><input type="text" style="text-align: right; margin: 0" name="Parcelas[0].vlParcela" data-input="calcularParcela" value="${vlParcela}" filter="floatnumber" class="span12"></td>' +
         '</tr>'
};
var Produto = {

    atualizar: function (el) {
        data.vlLucro = $('.vlLucro').val().replaceAll(".", "").replace(",", ".");
        data.vlVenda = $('.vlVenda').val().replaceAll(".", "").replace(",", ".");
        data.vlLiquido = $('.vlLiquido').val().replaceAll(".", "").replace(",", ".");
        data.comissaoProjetista = $('#comissaoProjetista').val().replaceAll(".", "").replace(",", ".");
        data.comissaoMarceneiro = $('#comissaoMarceneiro').val().replaceAll(".", "").replace(",", ".");
        data.vlProduto = $('.vlProduto').val().replaceAll(".", "").replace(",", ".");
        data.porcentagemProjetista = $('#porcentagemProjetista').val().replaceAll(".", "").replace(",", ".");
        data.porcentagemMarceneiro = $('#porcentagemMarceneiro').val().replaceAll(".", "").replace(",", ".");
        data.margemGanho = $('#margemGanho').val().replaceAll(".", "").replace(",", ".");
        data.vlDesconto = $('#vlDesconto').val().replaceAll(".", "").replace(",", ".");
        data.vlCustoMaterial = data.vlCustoMaterial.replaceAll(".", "").replace(",", ".");
        data.vlLiquidoDesconto = data.vlLiquidoDesconto.replaceAll(".", "").replace(",", ".");
        data.vlLiquidoMaterial = data.vlLiquidoMaterial.replaceAll(".", "").replace(",", ".");
        data.vlTotalMaterial = data.vlTotalMaterial.replaceAll(".", "").replace(",", ".");
        var tbl = $('#tblProduto').tGrid();
        tbl.updateRow(row, Produto.estrutura, Produto.formpatData(data));
        data = null;
        row = null;
        $("#dlg-produto").modal('hide');
        Produto.addValorTotal(tbl);
    },
    formpatData: function (item) {
        var data = {
            comissaoMarceneiro: window.NumberFormat(parseFloat(item.comissaoMarceneiro), 2, ',', '.'),
            comissaoProjetista: window.NumberFormat(parseFloat(item.comissaoProjetista), 2, ',', '.'),
            porcentagemMarceneiro: window.NumberFormat(parseFloat(item.porcentagemMarceneiro), 2, ',', '.'),
            porcentagemProjetista: window.NumberFormat(parseFloat(item.porcentagemProjetista), 2, ',', '.'),
            vlVenda: window.NumberFormat(parseFloat(item.vlVenda), 2, ',', '.'),
            vlProduto: window.NumberFormat(parseFloat(item.vlProduto), 2, ',', '.'),
            vlLucro: window.NumberFormat(parseFloat(item.vlLucro), 2, ',', '.'),
            vlLiquido: window.NumberFormat(parseFloat(item.vlLiquido), 2, ',', '.'),
            vlLiquidoDesconto: window.NumberFormat(parseFloat(item.vlLiquidoDesconto), 2, ',', '.'),
            vlDesconto: window.NumberFormat(parseFloat(item.vlDesconto), 2, ',', '.'),
            margemGanho: window.NumberFormat(parseFloat(item.margemGanho), 2, ',', '.'),
            vlLiquidoMaterial: window.NumberFormat(parseFloat(item.vlLiquidoMaterial), 2, ',', '.'),
            vlTotalMaterial: window.NumberFormat(parseFloat(item.vlTotalMaterial), 2, ',', '.'),
            vlCustoMaterial: window.NumberFormat(parseFloat(item.vlCustoMaterial), 2, ',', '.'),
            nome: item.nome,
            descricao: item.descricao,
            idProjetista: item.idProjetista,
            idMarceneiro: item.idMarceneiro,
            idProduto: item.idProduto,
            projetista: item.projetista,
            marceneiro: item.marceneiro,
        }
        return data;
    },

    calcular: function (el) {

        //dados produto desconto
        var margemGanho = $("#margemGanho").val().replaceAll(".", "").replace(",", ".");
        var vlTotalMaterial = $(".vlTotalMaterial").val().replaceAll(".", "").replace(",", ".");
        var vlCustoMaterial = $(".vlCustoMaterial").val().replaceAll(".", "").replace(",", ".");
        var porcentagemProjetista = $("#porcentagemProjetista").val().replaceAll(".", "").replace(",", ".");
        var porcentagemMarceneiro = $("#porcentagemMarceneiro").val().replaceAll(".", "").replace(",", ".");
        var vlDesconto = $("#vlDesconto").val().replaceAll(".", "").replace(",", ".");
        var vlLiquido = ((parseFloat(margemGanho)) / 100) * parseFloat(vlTotalMaterial);
        var vlProduto = parseFloat(vlTotalMaterial) + parseFloat(vlLiquido);
        var vlVenda = parseFloat(vlProduto) - parseFloat(vlDesconto);

        var comissaoProjetista = ((parseFloat(porcentagemProjetista)) / 100) * parseFloat(vlVenda);
        var comissaoMarceneiro = ((parseFloat(porcentagemMarceneiro)) / 100) * parseFloat(vlVenda);
        var gastos = (parseFloat(comissaoProjetista) + parseFloat(comissaoMarceneiro) + parseFloat(vlCustoMaterial));
        var vlLucro = parseFloat(vlVenda) - parseFloat(gastos);

        $(".vlLucro").val(window.NumberFormat(parseFloat(vlLucro), 2, ",", "."));
        $(".vlVenda").val(window.NumberFormat(parseFloat(vlVenda), 2, ",", "."));
        $(".vlLiquido").val(window.NumberFormat(parseFloat(vlLiquido), 2, ",", "."));
        $(".vlProduto").val(window.NumberFormat(parseFloat(vlProduto), 2, ",", "."));
        $("#comissaoProjetista").val(window.NumberFormat(parseFloat(comissaoProjetista), 2, ",", "."));
        $("#comissaoMarceneiro").val(window.NumberFormat(parseFloat(comissaoMarceneiro), 2, ",", "."));
    },
    selecionar: function (el) {
        var tbl = $('#tblProduto').tGrid();

        data = tbl.getRowData($(el).parents('tr'));
        row = $(el).parents('tr.data');

        $("#nomeProduto").val(data.nome);
        $("#vlTotalMaterial").val(data.vlTotalMaterial);
        $("#vlCustoMaterial").val(data.vlCustoMaterial);
        //dados projetista
        $("#porcentagemProjetista").val(data.porcentagemProjetista);
        $("#projetista").text(data.projetista);
        $("#comissaoProjetista").val(data.comissaoProjetista);
        //dados projetista
        $("#porcentagemMarceneiro").val(data.porcentagemMarceneiro);
        $("#marceneiro").text(data.marceneiro);
        $("#comissaoMarceneiro").val(data.comissaoMarceneiro);
        //dados material
        $(".vlLiquido").val(data.vlLiquido);
        $(".vlTotalMaterial").val(data.vlTotalMaterial);
        $(".vlCustoMaterial").val(data.vlCustoMaterial);
        $(".vlLiquidoMaterial").val(data.vlLiquidoMaterial);
        $(".vlMargemGanho").val(data.vlLiquidoMaterial);

        //dados produto margem de ganho
        $("#margemGanho").val(data.margemGanho);
        $(".vlVenda").val(data.vlVenda);

        //dados produto desconto
        $("#vlLiquidoDesconto").val(data.vlLiquidoDesconto);
        $("#vlDesconto").val(data.vlDesconto);
        $(".vlProduto").val(data.vlProduto);
        $(".vlLucro").val(data.vlLucro);
        $("#dlg-produto").modal('show');
    },

    addValorTotal: function (tbl) {
        var list = tbl.getRows();
        var vlDesconto = 0;
        var vlProduto = 0;
        var vlVenda = 0;

        var vlCustoMaterial = 0;
        var comissaoProjetista = 0;
        var comissaoMarceneiro = 0;

        var vlLiquidoMaterial = 0;
        var vlLiquido = 0;

        var vlLucro = 0;

        for (var i = 0; i < list.length; i++) {
            var data = tbl.getRowData($(list[i]));
            vlDesconto = parseFloat(vlDesconto) + parseFloat(data.vlDesconto.replaceAll(".", "").replace(",", "."));
            vlProduto = parseFloat(vlProduto) + parseFloat(data.vlProduto.replaceAll(".", "").replace(",", "."));
            vlVenda = parseFloat(vlVenda) + parseFloat(data.vlVenda.replaceAll(".", "").replace(",", "."));

            vlCustoMaterial = parseFloat(vlCustoMaterial) + parseFloat(data.vlCustoMaterial.replaceAll(".", "").replace(",", "."));
            comissaoProjetista = parseFloat(comissaoProjetista) + parseFloat(data.comissaoProjetista.replaceAll(".", "").replace(",", "."));
            comissaoMarceneiro = parseFloat(comissaoMarceneiro) + parseFloat(data.comissaoMarceneiro.replaceAll(".", "").replace(",", "."));

            vlLiquidoMaterial = parseFloat(vlLiquidoMaterial) + parseFloat(data.vlLiquidoMaterial.replaceAll(".", "").replace(",", "."));
            vlLiquido = parseFloat(vlLiquido) + parseFloat(data.vlLiquido.replaceAll(".", "").replace(",", "."));

            vlLucro = parseFloat(vlLucro) + parseFloat(data.vlLucro.replaceAll(".", "").replace(",", "."));
        }
        var porcentagemVendedor = $("#porcentagemVendedor").val().replaceAll(".", "").replace(",", ".");
        var comissaoVendedor = ((parseFloat(porcentagemVendedor)) / 100) * parseFloat(vlVenda);
        var vlCustosDesenvolvimento = parseFloat(vlCustoMaterial) + parseFloat(comissaoProjetista) + parseFloat(comissaoMarceneiro);
        var vlCustos = $('#vlCustos').val().replaceAll(".", "").replace(",", ".");

        vlLucro = parseFloat(vlLucro) - (parseFloat(comissaoVendedor) + parseFloat(vlCustos));

        $('#vlProjeto').val(window.NumberFormat(vlProduto, 2, ',', '.'));
        $('#vlDescontoProjeto').val(window.NumberFormat(vlDesconto, 2, ',', '.'));

        $('.vlTotalProjeto').val(window.NumberFormat(vlVenda, 2, ',', '.'));
        $('#comissaoVendedor').val(window.NumberFormat(comissaoVendedor, 2, ',', '.'));
        $('#vlCustosDesenvolvimento').val(window.NumberFormat(vlCustosDesenvolvimento, 2, ',', '.'));

        $('#vlTotalMargemGanhoMaterial').val(window.NumberFormat(vlLiquidoMaterial, 2, ',', '.'));
        $('#vlTotalMargemProduto').val(window.NumberFormat(vlLiquido, 2, ',', '.'));

        $('#vlLucroProjeto').val(window.NumberFormat(vlLucro, 2, ',', '.'));
        tbl.updateIndex();
        tbl.paginate();
    },

    estrutura:
        '<tr class="data">' +
            '<td >' +
                '<a href="#" class="btn" title="Selecionar" data-event="selecionar"><i class="icon-ok"></i></a>' +
                '<input type="hidden" name="Produtos[0].idProduto" value="${idProduto}"/>' +
                '<input type="hidden" name="Produtos[0].idProjetista" value="${idProjetista}"/>' +
                '<input type="hidden" name="Produtos[0].idMarceneiro" value="${idMarceneiro}"/>' +
                '<input type="hidden" name="Produtos[0].margemGanho" value="${margemGanho}"/>' +
                '<input type="hidden" name="Produtos[0].vlProduto" value="${vlProduto}"/>' +
                '<input type="hidden" name="Produtos[0].vlLiquidoDesconto" value="${vlLiquidoDesconto}"/>' +
                '<input type="hidden" name="Produtos[0].vlLiquido" value="${vlLiquido}"/>' +
                '<input type="hidden" name="Produtos[0].vlDesconto" value="${vlDesconto}"/>' +
                '<input type="hidden" name="Produtos[0].vlVenda" value="${vlVenda}"/>' +
                '<input type="hidden" name="Produtos[0].nome" value="${nome}"/>' +
                '<input type="hidden" name="Produtos[0].vlLucro" value="${vlLucro}"/>' +
                '<input type="hidden" name="Produtos[0].vlCustoMaterial" value="${vlCustoMaterial}"/>' +
                '<input type="hidden" name="Produtos[0].vlTotalMaterial" value="${vlTotalMaterial}"/>' +
                '<input type="hidden" name="Produtos[0].vlLiquidoMaterial" value="${vlLiquidoMaterial}"/>' +
                '<input type="hidden" name="Produtos[0].descricao" value="${descricao}"/>' +
                '<input type="hidden" name="Produtos[0].porcentagemProjetista" value="${porcentagemProjetista}"/>' +
                '<input type="hidden" name="Produtos[0].projetista" value="${projetista}"/>' +
                '<input type="hidden" name="Produtos[0].comissaoProjetista" value="${comissaoProjetista}"/>' +
                '<input type="hidden" name="Produtos[0].porcentagemMarceneiro" value="${porcentagemMarceneiro}"/>' +
                '<input type="hidden" name="Produtos[0].marceneiro" value="${marceneiro}"/>' +
                '<input type="hidden" name="Produtos[0].comissaoMarceneiro" value="${comissaoMarceneiro}"/>' +
            '</td>' +
            '<td>${nome}</td>' +
            '<td style="text-align: right">${vlProduto}</td>' +
            '<td style="text-align: right">${vlDesconto}</td>' +
            '<td style="text-align: right">${vlVenda}' +
            '</td>' +
         '</tr>'
};
