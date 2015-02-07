$(function () {
    var tbPerfil = $('#tblPerfil').tGrid({
        headerButtons: ['#btn-open-perfil'],
        pager: {
            enabled: true,
            size: 10
        },
        onCreate: function (grid) {
            var list = Functions.getJsonData('#tblPerfil_data');
            if (list) {
                for (var i = 0; i < list.length; i++) {
                    grid.addRow(Perfil.estrutura, { IdPerfil: list[i].IdPerfil, Nome: list[i].Nome });
                }
                grid.updateIndex();
                grid.paginate();
            }
        },
        onUpdate: function (grid) {
            $('#tblPerfil_count').val(grid.count());
        }
    });

    $('#dlg-perfil').dialog({
        width: 900
    })
    .on('shown', function () {
        $('#Perfil_idPerfil').focus();
    });

    $("#btn-add-perfil").click(function () {
        if (Perfil.validarCampos()) {
            if (!tbPerfil.hasUniqueValue($("#Perfil_idPerfil").val())) {
                tbPerfil.addRow(Perfil.estrutura, { IdPerfil: $("#Perfil_idPerfil").val(), Nome: $("#Perfil_nmPerfil").val() });
                tbPerfil.updateIndex();
                tbPerfil.paginate();
                Perfil.limparCampos();
            }
            else {
                alert('Esta perfil já foi adicionada ao usuario');
            }
        }
        else {
            alert('Informe corretamente os valores do perfil');
        }

        $("#Perfil_idPerfil").focus();
        return false;
    });
});

var Perfil = {
    validarCampos: function () {
        return (!IsNullOrEmpty($("#Perfil_idPerfil").val()) && !IsNullOrEmpty($("#Perfil_nmPerfil").val()));
    },

    limparCampos: function () {
        $("#Perfil_idPerfil").val('');
        $("#Perfil_nmPerfil").val('');
    },

    remover: function (row) {
        var tbPerfil = $('#tblPerfil').tGrid();
        tbPerfil.removeRow($(row));
        tbPerfil.updateIndex();
        tbPerfil.paginate();
    },

    estrutura:
        '<tr class="data">' +
            '<td class="actions b1">' +
                '<a href="#" class="tgrid-delete btn btn-danger" title="Excluir" onclick="Perfil.remover(this); return false"><i class="icon-trash icon-white"></i></a>' +
                '<input type="hidden" name="Perfis[0].IdPerfil" unique="true" value="${IdPerfil}"/>' +
                '<input type="hidden" name="Perfis[0].Nome" value="${Nome}"/>' +
            '</td>' +
            '<td>${IdPerfil}</td>' +
            '<td>${Nome}</td>' +
        '</tr>'
}