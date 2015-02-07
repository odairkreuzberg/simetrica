$(function () {
    $('#tabContainer a[href="#tab-projeto"]').on('shown', function (e) {
        $('#btn-produto').text('Produto');
    });

    $('#btn-produto').on({
        click: function () {
            $('#tabContainer a[href="#tab-projeto"]').tab('show');
            $('.produto').removeClass('active');
            $('.projeto').addClass('active');
            ShowMessage("Para adicionar itens ao produto selecione um na grid", "erro");
            return false;
        }
    });

    $('#tblProduto').on({
        click: function ()
        { Produto.selecionar(this); return false; }
    }, 'a[data-event=selecionar]');


    $('#tblItem').tGrid({
        searchable: true,
        addMode: "append",
        headerButtons: ['#open-item'],
        //pager: {
        //    enabled: true,
        //    size: 10
        //},
        //onCreate: function (grid) {

        //},
        //onUpdate: function (grid) {
        //}
    });


    $('#tblProduto').tGrid({
        searchable: true,
        addMode: "append",
        headerButtons: ['#open-produto'],
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
                    grid.progressbar().dispose();
                };
                work.start();
            }

        },
        onUpdate: function (grid) {
            $('#tblProduto_count').val(grid.count());
        }
    });

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

    $('#tblItem').on({
        click: function () {
            Item.habilitarEdit(this); return false;s
        }
    }, 'a[data-event=selecionar]');

    $('#tblItem').on({
        click: function () {
            Item.update(this); return false;
        }
    }, 'a[data-event=salvar]');
})
.on('AfterLoad_Material', function (e, data) {
    if (data) {
        $('#margemGanho').val(data.margemGanho);
        $('#valor').val(window.NumberFormat(data.preco, 2, ",", "."));
    }
});

var Produto = {

    selecionar: function (row) {
        var tbl = $('#tblProduto').tGrid();
        var data = tbl.getRowData($(row).parents('tr'));
        $('#tabContainer a[href="#tab-produto"]').tab('show');
        $('.produto').addClass('active');
        $('.projeto').removeClass('active');
        $('#btn-produto').text(data.nome);
        $('#Produto_descricao').val(data.descricao);
        $('#Produto_marceneiro').val(data.marceneiro);
        $('#Produto_projetista').val(data.projetista);
        $('#Produto_idProduto').val(data.idProduto);
        var tblItem = $('#tblItem').tGrid();
        tblItem.removeAllRows();

        if (data.idProduto) {
            $.ajax({
                dataType: 'json',
                type: 'GET',
                url: $('#url_get_itens').val(),
                data: { idProduto: data.idProduto },
                success: function (data) {
                    if (data && data.result) {
                        for (var i = 0; i < data.result.length; i++) {
                            tblItem.addRow(Item.estrutura, data.result[i]);
                        }
                        tblItem.updateIndex();
                        tblItem.paginate();
                        Item.addValorTotal(tblItem);
                    }
                },
                error: function (request) {
                    Functions.checkRequest(request);
                }
            });
        }
    },

    estrutura:
        '<tr class="data">' +
            '<td >' +
                '<a href="#" class="btn" title="Selecionar" data-event="selecionar"><i class="icon-check"></i></a>' +
                '<input type="hidden" name="Produtos[0].nome" unique="true" value="${nome}"/>' +
                '<input type="hidden" name="Produtos[0].descricao" value="${descricao}"/>' +
                '<input type="hidden" name="Produtos[0].projetista" value="${projetista}"/>' +
                '<input type="hidden" name="Produtos[0].marceneiro" value="${marceneiro}"/>' +
                '<input type="hidden" name="Produtos[0].idMarceneiro" value="${idMarceneiro}"/>' +
                '<input type="hidden" name="Produtos[0].idProjetista" value="${idProjetista}"/>' +
                '<input type="hidden" name="Produtos[0].idProduto" value="${idProduto}"/>' +
            '</td>' +
            '<td>${nome}</td>' +
            '<td>${projetista}</td>' +
            '<td>${marceneiro}</td>' +
         '</tr>'
};

var Item = {

    habilitarEdit: function (el) {

        var tbl = $('#tblItem').tGrid();
        var row = $(el).parents('tr.data');
        var edit = tbl.getRowData(row);
        tbl.updateRow(row, Item.edit, edit);
        tbl.updateIndex();
        tbl.paginate();
            window.Functions.handlers.mask();
    },

    remover: function (row) {
        var tbl = $('#tblItem').tGrid();
        var data = tbl.getRowData($(row).parents('tr'));

        $.ajax({
            dataType: 'json',
            type: 'POST',
            url: $('#url_delete').val(),
            data: { idProdutoMaterial: data.idProdutoMaterial},
            beforeSend: function () {
                $.loadingBox('show');
            },
            success: function (data) {
                tbl.removeRow($(row));
                tbl.updateIndex();
                tbl.paginate();

                ShowMessage(data.msg, "sucesso");
                $.loadingBox('hide');
                Item.addValorTotal(tbl);
            },
            error: function (request) {
                ShowMessage(request.responseText, "erro");
                $.loadingBox('hide');
            }
        });
    },

    update: function (el) {
        if (Item.validarEdit(el)) {

            var tbl = $('#tblItem').tGrid();
            var row = $(el).parents('tr.data');
            var model = tbl.getRowData(row);

            $.ajax({
                dataType: 'json',
                type: 'POST',
                url: $('#url_update').val(),
                data: model,
                beforeSend: function () {
                    $.loadingBox('show');
                },
                success: function (data) {
                    tbl.updateRow(row, Item.estrutura, data.model);
                    ShowMessage("Item alterado com sucesso!", "sucesso");
                    $.loadingBox('hide');
                    Item.addValorTotal(tbl);
                },
                error: function (request) {
                    $.loadingBox('hide');
                    ShowMessage(request.responseText, "erro");
                }
            });
        }
    },

    validarEdit: function (el) {
        var hora = $(el).parents('.data').find('[data-input=valor]');
        var tipo = $(el).parents('.data').find('[data-input=quantidade]');
        var situacao = $(el).parents('.data').find('[data-input=situacao]');
        if (!$(hora).val() && $(tipo).val() != "Encaixe" && $(situacao).val() != "Cancelado") {
            validarCampo($(hora));
            ShowMessage("Por favor, informe o horario", "erro");
            return false;
        }
        return true;
    },

    getForm: function () {
        return form = {
            inputIdMaterial: $('#Material_idMaterial'),
            inputNomeMaterial: $('#Material_nome'),
            inputQuantidade: $('#quantidade'),
            inputValor: $('#valor'),
            inputMargemGanho: $('#margemGanho'),
            spanMargemGanho: $('#margemganho-valid'),
            spanQuantidade: $('#quantidade-valid'),
            spanValor: $('#valor-valid'),
            spanMaterial: $('#Material'),
            btnAdd: $('#add-produto'),
            dialog: $("#dlg-item")
        };
    },

    validarForm: function (form) {
        validarCampo(form.spanQuantidade, form.inputQuantidade, "Informe a quantidade");
        validarCampo(form.spanValor, form.inputValor, "Informe o valor");
        validarCampo(form.spanMaterial, form.inputNomeMaterial, "Selecione um item");
        validarCampo(form.spanMargemGanho, form.inputMargemGanho, "Informe a margem de ganho");

        return (!window.IsNullOrEmpty(form.inputMargemGanho.val()) &&
            !window.IsNullOrEmpty(form.inputQuantidade.val()) &&
            !window.IsNullOrEmpty(form.inputValor.val()) &&
            !window.IsNullOrEmpty(form.inputIdMaterial.val()));
    },

    getCampos: function (form) {
        return model = {
            idMaterial: form.inputIdMaterial.val(),
            quantidade: form.inputQuantidade.val(),
            margemGanho: form.inputMargemGanho.val(),
            valor: form.inputValor.val(),
            idProduto: $('#Produto_idProduto').val(),
        };
    },

    limparForm: function (form) {
        form.inputIdMaterial.val('');
        form.inputNomeMaterial.val('');
        form.inputQuantidade.val('');
        form.inputMargemGanho.val('');
        form.inputValor.val('');
        form.btnAdd.button('reset');
        $.loadingBox('hide');
    },

    adicionar: function () {
        var form = Item.getForm();

        if (Item.validarForm(form)) {

            var tbl = $('#tblItem').tGrid();

            if (!tbl.hasUniqueValue(form.inputIdMaterial.val())) {

                form.btnAdd.button('loading');
                var model = Item.getCampos(form);
                $.ajax({
                    dataType: 'json',
                    type: 'POST',
                    url: $('#url_create').val(),
                    data: model,
                    beforeSend: function () {
                        $.loadingBox('show');
                    },
                    success: function (data) {

                        var tbl = $('#tblItem').tGrid();
                        tbl.addRow(Item.estrutura, data.model);
                        tbl.updateIndex();
                        tbl.paginate();
                        ShowMessage("Item adicionado com sucesso", "sucesso");

                        Item.limparForm(form);
                        Item.addValorTotal(tbl);
                    },
                    error: function (request) {
                        form.btnAdd.button('reset');
                        $.loadingBox('hide');
                    }
                });
            } else {
                ShowMessage("Este produto já foi adicionado", "erro");
            }
        }
    },

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
        tbl.updateIndex();
        tbl.paginate();
    },

    estrutura:
        '<tr class="data">' +
            '<td class="actions b2">' +
                '<a href="#" class="btn btn-danger" title="Excluir" data-event="remove"><i class="icon-trash icon-white"></i></a>' +
                '<a href="#" class="btn btn-info" title="Selecionar" data-event="selecionar"><i class="icon-edit icon-white"></i></a>' +
                '<input type="hidden" name="Itens[0].idMaterial" unique="true" value="${idMaterial}"/>' +
                '<input type="hidden" name="Itens[0].idProduto" unique="true" value="${idProduto}"/>' +
                '<input type="hidden" name="Itens[0].quantidade" value="${quantidade}"/>' +
                '<input type="hidden" name="Itens[0].margemGanho" value="${margemGanho}"/>' +
                '<input type="hidden" name="Itens[0].valor" value="${valor}"/>' +
                '<input type="hidden" name="Itens[0].nome" value="${nome}"/>' +
                '<input type="hidden" name="Itens[0].idProdutoMaterial" value="${idProdutoMaterial}"/>' +
                '<input type="hidden" name="Itens[0].totalCusto" value="${totalCusto}"/>' +
                '<input type="hidden" name="Itens[0].totalGanho" value="${totalGanho}"/>' +
                '<input type="hidden" name="Itens[0].totalLiquido" value="${totalLiquido}"/>' +
            '</td>' +
            '<td>${nome}</td>' +
            '<td style="text-align: right">${margemGanho}</td>' +
            '<td style="text-align: right">${window.NumberFormat(quantidade, 2, ",", ".")}</td>' +
            '<td style="text-align: right">${window.NumberFormat(valor, 2, ",", ".")}</td>' +
            '<td style="text-align: right">${window.NumberFormat(totalCusto, 2, ",", ".")}</td>' +
            '<td style="text-align: right">${window.NumberFormat(totalGanho, 2, ",", ".")}</td>' +
            '<td style="text-align: right">${window.NumberFormat(totalLiquido, 2, ",", ".")}' +
         '</tr>',
    edit:
        '<tr class="data">' +
            '<td class="actions b2">' +
                '<a href="#" class="btn btn-danger" title="Excluir" data-event="remove"><i class="icon-trash icon-white"></i></a>' +
                '<a href="#" class="btn btn-success" title="Salvar" data-event="salvar"><i class="icon-check icon-white"></i></a>' +
                '<input type="hidden" name="Itens[0].idMaterial" unique="true" value="${idMaterial}"/>' +
                '<input type="hidden" name="Itens[0].idProduto" unique="true" value="${idProduto}"/>' +
                '<input type="hidden" name="Itens[0].nome" value="${nome}"/>' +
                '<input type="hidden" name="Itens[0].idProdutoMaterial" value="${idProdutoMaterial}"/>' +
                '<input type="hidden" name="Itens[0].totalCusto" value="${totalCusto}"/>' +
                '<input type="hidden" name="Itens[0].totalGanho" value="${totalGanho}"/>' +
            '</td>' +
            '<td>${nome}</td>' +
            '<td><input type="text" style="text-align:right" data-input="margemGanho" name="Itens[0].margemGanho" filter="numeric" value="${margemGanho}"class="span12"></td>' +
            '<td><input type="text" style="text-align:right" data-input="quantidade" name="Itens[0].quantidade" filter="floatnumber" value="${window.NumberFormat(quantidade, 2, ",", ".")}"class="span12"></td>' +
            '<td><input type="text" style="text-align:right" data-input="valor" name="Itens[0].valor" filter="floatnumber" value="${window.NumberFormat(valor, 2, ",", ".")}"class="span12"></td>' +
            '<td style="text-align: right">${window.NumberFormat(totalCusto, 2, ",", ".")}</td>' +
            '<td style="text-align: right">${window.NumberFormat(totalGanho, 2, ",", ".")}</td>' +
            '<td style="text-align: right">${window.NumberFormat(totalLiquido, 2, ",", ".")}' +
         '</tr>'
};