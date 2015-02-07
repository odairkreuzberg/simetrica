$(function () {
    $('#nome').focus();

    if ($('#tipo').val() == 'Físico') {
        $('#dv-cnpj').hide();
        $('#cnpj').val('')
        $('#dv-cpf').show();

    } else {
        $('#dv-cpf').hide();
        $('#cpf').val('')
        $('#dv-cnpj').show();
    }

    $('#tipo').change(function () {
        if ($('#tipo').val() == 'Físico') {
            $('#dv-cnpj').hide();
            $('#cnpj').val('')
            $('#dv-cpf').show();

        } else {
            $('#dv-cpf').hide();
            $('#cpf').val('')
            $('#dv-cnpj').show();
        }
    });

    $('#dlg-telefone').dialog({
    });

    $('#add-telefone').click(function () {
        Telefone.adicionar();
        return false;
    });

    $('#tblTelefone').on({
        click: function ()
        { Telefone.remover(this); return false; }
    }, 'a[data-event=remove]');



    $('#tblTelefone').tGrid({
        searchable: false,
        addMode: "append",
        headerButtons: ['#open-telefone'],
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

    adicionar: function () {
        var form = Telefone.getForm();

        if (Telefone.validarForm(form)) {

            var tbl = $('#tblTelefone').tGrid();

            if (!tbl.hasUniqueValue(form.inputNumero.val())) {

                tbl.addRow(Telefone.estrutura, { tipo: form.inputTipo.val(), numero: form.inputNumero.val() });
                tbl.updateIndex();
                tbl.paginate();
                ShowMessage("Telefone adicionado com sucesso", "sucesso");

                Telefone.limparCampos(form);
            } else {
                ShowMessage("Este telefone já foi adicionado", "erro");
            }
        } 
    },

    getForm: function () {
        return form = {
            inputTipo: $('#Telefone_tipo'),
            inputNumero: $('#Telefone_numero'),
            spanTipo: $('#tipo-valid'),
            spanNumero: $('#numero-valid')
        };
    },
    limparCampos: function (form) {
        form.inputNumero.val('');
        form.inputTipo.val('');
    },

    validarForm: function (form) {
        validarCampo(form.spanTipo, form.inputTipo, "Informe o tipo");
        validarCampo(form.spanNumero, form.inputNumero, "Informe o número");

        return (!window.IsNullOrEmpty(form.inputTipo.val()) && !window.IsNullOrEmpty(form.inputNumero.val()));
    },

    remover: function (row) {
        var tbl = $('#tblTelefone').tGrid();
        tbl.removeRow($(row));
        tbl.updateIndex();
        tbl.paginate();
    },

    estrutura:
        '<tr class="data">' +
            '<td >' +
                '<a href="#" class="btn btn-danger" title="Excluir" data-event="remove"><i class="icon-trash icon-white"></i></a>' +
                '<input type="hidden" name="Telefones[0].numero" unique="true" value="${numero}"/>' +
                '<input type="hidden" name="Telefones[0].tipo" value="${tipo}"/>' +
            '</td>' +
            '<td>${tipo}</td>' +
            '<td>${numero}</td>' +
         '</tr>'
};
