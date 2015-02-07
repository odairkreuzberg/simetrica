$(function () {

    $('form').submit(function (e) {
            Parcela.inicializar();
            return $('#dlg-pagamento').modal(options);
    });

    $('#dlg-pagamento').dialog({
        width: 960
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

    $('#tblParcela').on({
        change: function () {
            var tbl = $('#tblParcela').tGrid();
            Parcela.addValorTotal(tbl);
            return false;
        }
    }, 'input[data-input=calcularParcela]');

    $('#dlg-item').dialog({
        width: 960
    });

    $('#add-item').click(function () {
        Item.adicionar();
        return false;
    });

    $('#tblItem').on({
        click: function ()
        { Item.remover(this); return false; }
    }, 'a[data-event=remove]');


    $('#tblCompraProjeto').tGrid({
        addMode: "append",
        pager: {
            enabled: true,
            size: 10
        },
        onCreate: function (grid) {
            var data = window.Functions.getJsonData('#tblCopraProjeto_data');
            if (data.length) {
                grid.progressbar();
                var work = new window.ThreadLoop(data);
                work.action = function (item, index, total) {
                    grid.addRow(CopraProjeto.estrutura, item);

                    Functions.progressbar.update(grid.progressbar(), index, total);
                };
                work.finish = function (thread) {
                    CopraProjeto.addValorTotal(grid);
                    grid.updateIndex();
                    grid.paginate();
                    grid.progressbar().dispose();
                };
                work.start();
            }

        },
        onUpdate: function (grid) {
            $('#tblCompraProjeto_count').val(grid.count());
        }
    });

    $('#tblItem').tGrid({
        searchable: true,
        addMode: "append",
        headerButtons: ['#open-item'],
        pager: {
            enabled: true,
            size: 10
        },
        onCreate: function (grid) {
            var data = window.Functions.getJsonData('#tblItem_data');
            if (data.length) {
                grid.progressbar();
                var work = new window.ThreadLoop(data);

                work.action = function (item, index, total) {
                    grid.addRow(Item.estrutura, Item.formpatData(item));

                    Functions.progressbar.update(grid.progressbar(), index, total);
                };
                work.finish = function (thread) {
                    grid.updateIndex();
                    grid.paginate();
                    grid.progressbar().dispose();
                    Item.addValorTotal(grid);
                };
                work.start();
            }

        },
        onUpdate: function (grid) {
            $('#tblItem_count').val(grid.count());
        }
    });

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

    $('#tblItem').on({
        change: function () {
            var tbl = $('#tblItem').tGrid();
            Item.addValorTotal(tbl);
            return false;
        }
    }, 'input[data-input=calcular]');
})
.on('AfterLoad_Projeto', function (e, data) {
    if (data) {
        Item.showCompras(data.idProjeto);
    }
    else {
        Item.hideCompras();
    }
});

var Item = {

    showCompras: function (idProjeto) {
        $('#compra-projeto').show();
    },

    hideCompras: function () {
        $('#compra-projeto').hide();
    },
    adicionar: function () {
        var form = Item.getForm();

        if (Item.validarForm(form)) {

            var tbl = $('#tblItem').tGrid();

            if (!tbl.hasUniqueValue(form.inputIdMaterial.val())) {

                tbl.addRow(Item.estrutura, {
                    idMaterial: form.inputIdMaterial.val(),
                    nome: form.imputNome.val(),
                    valor: form.inputValor.val(),
                    quantidade: form.inputQuantidade.val(),
                    total: 0
                });
                ShowMessage("Item adicionado com sucesso", "sucesso");
                Item.addValorTotal(tbl);
                Item.limparCampos(form);
            } else {
                ShowMessage("Este item já foi adicionado", "erro");
            }
        }
    },

    getForm: function () {
        return form = {
            imputNome: $('#Material_nome'),
            inputIdMaterial: $('#Material_idMaterial'),
            inputQuantidade: $('#quantidade'),
            inputValor: $('#valor'),
            spanValor: $('#valor-valid'),
            spanQuantidade: $('#quantidade-valid'),
            spanMaterial: $('#Material')
        };
    },
    limparCampos: function (form) {
        form.imputNome.val('');
        form.inputIdMaterial.val('');
        form.inputQuantidade.val('');
        form.inputValor.val('');
    },

    validarForm: function (form) {
        if (form.inputValor.val() === '0,00') {
            form.inputValor.val('');
        }
        validarCampo(form.spanMaterial, form.imputNome, "Informe o material");
        validarCampo(form.spanValor, form.inputValor, "Informe o valor");
        validarCampo(form.spanQuantidade, form.inputQuantidade, "Informe a quantidade");

        return (!window.IsNullOrEmpty(form.imputNome.val()) && !window.IsNullOrEmpty(form.inputValor.val()) && !window.IsNullOrEmpty(form.inputQuantidade.val()));
    },

    addValorTotal: function (tbl) {
        var list = tbl.getRows();
        var vlTotalGeral = 0;

        for (var i = 0; i < list.length; i++) {
            var data = tbl.getRowData($(list[i]));
            var valor = data.valor.replaceAll(".", "").replace(",", ".");
            var quantidade = data.quantidade;

            var vlTotal = parseFloat(valor) * parseFloat(quantidade);
            vlTotalGeral += parseFloat(vlTotal);
            data.total = window.NumberFormat(vlTotal, 2, ',', '.');
            data.valor = data.valor.replaceAll(".", "").replace(",", ".");
            data.valor = window.NumberFormat(data.valor, 2, ',', '.');

            tbl.updateRow($(list[i]), Item.estrutura, data);
        }

        $('#vl-total-geral').text(window.NumberFormat(vlTotalGeral, 2, ',', '.'));

        window.Functions.handlers.filter();
        tbl.updateIndex();
        tbl.paginate();
    },

    remover: function (row) {
        var tbl = $('#tblItem').tGrid();
        tbl.removeRow($(row));
        tbl.updateIndex();
        tbl.paginate();
        Item.addValorTotal(tbl);
    },
    formpatData: function (item) {
        var data = {
            valor: window.NumberFormat(parseFloat(item.valor), 2, ',', '.'),
            total: window.NumberFormat(parseFloat(item.total), 2, ',', '.'),
            quantidade: item.quantidade,
            idMaterial: item.idMaterial,
            nome: item.nome,
        }
        return data;
    },

    estrutura:
        '<tr class="data">' +
            '<td >' +
                '<a href="#" class="btn btn-danger" title="Excluir" data-event="remove"><i class="icon-trash icon-white"></i></a>' +
                '<input type="hidden" name="Itens[0].nome" value="${nome}"/>' +
                '<input type="hidden" name="Itens[0].idMaterial"  unique="true"  value="${idMaterial}"/>' +
                '<input type="hidden" name="Itens[0].total" value="${total}"/>' +
            '</td>' +
            '<td>${nome}</td>' +
            '<td style="text-align: right">' +
                '<input type="text" style="text-align: right; margin: 0" name="Itens[0].quantidade" data-input="calcular" value="${quantidade}" filter="numeric" class="input-small" >' +
            '</td>' +
            '<td style="text-align: right">' +
                '<input type="text" style="text-align: right; margin: 0" name="Itens[0].valor" data-input="calcular" value="${valor}" filter="floatnumber" class="input-small" >' +
            '</td>' +
            '<td style="text-align: right">${total}</td>' +
         '</tr>'
};

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
        if (parseFloat(somaParcelas) != parseFloat(Projeto_vlVenda)) {
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
        $('#total').val($('#vl-total-geral').text());
        $('#dlg-pagamento').modal('show');
    },
    valid: function () {
        var total = $('#total').val().replaceAll(".", "").replace(",", ".");
        if (parseFloat(total)== 0) {
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
            window.Functions.handlers.mask();
            window.Functions.handlers.filter();
            window.Functions.handlers.datepicker();
            Parcela.addValorTotal(tbl);
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

var CopraProjeto = {

    addValorTotal: function (tbl) {
        var list = tbl.getRows();
        var vlTotalGeral = 0;

        for (var i = 0; i < list.length; i++) {
            var data = tbl.getRowData($(list[i]));
            var valor = data.valor.replaceAll(".", "").replace(",", ".");
            var quantidade = data.quantidade;

            var vlTotal = parseFloat(valor) * parseFloat(quantidade);
            vlTotalGeral += parseFloat(vlTotal);
            data.total = window.NumberFormat(vlTotal, 2, ',', '.');
            data.valor = data.valor.replaceAll(".", "").replace(",", ".");
            data.valor = window.NumberFormat(data.valor, 2, ',', '.');

            tbl.updateRow($(list[i]), CopraProjeto.estrutura, data);
        }

        $('#vl-compra-geral').text(window.NumberFormat(vlTotalGeral, 2, ',', '.'));

        tbl.updateIndex();
        tbl.paginate();
    },

    estrutura:
        '<tr class="data">' +
            '<td >' +
                '${fornecedor}' +
                '<input type="hidden" name="Materiais[0].idMaterial" value="${idMaterial}"/>' +
                '<input type="hidden" name="Materiais[0].nome" value="${nome}"/>' +
                '<input type="hidden" name="Materiais[0].fornecedor" value="${fornecedor}"/>' +
                '<input type="hidden" name="Materiais[0].valor" value="${valor}"/>' +
                '<input type="hidden" name="Materiais[0].total" value="${total}"/>' +
                '<input type="hidden" name="Materiais[0].quantidade" value="${quantidade}"/>' +
            '</td>' +
            '<td>${nome}</td>' +
            '<td style="text-align: right">${quantidade}</td>' +
            '<td style="text-align: right">${total}</td>' +
            '<td style="text-align: right">${total}</td>' +
         '</tr>'
};
