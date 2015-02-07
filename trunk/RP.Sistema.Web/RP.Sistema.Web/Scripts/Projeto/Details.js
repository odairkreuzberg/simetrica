$(function () {



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
                    grid.addRow(Produto.estrutura, item);

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
});

var Produto = {

    addValorTotal: function (tbl) {
        var list = tbl.getRows();
        var totalCusto = 0;
        var totalGanho = 0;
        var totalLiquido = 0;
        for (var i = 0; i < list.length; i++) {
            var data = tbl.getRowData($(list[i]));
            var custo = data.totalCusto.replace(",", ".");
            var ganho = data.totalGanho.replace(",", ".");
            var liquido = data.totalLiquido.replace(",", ".");

            totalCusto = parseFloat(totalCusto) + parseFloat(custo);
            totalGanho = parseFloat(totalGanho) + parseFloat(ganho);
            totalLiquido = parseFloat(totalLiquido) + parseFloat(liquido);
        }

        $('#vl-total-custo').text(window.NumberFormat(totalCusto, 2, ',', '.'));
        $('#vl-total-ganho').text(window.NumberFormat(totalGanho, 2, ',', '.'));
        $('#vl-total-liquido').text(window.NumberFormat(totalLiquido, 2, ',', '.'));
    },

    estrutura:
        '<tr class="data">' +
            '<td>${nome}</td>' +
            '<td>${projetista}</td>' +
            '<td>${marceneiro}</td>' +
            '<td style="text-align: right">${window.NumberFormat(totalCusto, 2, ",", ".")}</td>' +
            '<td style="text-align: right">${window.NumberFormat(totalGanho, 2, ",", ".")}</td>' +
            '<td style="text-align: right">${window.NumberFormat(totalLiquido, 2, ",", ".")}' +
                '<input type="hidden" name="Produtos[0].totalCusto" value="${totalCusto}"/>' +
                '<input type="hidden" name="Produtos[0].totalGanho" value="${totalGanho}"/>' +
                '<input type="hidden" name="Produtos[0].totalLiquido" value="${totalLiquido}"/>' +
            '</td>' +
         '</tr>'
};
