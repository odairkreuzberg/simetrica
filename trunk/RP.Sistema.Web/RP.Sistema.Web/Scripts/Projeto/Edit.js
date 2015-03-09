var rowEdit = null;
$(function () {
    $('#descricao').focus();

    $('#dlg-produto').dialog({
        width: 960
    });

    $('#add-produto').click(function () {
        Produto.adicionar();
        return false;
    });

    $('#tblProduto').on({
        click: function ()
        {
            Produto.edit(this);
            return false;

        }
    }, 'a[data-event=edit]');

    $('#tblProduto').on({
        click: function ()
        { Produto.remover(this); return false; }
    }, 'a[data-event=remove]');



    $('#tblProduto').tGrid({
        searchable: false,
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
                    grid.addRow(Produto.estrutura, Produto.formpatData(item));

                    Functions.progressbar.update(grid.progressbar(), index, total);
                };
                work.finish = function (thread) {
                    grid.updateIndex();
                    grid.paginate();
                    grid.progressbar().dispose();
                    window.Functions.handlers.filter();
                };
                work.start();
            }

        },
        onUpdate: function (grid) {
            $('#tblProduto_count').val(grid.count());
        }
    });

    window.Functions.handlers.mask();
})
.on('AfterLoad_Marceneiro', function (e, data) {
    if (data) {
        $('#Produto_porcentagemMarceneiro').val(window.NumberFormat(data.comissao, 2, ",", "."));
    }
})
.on('AfterLoad_Projetista', function (e, data) {
    if (data) {
        $('#Produto_porcentagemProjetista').val(window.NumberFormat(data.comissao, 2, ",", "."));
    }
})
.on('AfterLoad_Vendedor', function (e, data) {
    if (data) {
        $('#porcentagemVendedor').val(window.NumberFormat(data.comissao, 2, ",", "."));
    }
});




var Produto = {
    formpatData: function (item) {
        var data = {
            comissaoMarceneiro: window.NumberFormat(parseFloat(item.comissaoMarceneiro), 2, ',', '.'),
            comissaoProjetista: window.NumberFormat(parseFloat(item.comissaoProjetista), 2, ',', '.'),
            porcentagemMarceneiro: window.NumberFormat(parseFloat(item.porcentagemMarceneiro), 2, ',', '.'),
            porcentagemProjetista: window.NumberFormat(parseFloat(item.porcentagemProjetista), 2, ',', '.'),
            vlVenda: window.NumberFormat(parseFloat(item.vlVenda), 2, ',', '.'),
            vlProduto: window.NumberFormat(parseFloat(item.vlProduto), 2, ',', '.'),
            vlLucro: window.NumberFormat(parseFloat(item.vlLucro), 2, ',', '.'),
            vlLiquido: window.NumberFormat(parseFloat(item.vlLiquido), 2, ',', '.'),
            vlLiquidoDesconto: window.NumberFormat(parseFloat(item.vlLiquidoDesconto), 2, ',', '.'),
            vlDesconto: window.NumberFormat(parseFloat(item.vlDesconto), 2, ',', '.'),
            margemGanho: window.NumberFormat(parseFloat(item.margemGanho), 2, ',', '.'),
            vlLiquidoMaterial: window.NumberFormat(parseFloat(item.vlLiquidoMaterial), 2, ',', '.'),
            vlTotalMaterial: window.NumberFormat(parseFloat(item.vlTotalMaterial), 2, ',', '.'),
            vlCustoMaterial: window.NumberFormat(parseFloat(item.vlCustoMaterial), 2, ',', '.'),
            nome: item.nome,
            descricao: item.descricao,
            idProjetista: item.idProjetista,
            idMarceneiro: item.idMarceneiro,
            idProduto: item.idProduto,
            projetista: item.projetista,
            marceneiro: item.marceneiro,
            edit: "true"
        }
        return data;
    },

    adicionar: function () {
        var form = Produto.getForm();

        if (Produto.validarForm(form)) {

            var tbl = $('#tblProduto').tGrid();

            if (!tbl.hasUniqueValue(form.inputNome.val())) {
                var data = {
                    descricao: form.inputDescricao.val(),
                    nome: form.inputNome.val(),
                    idMarceneiro: form.inputIdMarceneiro.val(),
                    idProjetista: form.inputIdProtista.val(),
                    projetista: form.inputNmProjetista.val(),
                    marceneiro: form.inputNmMarceneiro.val(),
                    margemGanho: form.inputMargemGanho.val(),
                    porcentagemProjetista: form.inputPorcentagemProjetista.val(),
                    porcentagemMarceneiro: form.inputPorcentagemMarceneiro.val(),
                    idProduto: 0,
                    vlProduto: 0,
                    comissaoProjetista: 0,
                    comissaoMarceneiro: 0,
                    vlVenda: 0,
                    vlLucro: 0,
                    vlLiquido: 0,
                    vlLiquidoDesconto: 0,
                    vlDesconto: 0,
                    vlLiquidoMaterial: 0,
                    vlCustoMaterial: 0,
                    vlTotalMaterial: 0
                }
                if (rowEdit) {
                    var item = tbl.getRowData(rowEdit);
                    data.idProduto = item.idProduto;
                    tbl.updateRow(rowEdit, Produto.estrutura, Produto.formpatData(data));
                    ShowMessage("Produto alterado com sucesso", "sucesso");
                    rowEdit = null;
                    tbl.updateIndex();
                    tbl.paginate();
                } else {
                    tbl.addRow(Produto.estrutura, data);
                    tbl.updateIndex();
                    tbl.paginate();
                    ShowMessage("Produto adicionado com sucesso", "sucesso");
                }
                window.Functions.handlers.filter();

                Produto.limparCampos(form);
            } else {
                ShowMessage("Este produto já foi adicionado", "erro");
            }
        }
    },

    getForm: function () {
        return form = {
            inputDescricao: $('#Produto_descricao'),
            inputNome: $('#Produto_nome'),
            inputMargemGanho: $('#Produto_margemGanho'),
            inputNmProjetista: $('#Projetista_nome'),
            inputNmMarceneiro: $('#Marceneiro_nome'),
            inputIdProtista: $('#Projetista_idFuncionario'),
            inputIdMarceneiro: $('#Marceneiro_idFuncionario'),
            inputPorcentagemProjetista: $('#Produto_porcentagemProjetista'),
            inputPorcentagemMarceneiro: $('#Produto_porcentagemMarceneiro'),

            spanDescricao: $('#descricao-valid'),
            spanNome: $('#nome-valid'),
            spanMargemGanho: $('#margemGanho-valid'),
            spanProjetista: $('#Projetista'),
            spanMarceneiro: $('#Marceneiro')
        };
    },
    limparCampos: function (form) {
        form.inputDescricao.val('');
        form.inputNome.val('');
        form.inputNmProjetista.val('');
        form.inputNmMarceneiro.val('');
        form.inputIdProtista.val('');
        form.inputIdMarceneiro.val('');
        form.inputMargemGanho.val('');
        form.inputPorcentagemProjetista.val('');
        form.inputPorcentagemMarceneiro.val('');
    },

    validarForm: function (form) {
        validarCampo(form.spanDescricao, form.inputDescricao, "Informe a descrição");
        validarCampo(form.spanNome, form.inputNome, "Informe o nome");

        return (!window.IsNullOrEmpty(form.inputDescricao.val()) &&
            !window.IsNullOrEmpty(form.inputNome.val()));
    },

    remover: function (row) {
        var tbl = $('#tblProduto').tGrid();
        tbl.removeRow($(row));
        tbl.updateIndex();
        tbl.paginate();
    },

    edit: function (el) {
        var tbl = $('#tblProduto').tGrid();
        var row = $(el).parents('tr.data');
        var data = tbl.getRowData(row);
        rowEdit = row;
        $('#Produto_descricao').val(data.descricao);
        $('#Produto_nome').val(data.nome);
        $('#Produto_margemGanho').val(data.margemGanho);
        $('#Projetista_nome').val(data.projetista);
        $('#Marceneiro_nome').val(data.marceneiro);
        $('#Projetista_idFuncionario').val(data.idProjetista);
        $('#Marceneiro_idFuncionario').val(data.idMarceneiro);
        $('#Produto_porcentagemProjetista').val(data.porcentagemProjetista);
        $('#Produto_porcentagemMarceneiro').val(data.porcentagemMarceneiro);
        $("#dlg-produto").modal('show');
    },

    estrutura:
        '<tr class="data">' +
            '<td class="actions b2" >' +
                '<a href="#" class="btn btn-danger" title="Excluir" data-event="remove"><i class="icon-trash icon-white"></i></a>' +
                '<a href="#" class="btn btn-info" title="Editar" data-event="edit"><i class="icon-check icon-white"></i></a>' +
                '<input type="hidden" name="Produtos[0].idProduto" value="${idProduto}"/>' +
                '<input type="hidden" name="Produtos[0].idProjetista" value="${idProjetista}"/>' +
                '<input type="hidden" name="Produtos[0].idMarceneiro" value="${idMarceneiro}"/>' +
                '<input type="hidden" name="Produtos[0].vlProduto" value="${vlProduto}"/>' +
                '<input type="hidden" name="Produtos[0].vlLiquidoDesconto" value="${vlLiquidoDesconto}"/>' +
                '<input type="hidden" name="Produtos[0].vlLiquido" value="${vlLiquido}"/>' +
                '<input type="hidden" name="Produtos[0].vlDesconto" value="${vlDesconto}"/>' +
                '<input type="hidden" name="Produtos[0].vlVenda" value="${vlVenda}"/>' +
                '<input type="hidden" name="Produtos[0].nome" value="${nome}"/>' +
                '<input type="hidden" name="Produtos[0].vlLucro" value="${vlLucro}"/>' +
                '<input type="hidden" name="Produtos[0].vlCustoMaterial" value="${vlCustoMaterial}"/>' +
                '<input type="hidden" name="Produtos[0].vlTotalMaterial" value="${vlTotalMaterial}"/>' +
                '<input type="hidden" name="Produtos[0].vlLiquidoMaterial" value="${vlLiquidoMaterial}"/>' +
                '<input type="hidden" name="Produtos[0].descricao" value="${descricao}"/>' +
                '<input type="hidden" name="Produtos[0].projetista" value="${projetista}"/>' +
                '<input type="hidden" name="Produtos[0].comissaoProjetista" value="${comissaoProjetista}"/>' +
                '<input type="hidden" name="Produtos[0].marceneiro" value="${marceneiro}"/>' +
                '<input type="hidden" name="Produtos[0].comissaoMarceneiro" value="${comissaoMarceneiro}"/>' +
            '</td>' +
            '<td>${nome}</td>' +
            '<td>${projetista}<span class="pull-right"><input type="text" style="text-align:right" name="Produtos[0].porcentagemProjetista"  filter="floatnumber" value="${porcentagemProjetista}" class="input-small"></span></td>' +
            '<td>${marceneiro}<span class="pull-right"><input type="text" style="text-align:right" name="Produtos[0].porcentagemMarceneiro"  filter="floatnumber" value="${porcentagemMarceneiro}" class="input-small"></span></td>' +
            '<td  style="text-align: right;">          <input type="text" style="text-align:right" name="Produtos[0].margemGanho"            filter="floatnumber" value="${margemGanho}"           class="input-small"></td>' +
         '</tr>'
};
