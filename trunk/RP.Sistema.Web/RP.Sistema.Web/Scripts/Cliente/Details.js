$(function () {
    $('#tblTelefone').tGrid({
        searchable: false,
        addMode: "append",
        pager: {
            enabled: true,
            size: 10
        },
        onCreate: function (grid) {
            var data = window.Functions.getJsonData('#tblTelefone_data');
            if (data.length) {
                grid.progressbar();
                var work = new window.ThreadLoop(data);
                var unique;
                work.action = function (item, index, total) {
                    grid.addRow(Telefone.estrutura, {
                        tipo: item.tipo,
                        numero: item.numero
                    });

                    Functions.progressbar.update(grid.progressbar(), index, total);
                };
                work.finish = function (thread) {
                    grid.updateIndex();
                    grid.paginate();
                    grid.progressbar().dispose();
                };
                work.start();
            }

        },
        onUpdate: function (grid) {
            $('#tblTelefone_count').val(grid.count());
        }
    });
});

var Telefone = {

    estrutura:
        '<tr class="data">' +
            '<td>${tipo}</td>' +
            '<td>${numero}</td>' +
         '</tr>'
};
