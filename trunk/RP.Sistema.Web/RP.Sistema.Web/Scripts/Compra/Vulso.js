$(function () {

    $('form').submit(function (e) {
        return Parcela.validarPagamento()
    });

    $('#gerar-parcela').on({
        click: function () {
            Parcela.gerarParcelas(this);
            return false;
        }
    });
    $('#total').on({
        change: function () {
            Parcela.zerarParcelas(this);
            return false;
        }
    });

    $('#tblParcela').on({
        change: function () {
            var tbl = $('#tblParcela').tGrid();
            Parcela.addValorTotal(tbl);
            return false;
        }
    }, 'input[data-input=calcularParcela]');

    $('#tblParcela').tGrid({
        addMode: "append",
        pager: {
            enabled: true,
            size: 10
        },
        onCreate: function (grid) {
            var data = window.Functions.getJsonData('#tblParcela_data');
            if (data.length) {
                grid.progressbar();
                var work = new window.ThreadLoop(data);
                work.action = function (item, index, total) {
                    grid.addRow(Parcela.estrutura, {
                        vlParcela: window.NumberFormat(item.vlParcela, 2, ',', '.'),
                        flFormaPagamento: item.flFormaPagamento,
                        nrParcela: item.nrParcela,
                        dsObservacao: item.dsObservacao,
                        dtVencimento: item.dtVencimento
                    });

                    Functions.progressbar.update(grid.progressbar(), index, total);
                };
                work.finish = function (thread) {
                    Parcela.addValorTotal(grid);
                    grid.updateIndex();
                    grid.paginate();
                    grid.progressbar().dispose();
                };
                work.start();
            }

        },
        onUpdate: function (grid) {
            $('#tblParcela_count').val(grid.count());
        }
    });
});

var Parcela = {
    validarPagamento: function () {
        var tbl = $('#tblParcela').tGrid();
        var list = tbl.getRows();
        if (list.length > 0) {
            var somaParcelas = $('#somaParcelas').val().replaceAll(".", "").replace(",", ".");
            var total = $('#total').val().replaceAll(".", "").replace(",", ".");
            if (parseFloat(somaParcelas) != parseFloat(total)) {
                ShowMessage("A soma das parcelas deve ser igual ao valor total", "erro");
                return false;
            }

            for (var i = 0; i < list.length; i++) {
                var data = tbl.getRowData($(list[i]));
                if (data.dtVencimento == "") {
                    ShowMessage("Todas as datas de vencimento das parcelas devem ser preenchidas", "erro");
                    return false;
                }
                if (data.vlParcela == "" || data.vlParcela <= 0) {
                    ShowMessage("Todos os valores das parcelas devem ser preenchidas", "erro");
                    return false;
                }
            }
        }
        return true;
    },
    valid: function () {
        var total = $('#total').val().replaceAll(".", "").replace(",", ".");
        if (parseFloat(total) == 0) {
            ShowMessage("O valor total da compra não pode ser [0]", "erro");
            return false;
        }

        if ($('#Compra_qtdParcela').val() == "" || $('#Compra_qtdParcela').val() == "0") {
            ShowMessage("Informe a quantidade de parcelas.", "erro");
            return false;
        }
        var Compra_dtVencimento = $('#Compra_dtVencimento').val();
        if ($('#Compra_dtVencimento').val() == "") {
            ShowMessage("Informe o vencimento da 1º parcela", "erro");
            return false;
        }
        return true;
    },

    zerarParcelas: function () {
        $('#Compra_qtdParcela').val();
        $('#Compra_dtVencimento').val();
        $('#somaParcelas').val('0,00');
        var tbl = $('#tblParcela').tGrid();
        tbl.removeAllRows();
    },
    gerarParcelas: function () {
        if (Parcela.valid()) {
            var data = $('#Compra_dtVencimento').val().split("/");
            var d = new Date(data[2] + '/' + data[1] + '/' + data[0]);
            var parcelas = $('#Compra_qtdParcela').val();
            var vlTotal = $('#total').val().replaceAll(".", "").replace(",", ".");
            var vlParcela = parseFloat(vlTotal) / parseFloat(parcelas);
            var tbl = $('#tblParcela').tGrid();
            tbl.removeAllRows();
            for (var i = 0; i < parcelas; i++) {
                var dat = d.getDate();
                var mon = d.getMonth();
                var year = d.getFullYear();
                var vencimento = $.datepicker.formatDate("dd/mm/yy", d);
                console.log(vencimento)
                tbl.addRow(Parcela.estrutura, { nrParcela: (i + 1), vlParcela: window.NumberFormat(parseFloat(vlParcela), 2, ",", "."), dtVencimento: vencimento });
                d.setMonth(d.getMonth() + 1);
            }
            Parcela.addValorTotal(tbl);
            window.Functions.handlers.mask();
            window.Functions.handlers.filter();
            window.Functions.handlers.datepicker();
            tbl.updateIndex();
            tbl.paginate();
        }
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