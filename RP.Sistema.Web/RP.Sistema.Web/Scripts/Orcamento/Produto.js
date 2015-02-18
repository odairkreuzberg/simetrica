$(function () {

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
                    window.Functions.handlers.mask();
                };
                work.start();
            }

        },
        onUpdate: function (grid) {
            $('#tblItem_count').val(grid.count());
        }
    });
});

var Item = {
    adicionar: function () {
        var form = Item.getForm();

        if (Item.validarForm(form)) {

            var tbl = $('#tblItem').tGrid();

            if (!tbl.hasUniqueValue(form.inputIdMaterial.val())) {

                tbl.addRow(Item.estrutura, {
                    idMaterial: form.inputIdMaterial.val(),
                    nome: form.imputNome.val(),
                    quantidade: form.inputQuantidade.val()
                });
                ShowMessage("Item adicionado com sucesso", "sucesso");
                Item.limparCampos(form);
                tbl.updateIndex();
                tbl.paginate();
                window.Functions.handlers.mask();
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
            spanQuantidade: $('#quantidade-valid'),
            spanMaterial: $('#Material')
        };
    },
    limparCampos: function (form) {
        form.imputNome.val('');
        form.inputIdMaterial.val('');
        form.inputQuantidade.val('');
    },

    validarForm: function (form) {
        validarCampo(form.spanMaterial, form.imputNome, "Informe o material");
        validarCampo(form.spanQuantidade, form.inputQuantidade, "Informe a quantidade");

        return (!window.IsNullOrEmpty(form.imputNome.val()) && !window.IsNullOrEmpty(form.inputQuantidade.val()));
    },

    remover: function (row) {
        var tbl = $('#tblItem').tGrid();
        tbl.removeRow($(row));
        tbl.updateIndex();
        tbl.paginate();
    },
    formpatData: function (item) {
        var data = {
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
            '</td>' +
            '<td>${nome}</td>' +
            '<td style="text-align: right">' +
                '<input type="text" style="text-align: right; margin: 0" name="Itens[0].quantidade" value="${quantidade}" filter="floatnumber" class="input-small" >' +
            '</td>' +
         '</tr>'
};