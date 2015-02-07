$(function () {
    var tbAcoes = $('#tblAcoes').tGrid({
        searchable: false,
        pager: {
            enabled: false,
            size: 10
        },
        onCreate: function (grid) {
            var data = Functions.getJsonData('#tblAcoes_data');
            if (data.length){
                grid.progressbar();

                var work = new ThreadLoop(data);

                work.action = function (item, index, total)
                {
                    grid.addRow(Acoes.estrutura, {
                        nmMenu: item.nmMenu,
                        dsAcao: item.dsAcao,
                        idAcao: item.idAcao,
                        nmAcao: item.nmAcao
                    });

                    Functions.progressbar.update(grid.progressbar(), index, total);
                }

                work.finish = function (thread)
                {
                    grid.updateIndex();
                    grid.paginate();
                    grid.progressbar().dispose();
                }

                work.start();
            }
        },
        onUpdate: function (grid) {
            $('#tblAcoes_count').val(grid.count());
        }
    });

    var tbAcoesPerfil = $('#tblAcoesPerfil').tGrid({
        searchable: true,
        pager: {
            enabled: true,
            size: 10
        },
        onCreate: function (grid) {
            var data = Functions.getJsonData('#tblAcoesPerfil_data');
            if (data.length) {
                grid.progressbar();

                var work = new ThreadLoop(data);

                work.action = function (item, index, total) {
                    grid.addRow(AcoesPerfil.estrutura, {
                        nmMenu: item.nmMenu,
                        dsAcao: item.dsAcao,
                        idAcao: item.idAcao,
                        nmAcao: item.nmAcao,
                        nmControle: item.nmControle
                    });

                    Functions.progressbar.update(grid.progressbar(), index, total);
                }

                work.finish = function (thread) {
                    grid.updateIndex()
                        .paginate()
                        .progressbar().dispose();
                }

                work.start();
            }
        },
        onUpdate: function (grid) {
            $('#tblAcoesPerfil_count').val(grid.count());
        }
    });
})
.on('AfterLoadControle', function (e, data) {
    if (data && data.idControle > 0) {
        var tblAcoes = $('#tblAcoes').tGrid();

        $.ajax({
            dataType: 'json',
            type: 'GET',
            url: $('#url_listar_acoes').val(),
            data: { idControle: data.idControle },

            success: function (data) {
                tblAcoes.removeAllRows();
                if (data && data.result) {
                    for (var i = 0; i < data.result.length; i++)
                    {
                        if (!AcoesPerfil.exists(data.result[i].idAcao)) {
                            tblAcoes.addRow(Acoes.estrutura, {
                                idAcao: data.result[i].idAcao,
                                dsAcao: data.result[i].dsAcao,
                                nmAcao: data.result[i].nmAcao,
                                nmMenu: data.result[i].nmMenu,
                                nmControle: data.result[i].nmControle
                            });
                        }
                    }
                    tblAcoes.updateIndex().paginate();
                }
            },

            error: function (request) {
                Functions.checkRequest(request);
            }
        });
    }
});

var Acoes = {
    estrutura:
        '<tr class="data">' +
            '<td class="actinons b1">' +
                '<input type="hidden" name="Acoes[0].idAcao" unique="true" value="${idAcao}"/>' +
                '<input type="hidden" name="Acoes[0].dsAcao" value="${dsAcao}"/>' +
                '<input type="hidden" name="Acoes[0].nmMenu" value="${nmMenu}"/>' +
                '<input type="hidden" name="Acoes[0].nmControle" value="${nmControle}"/>' +
                '<input type="hidden" name="Acoes[0].nmAcao" value="${nmAcao}"/>' +
                '<a href="#" class="btn btn-success" title="Adicionar" onclick="AcoesPerfil.adicionarNaGrid(this); return false"><i class="icon-plus-sign icon-white"></i></a>' +
            '</td>' +
            '<td>${nmAcao}</td>' +
            '<td>${dsAcao}</td>' +
        '</tr>'
}

var AcoesPerfil = {
    remover: function (row) {
        var tbAcoesPerfil = $('#tblAcoesPerfil').tGrid();
        var data = tbAcoesPerfil.getRowData($(row).parents('tr'));
        tbAcoesPerfil.removeRow($(row));
        tbAcoesPerfil.updateIndex();
        tbAcoesPerfil.paginate();
    },

    estrutura:
        '<tr class="data">' +
            '<td class="actions b1">' +
                '<input type="hidden" name="AcoesPerfil[0].idAcao" unique="true" value="${idAcao}"/>' +
                '<input type="hidden" name="AcoesPerfil[0].dsAcao" value="${dsAcao}"/>' +
                '<input type="hidden" name="AcoesPerfil[0].nmMenu" value="${nmMenu}"/>' +
                '<input type="hidden" name="AcoesPerfil[0].nmControle" value="${nmControle}"/>' +
                '<input type="hidden" name="AcoesPerfil[0].nmAcao" value="${nmAcao}"/>' +
                '<a href="#" class="btn btn-danger" title="Excluir" onclick="AcoesPerfil.remover(this); return false"><i class="icon-trash icon-white"></i></a>' +
            '</td>' +
            '<td>${nmAcao}</td>' +
            '<td>${dsAcao}</td>' +
            '<td>${nmControle}</td>' +
        '</tr>',

    adicionarNaGrid: function (row) {
        var tblAcoes = $('#tblAcoes').tGrid();
        var tblAcoesPerfil = $('#tblAcoesPerfil').tGrid();
        var data = tblAcoes.getRowData($(row).parents('tr'));

        if (!tblAcoesPerfil.hasUniqueValue(data[0]))
        {
            tblAcoesPerfil.addRow(AcoesPerfil.estrutura, { idAcao: data[0], dsAcao: data[1], nmMenu: data[2], nmControle: data[3], nmAcao: data[4] });
            tblAcoesPerfil.updateIndex();
            tblAcoesPerfil.paginate();
            tblAcoes.removeRow($(row));
            tblAcoes.updateIndex();
        }
        else {
            alert('Esta ação ja foi adicionada ao perfil');
        }
    },

    exists: function (value)
    {
        var grid = $('#tblAcoesPerfil').tGrid();
        return grid.hasUniqueValue(value);
    }
}