$(function () {
    var tbTelefone = $('#tblUsuario').tGrid({
        searchable: true,
        pager: {
            enabled: true,
            size: 10
        },
        onCreate: function (grid) {
            var data = window.Functions.getJsonData('#tblUsuario_data');
            if (data.length) {
                grid.progressbar();
                var work = new window.ThreadLoop(data);

                work.action = function (item, index, total) {
                    grid.addRow(Usuario.estrutura, {
                        login: item.login,
                        nome: item.nome,
                        ativo: item.ativo
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
            $('#tblUsuario_count').val(grid.count());
        }
    });
});

var Usuario = {

    estrutura:
        '<tr class="data">' +
            '<td>${login}</td>' +
            '<td>${nome}</td>' +
            '<td>${ativo}</td>' +
        '</tr>'
}

