$(function () {

    $('#tblPonto').tGrid({
        addMode: "append",
        onCreate: function (grid) {
            var data = window.Functions.getJsonData('#tblPonto_data');
            if (data.length) {
                grid.progressbar();
                var work = new window.ThreadLoop(data);
                var unique;
                work.action = function (item, index, total) {
                    grid.addRow(Ponto.estrutura, item);
                    Functions.progressbar.update(grid.progressbar(), index, total);
                };
                work.finish = function (thread) {
                    grid.updateIndex();
                    grid.paginate();
                    grid.progressbar().dispose();
                    window.Functions.handlers.mask();
                };
                work.start();
            }

        },
        onUpdate: function (grid) {
            $('#tblPonto_count').val(grid.count());
        }
    });

    $('#tblComissao').tGrid({
        searchable: true,
        addMode: "append",
        pager: {
            enabled: true,
            size: 10
        },
        onCreate: function (grid) {
            var data = window.Functions.getJsonData('#tblComissao_data');
            if (data.length) {
                grid.progressbar();
                var work = new window.ThreadLoop(data);
                var unique;
                work.action = function (item, index, total) {
                    grid.addRow(Comissao.estrutura, item);
                    Functions.progressbar.update(grid.progressbar(), index, total);
                };
                work.finish = function (thread) {
                    grid.updateIndex();
                    grid.paginate();
                    grid.progressbar().dispose();
                    window.Functions.handlers.mask();
                };
                work.start();
            }

        },
        onUpdate: function (grid) {
            $('#tblComissao_count').val(grid.count());
        }
    });

    $('#tblProximo').tGrid({
        searchable: true,
        addMode: "append",
        pager: {
            enabled: true,
            size: 10
        },
        onCreate: function (grid) {
            var data = window.Functions.getJsonData('#tblProximo_data');
            if (data.length) {
                grid.progressbar();
                var work = new window.ThreadLoop(data);
                var unique;
                work.action = function (item, index, total) {
                    grid.addRow(Proximo.estrutura, item);
                    Functions.progressbar.update(grid.progressbar(), index, total);
                };
                work.finish = function (thread) {
                    grid.updateIndex();
                    grid.paginate();
                    grid.progressbar().dispose();
                    window.Functions.handlers.mask();
                };
                work.start();
            }

        },
        onUpdate: function (grid) {
            $('#tblProximo_count').val(grid.count());
        }
    });

    $('#tblProximo').on({
        click: function ()
        { Proximo.addComissao(this); return false; }
    }, 'a[data-event=add-comissao]');

    $('#tblComissao').on({
        click: function ()
        { Comissao.addProximo(this); return false; }
    }, 'a[data-event=add-proximo]');
});

var Ponto = {

    estrutura:
        '<tr class="data">' +
            '<td >' +
                '<input type="hidden" name="Pontos[0].idFuncionario" value="${idFuncionario}"/>' +
                '<input type="hidden" name="Pontos[0].nrDia" value="${nrDia}"/>' +
                '<input type="hidden" name="Pontos[0].dsDia" value="${dsDia}"/>' +
                '<input type="hidden" name="Pontos[0].nrMes" value="${nrMes}"/>' +
                '<input type="hidden" name="Pontos[0].tipo" value="${tipo}"/>' +
                '<input type="hidden" name="Pontos[0].flSituacao" value="${flSituacao}"/>' +
                '${nrDia}º - ${dsDia}</td>' +
            '<td>${flSituacao}</td>' +
            '<td>' +
                '<input type="text" name="Pontos[0].dsObservacao" value="${dsObservacao}" class="span12">' +
            '</td>' +
            '<td>' +
                '<input type="text" name="Pontos[0].entradaManha" value="${entradaManha}" class="span12" mask = "99:99" >' +
            '</td>' +
            '<td>' +
                '<input type="text" name="Pontos[0].saidaManha" value="${saidaManha}" class="span12" mask = "99:99" >' +
            '</td>' +
            '<td>' +
                '<input type="text" name="Pontos[0].entraTarde" value="${entraTarde}" class="span12" mask = "99:99" >' +
            '</td>' +
            '<td>' +
                '<input type="text" name="Pontos[0].saidaTarde" value="${saidaTarde}" class="span12" mask = "99:99" >' +
            '</td>' +
            '<td>' +
                '<input type="text" name="Pontos[0].entradaExtra" value="${entradaExtra}" class="span12" mask = "99:99" >' +
            '</td>' +
            '<td>' +
                '<input type="text" name="Pontos[0].saidaExtra" value="${saidaExtra}" class="span12" mask = "99:99" >' +
            '</td>' +
         '</tr>'
};

var Proximo = {

    addComissao: function (el) {
        var tblProximo = $('#tblProximo').tGrid();
        var tblComissao = $('#tblComissao').tGrid();
        var row = $(el).parents('tr.data');
        var data = tblProximo.getRowData(row);

        tblComissao.addRow(Comissao.estrutura, data);
        tblComissao.updateIndex();
        tblComissao.paginate();

        tblProximo.removeRow($(el));
        tblProximo.updateIndex();
        tblProximo.paginate();
        $('#tabContainer a[href="#tab-comissao"]').tab('show');
        ShowMessage("Comissão adicionada para este mês com sucesso!", "sucesso");
    },

    estrutura:
        '<tr class="data">' +
            '<td >' +
                '<a href="#" class="btn btn-success" title="Pagar este mês" data-event="add-comissao"><i class="icon-thumbs-up icon-white"></i></a>' +
                '<input type="hidden" name="Proximos[0].idMovimento" value="${idMovimento}"/>' +
                '<input type="hidden" name="Proximos[0].dtVencimento" value="${dtVencimento}"/>' +
                '<input type="hidden" name="Proximos[0].descricao" value="${descricao}"/>' +
                '<input type="hidden" name="Proximos[0].valor" value="${valor}"/>' +
            '</td>' +
            '<td>${dtVencimento}</td>' +
            '<td>${descricao}</td>' +
            '<td>${valor}</td>' +
         '</tr>'
};

var Comissao = {

    addProximo: function (el) {
        var tblComissao = $('#tblComissao').tGrid();
        var tblProximo = $('#tblProximo').tGrid();

        var row = $(el).parents('tr.data');
        var data = tblComissao.getRowData(row);

        tblProximo.addRow(Proximo.estrutura, data);
        tblProximo.updateIndex();
        tblProximo.paginate();

        tblComissao.removeRow($(el));
        tblComissao.updateIndex();
        tblComissao.paginate();
        $('#tabContainer a[href="#tab-proximo"]').tab('show');
        ShowMessage("Comissão adicionada para o próximo mês com sucesso!", "sucesso");
    },

    estrutura:
        '<tr class="data">' +
            '<td >' +
                '<a href="#" class="btn btn-warning" title="Não pagar este mês" data-event="add-proximo"><i class="icon-thumbs-down icon-white"></i></a>' +
                '<input type="hidden" name="Comissoes[0].idMovimento" value="${idMovimento}"/>' +
                '<input type="hidden" name="Comissoes[0].dtVencimento" value="${dtVencimento}"/>' +
                '<input type="hidden" name="Comissoes[0].descricao" value="${descricao}"/>' +
                '<input type="hidden" name="Comissoes[0].valor" value="${valor}"/>' +
            '</td>' +
            '<td>${dtVencimento}</td>' +
            '<td>${descricao}</td>' +
            '<td>${valor}</td>' +
         '</tr>'
};

