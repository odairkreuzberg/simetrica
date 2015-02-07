$(function () {

    $('#dlg-custo').dialog({
        width: 960
    });

    $('#add-custo').click(function () {
        Custo.adicionar();
        return false;
    });

    $('#tblCusto').on({
        click: function ()
        { Custo.remover(this); return false; }
    }, 'a[data-event=remove]');



    $('#tblCusto').tGrid({
        searchable: true,
        addMode: "append",
        headerButtons: ['#open-custo'],
        pager: {
            enabled: true,
            size: 10
        },
        onCreate: function (grid) {
            var data = window.Functions.getJsonData('#tblCusto_data');
            if (data.length) {
                grid.progressbar();
                var work = new window.ThreadLoop(data);
                var unique;
                work.action = function (item, index, total) {
                    grid.addRow(Custo.estrutura, {
                        valor: window.NumberFormat(parseFloat(item.valor), 2, ',', '.'),
                        descricao: item.descricao,
                        dtCusto: item.dtCusto,
                        gerarConta: item.gerarConta,
                        idProjetoCusto: item.idProjetoCusto
                    });

                    Functions.progressbar.update(grid.progressbar(), index, total);
                };
                work.finish = function (thread) {
                    grid.updateIndex();
                    grid.paginate();
                    grid.progressbar().dispose();
                    Custo.addValorTotal(grid);
                };
                work.start();
            }

        },
        onUpdate: function (grid) {
            $('#tblCusto_count').val(grid.count());
        }
    });
});

var Custo = {

    adicionar: function () {
        var form = Custo.getForm();

        if (Custo.validarForm(form)) {

            var tbl = $('#tblCusto').tGrid();

            tbl.addRow(Custo.estrutura, { gerarConta: form.inputGerarConta.val(), valor: form.inputValor.val(), dtCusto: form.inputDtCusto.val(), descricao: form.inputDescricao.val() });
            tbl.updateIndex();
            tbl.paginate();
            ShowMessage("Custo adicionado com sucesso", "sucesso");
            Custo.addValorTotal(tbl);
            Custo.limparCampos(form);
        }
    },

    getForm: function () {
        return form = {
            inputGerarConta: $('#Custo_geraconta'),
            inputValor: $('#Custo_valor'),
            inputDtCusto: $('#Custo_dtCusto'),
            inputDescricao: $('#Custo_descricao'),
            spanValor: $('#valor-valid'),
            spanDtCusto: $('#dtCusto-valid'),
            spanDescricao: $('#descricao-valid')
        };
    },
    limparCampos: function (form) {
        form.inputDescricao.val('');
        form.inputValor.val('');
        form.inputDtCusto.val('');
        form.inputGerarConta.val('Não');
    },

    validarForm: function (form) {
        if ($('#Custo_valor').val() === '0,00') {
            $('#Custo_valor').val('');
        }
        validarCampo(form.spanDescricao, form.inputDescricao, "Informe a descrição");
        validarCampo(form.spanValor, form.inputValor, "Informe o valor");
        validarCampo(form.spanDtCusto, form.inputDtCusto, "Informe a data de lançamento");

        return (!window.IsNullOrEmpty(form.inputDescricao.val()) && !window.IsNullOrEmpty(form.inputDtCusto.val()) && !window.IsNullOrEmpty(form.inputValor.val()));
    },

    addValorTotal: function (tbl) {
        var list = tbl.getRows();
        var vlTotal = 0;

        for (var i = 0; i < list.length; i++) {
            var data = tbl.getRowData($(list[i]));
            var totalItem = data.valor.replace(",", ".");

            vlTotal = parseFloat(vlTotal) + parseFloat(totalItem);
        }

        $('#vl-total-geral').text(window.NumberFormat(vlTotal, 2, ',', '.'));
    },

    remover: function (row) {
        var tbl = $('#tblCusto').tGrid();
        tbl.removeRow($(row));
        tbl.updateIndex();
        tbl.paginate();
        Custo.addValorTotal(tbl);
    },

    estrutura:
        '<tr class="data">' +
            '<td >' +
                '<a href="#" class="btn btn-danger" title="Excluir" data-event="remove"><i class="icon-trash icon-white"></i></a>' +
                '<input type="hidden" name="Custos[0].dtCusto" value="${dtCusto}"/>' +
                '<input type="hidden" name="Custos[0].descricao" value="${descricao}"/>' +
                '<input type="hidden" name="Custos[0].valor" value="${valor}"/>' +
                '<input type="hidden" name="Custos[0].gerarConta" value="${gerarConta}"/>' +
                '<input type="hidden" name="Custos[0].idProjetoCusto" value="${idProjetoCusto}"/>' +
            '</td>' +
            '<td>${dtCusto}</td>' +
            '<td>${descricao}</td>' +
            '<td>${gerarConta}</td>' +
            '<td style="text-align: right">${valor}</td>' +
         '</tr>'
};
