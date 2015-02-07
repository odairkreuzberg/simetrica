$(function () {

    $('#tblParcela').tGrid({
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
                        dtVencimento: item.dtVencimento,
                        flSituacao: item.flSituacao
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
                '<input type="hidden" name="Parcelas[0].vlParcela" value="${vlParcela}"/>' +
                '<input type="hidden" name="Parcelas[0].dsObservacao" value="${dsObservacao}"/>' +
                '<input type="hidden" name="Parcelas[0].flFormaPagamento" value="${flFormaPagamento}"/>' +
                '<input type="hidden" name="Parcelas[0].flSituacao" value="${flSituacao}"/>' +
            '</td>' +
            '<td >${flFormaPagamento}</td>' +
            '<td >${dsObservacao}</td>' +
            '<td >${dtVencimento}</td>' +
            '<td >${flSituacao}</td>' +
            '<td style="text-align:right" >${vlParcela}</td>' +
         '</tr>'
};