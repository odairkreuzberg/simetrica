$(function () {

    $('#porcentagemVendedor, #desconto').on({
        change: function ()
        { Produto.calcularPorcentagemVendedor(this); return false; }
    });

    $('#tblExtrato').on({
        change: function ()
        { Produto.calcular(this); return false; }
    }, 'input[data-input=calcular]');

    $('#tblExtrato').tGrid({
        onCreate: function (grid) {
            var data = window.Functions.getJsonData('#tblProduto_data');
            if (data.length) {
                grid.progressbar();
                var work = new window.ThreadLoop(data);
                var unique;
                work.action = function (item, index, total) {
                    grid.addRow(Produto.extrato, item);

                    Functions.progressbar.update(grid.progressbar(), index, total);
                };
                work.finish = function (thread) {
                    grid.updateIndex();
                    grid.paginate();
                    grid.progressbar().dispose();
                    window.Functions.handlers.filter();
                };
                work.start();
            }

        },
        onUpdate: function (grid) {
            $('#tblProduto_count').val(grid.count());
        }
    });
});

var Produto = {

    calcularPorcentagemVendedor: function (el) {
        var valor = $('#valor').val().replaceAll(".", "").replace(",", ".");
        var desconto = $('#desconto').val().replaceAll(".", "").replace(",", ".");
        var porcentagem = $('#porcentagemVendedor').val().replaceAll(".","").replace(",", ".");
        var totalCusto = $('#totalCusto').val().replaceAll(".", "").replace(",", ".");
        var desconto = $('#desconto').val().replace(",", ".");

        var total = parseFloat(valor) - parseFloat(desconto);

        var comissaoVendedor = ((parseFloat(porcentagem)) / 100) * Number(total);
        var despesaProjeto = parseFloat(totalCusto) + parseFloat(desconto) + parseFloat(comissaoVendedor);


        var custoProjeto = parseFloat(total) - parseFloat(despesaProjeto);

        $('#comissaoVendedor').val(window.NumberFormat(parseFloat(comissaoVendedor), 2, ",", "."));
        $('#despesaProjeto').val(window.NumberFormat(parseFloat(despesaProjeto), 2, ",", "."));
        $('#total').val(window.NumberFormat(parseFloat(total), 2, ",", "."));
        $('.valor-total').val(window.NumberFormat(parseFloat(total), 2, ",", "."));
        $('#custoProjeto').val(window.NumberFormat(parseFloat(custoProjeto), 2, ",", "."));
    },

    calcular: function (el) {
        var tbl = $('#tblExtrato').tGrid();
        var data = tbl.getRowData($(el).parents('tr'));
        var row = $(el).parents('tr.data');
        data = Produto.calcularProduto(data);

        tbl.updateRow(row, Produto.extrato, data);
    },
    calcularProduto: function (data) {
        var liquidoProduto = data.liquidoProduto.replaceAll(".", "").replace(",", ".");
        var totalMaterial = data.totalMaterial.replaceAll(".", "").replace(",", ".");
        var custoMaterial = data.custoMaterial.replaceAll(".", "").replace(",", ".");
        var liquidoMaterial = data.liquidoMaterial.replaceAll(".", "").replace(",", ".");
        var margemGanho = data.margemGanho.replaceAll(".", "").replace(",", ".");

        var porcentagemMarceneiro = data.porcentagemMarceneiro.replaceAll(".", "").replace(",", ".");
        var porcentagemProjetista = data.porcentagemProjetista.replaceAll(".", "").replace(",", ".");

        var liquidoProduto = ((parseFloat(margemGanho)) / 100) * parseFloat(totalMaterial);
        var totalProduto = parseFloat(totalMaterial) + parseFloat(liquidoProduto);
        var totalLiquido = parseFloat(liquidoProduto) + parseFloat(liquidoMaterial);

        var comissaoMarceneiro = ((parseFloat(porcentagemMarceneiro)) / 100) * parseFloat(totalProduto);
        var comissaoProjetista = ((parseFloat(porcentagemProjetista)) / 100) * parseFloat(totalProduto);

        var custoProduto = parseFloat(custoMaterial) + parseFloat(comissaoMarceneiro) + parseFloat(comissaoProjetista);

        data.comissaoMarceneiro = comissaoMarceneiro;
        data.comissaoProjetista = comissaoProjetista;
        data.liquidoProduto = liquidoProduto;
        data.totalProduto = totalProduto;
        data.custoProduto = custoProduto;
        data.totalLiquido = totalLiquido;
        data.lucro = parseFloat(totalProduto) - parseFloat(custoProduto);
        return data;
    },

    addValorTotal: function (tbl) {
        var tbl = $('#tblExtrato').tGrid();
        var list = tbl.getRows();
        var vlTotal = 0;

        for (var i = 0; i < list.length; i++) {
            var data = tbl.getRowData($(list[i]));
            data = Produto.calcularProduto(data);
        }
    },

    extrato:
        '<tr class="data">' +
            '<td >' +
                '<input type="hidden" name="Produtos[0].nome" value="${nome}"/>' +
                '<input type="hidden" name="Produtos[0].descricao" value="${descricao}"/>' +
                '<input type="hidden" name="Produtos[0].idProjetista" value="${idProjetista}"/>' +
                '<input type="hidden" name="Produtos[0].idMarceneiro" value="${idMarceneiro}"/>' +
                '<input type="hidden" name="Produtos[0].idProduto" value="${idProduto}"/>' +
                '<input type="hidden" name="Produtos[0].projetista" value="${projetista}"/>' +
                '<input type="hidden" name="Produtos[0].marceneiro" value="${marceneiro}"/>' +
                '<input type="hidden" name="Produtos[0].comissaoMarceneiro" value="${comissaoMarceneiro}"/>' +
                '<input type="hidden" name="Produtos[0].comissaoProjetista" value="${comissaoProjetista}"/>' +
                '<input type="hidden" name="Produtos[0].porcentagemMarceneiro" value="${porcentagemMarceneiro}"/>' +
                '<input type="hidden" name="Produtos[0].porcentagemProjetista" value="${porcentagemProjetista}"/>' +
                '<input type="hidden" name="Produtos[0].liquidoMaterial" value="${liquidoMaterial}"/>' +
                '<input type="hidden" name="Produtos[0].custoMaterial" value="${custoMaterial}"/>' +
                '<input type="hidden" name="Produtos[0].totalMaterial" value="${totalMaterial}"/>' +
                '<input type="hidden" name="Produtos[0].custoProduto" value="${custoProduto}"/>' +
                '<input type="hidden" name="Produtos[0].liquidoProduto" value="${liquidoProduto}"/>' +
                '<input type="hidden" name="Produtos[0].totalProduto" value="${totalProduto}"/>' +
                '<input type="hidden" name="Produtos[0].totalLiquido" value="${totalLiquido}"/>' +
                '<input type="hidden" name="Produtos[0].margemGanho" value="${margemGanho}"/>' +
                '<input type="hidden" name="Produtos[0].lucro" value="${lucro}"/>' +
                '<div style="display: table; width: 100%; margin: 0">' +
                    '<div style="display: table-row">' +
                    '    <div class="dv-cell-head" style="width:40%;border: 0 none; color:#fff;"><strong>${nome} - ${descricao}</strong></div>' +
                    '    <div class="dv-cell-head" style="width:15%;border: 0 none"></div>' +
                    '    <div class="dv-cell-head" style="width:15%;border: 0 none"></div>' +
                    '    <div class="dv-cell-head" style="width:15%;border: 0 none; text-align: right;color:#fff;"><strong>Lucro (R$):</strong></div>' +
                    '    <div class="dv-cell-head" style="width:15%;border: 0 none; text-align: right;color:#fff;"><strong>${window.NumberFormat(parseFloat(lucro), 2, ",", ".")}</strong></div>' +
                    '</div>' +
                    '<div style="display: table-row">' +
                    '    <div class="dv-cell" style="font-weight: bold">Descrição</div>' +
                    '    <div class="dv-cell" style="text-align: right;border-left: 0 none;font-weight: bold">Base Cálculo</div>' +
                    '    <div class="dv-cell" style="text-align: right;border-left: 0 none;font-weight: bold">Referência</div>' +
                    '    <div class="dv-cell" style="text-align: right;border-left: 0 none;font-weight: bold;">Despesa</div>' +
                    '    <div class="dv-cell" style="text-align: right;border-left: 0 none;font-weight: bold">Receita</div>' +
                    '</div>' +
                    '<div style="display: table-row">' +
                    '    <div class="dv-cell" >Custos gasto com material</div>' +
                    '    <div class="dv-cell" style="text-align: right;border-left: 0 none"></div>' +
                    '    <div class="dv-cell" style="text-align: right;border-left: 0 none"></div>' +
                    '    <div class="dv-cell" style="text-align: right;border-left: 0 none; color:red"><strong>${window.NumberFormat(parseFloat(custoMaterial), 2, ",", ".")}</strong></div>' +
                    '    <div class="dv-cell" style="text-align: right;border-left: 0 none"></div>' +
                    '</div>' +
                    '<div style="display: table-row">' +
                    '    <div class="dv-cell" >Comissão projetista ${projetista}</div>' +
                    '    <div class="dv-cell" style="text-align: right;border-left: 0 none"><input type="text" style="text-align:right" data-input="calcular" name="Produtos[0].porcentagemProjetista"  filter="floatnumber" value="${porcentagemProjetista}"class="input-small"></div>' +
                    '    <div class="dv-cell" style="text-align: right;border-left: 0 none"><strong>${window.NumberFormat(parseFloat(totalProduto), 2, ",", ".")}</strong></div>' +
                    '    <div class="dv-cell" style="text-align: right;border-left: 0 none; color:red"><strong>${window.NumberFormat(parseFloat(comissaoProjetista), 2, ",", ".")}</strong></div>' +
                    '    <div class="dv-cell" style="text-align: right;border-left: 0 none"></div>' +
                    '</div>' +
                    '<div style="display: table-row">' +
                    '    <div class="dv-cell" >Comissão marceneiro ${marceneiro}</div>' +
                    '    <div class="dv-cell" style="text-align: right;border-left: 0 none"><input type="text" style="text-align:right" data-input="calcular" name="Produtos[0].porcentagemMarceneiro"  filter="floatnumber" value="${porcentagemMarceneiro}"class="input-small"></div>' +
                    '    <div class="dv-cell" style="text-align: right;border-left: 0 none"><strong>${window.NumberFormat(parseFloat(totalProduto), 2, ",", ".")}</strong></div>' +
                    '    <div class="dv-cell" style="text-align: right;border-left: 0 none; color:red"><strong>${window.NumberFormat(parseFloat(comissaoMarceneiro), 2, ",", ".")}</strong></div>' +
                    '    <div class="dv-cell" style="text-align: right;border-left: 0 none"></div>' +
                    '</div>' +
                    '<div style="display: table-row">' +
                    '    <div class="dv-cell" >Lucro calculado com base na margem de ganho no material</div>' +
                    '    <div class="dv-cell" style="text-align: right;border-left: 0 none"><strong>${window.NumberFormat(parseFloat(totalMaterial), 2, ",", ".")}</strong></div>' +
                    '    <div class="dv-cell" style="text-align: right;border-left: 0 none"><strong>${window.NumberFormat(parseFloat(custoMaterial), 2, ",", ".")}</strong></div>' +
                    '    <div class="dv-cell" style="text-align: right;border-left: 0 none"></div>' +
                    '    <div class="dv-cell" style="text-align: right;border-left: 0 none; color:green"><strong>${window.NumberFormat(parseFloat(liquidoMaterial), 2, ",", ".")}</strong></div>' +
                    '</div>' +
                    '<div style="display: table-row">' +
                    '    <div class="dv-cell" >Valor a ser cobrado pelo produto com base no material</div>' +
                    '    <div class="dv-cell" style="text-align: right;border-left: 0 none"><input type="text" style="text-align:right" data-input="calcular" name="Produtos[0].margemGanho"  filter="floatnumber" value="${margemGanho}"class="input-small"></div>' +
                    '    <div class="dv-cell" style="text-align: right;border-left: 0 none"><strong>${window.NumberFormat(parseFloat(totalMaterial), 2, ",", ".")}</strong></div>' +
                    '    <div class="dv-cell" style="text-align: right;border-left: 0 none;"></div>' +
                    '    <div class="dv-cell" style="text-align: right;border-left: 0 none; color:green"><strong>${window.NumberFormat(parseFloat(liquidoProduto), 2, ",", ".")}</strong></div>' +
                    '</div>' +
                    '<div style="display: table-row">' +
                    '    <div class="dv-cell" style="text-align:right"><strong>Totais (R$)</strong></div>' +
                    '    <div class="dv-cell" style="border-left: 0 none;"></div>' +
                    '    <div class="dv-cell" style="text-align: right;border-left: 0 none; color:grey"><strong>${window.NumberFormat(parseFloat(totalProduto), 2, ",", ".")}</strong></div>' +
                    '    <div class="dv-cell" style="text-align: right;border-left: 0 none;width:13%; color:red"><strong>${window.NumberFormat(parseFloat(custoProduto), 2, ",", ".")}</strong></div>' +
                    '    <div class="dv-cell" style="text-align: right;border-left: 0 none; color:green"><strong>${window.NumberFormat(parseFloat(totalLiquido), 2, ",", ".")}</strong></div>' +
                    '</div>' +
                '</div>' +
            '</td >' +
         '</tr>'
};
